using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


// Contains minimal server data that is team specific
// Contains : 
// - The units of the team
// - Teams' relations
// Can be use for teams of monsters etc (simple behavior)
public class TeamStateBase : NetworkBehaviour
{
    //// Buildings and units that the team owns
    [SerializeField]
    List<TeamComponent> units = new List<TeamComponent>();

    public List<TeamComponent> Units { get => units; }

    public UnityEvent<TeamComponent> onUnitRegistered = new UnityEvent<TeamComponent>();
    public UnityEvent<TeamComponent> onUnitUnregistered = new UnityEvent<TeamComponent>();

    #region UnitRegistration

    public virtual void RegisterUnit(TeamComponent unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);

        onUnitRegistered?.Invoke(unit);

        RegisterUnitClientRpc(unit);
    }

    public void UnregisterUnit(TeamComponent unit)
    {
        units.Remove(unit);
        onUnitUnregistered?.Invoke(unit);
        UnregisterUnitClientRpc(unit);
    }

    [ClientRpc]
    void RegisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out TeamComponent addedUnit))
            {
                if (!units.Contains(addedUnit))
                    units.Add(addedUnit);

                onUnitRegistered?.Invoke(addedUnit);
            }
    }

    [ClientRpc]
    public void UnregisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out TeamComponent removedUnit))
            {
                units.Remove(removedUnit);
                onUnitUnregistered?.Invoke(removedUnit);
            }
    }

    #endregion

    #region Teams

    public enum TeamRelation
    {
        Equal,
        Neutral,
        Ally,
        Enemy
    }

    public TeamRelation GetRelationTo(TeamStateBase otherTeam)
    {
        if (otherTeam == null)
            return TeamRelation.Neutral;

        if (this == otherTeam)
            return TeamRelation.Equal;

        if (IsAlly(otherTeam))
            return TeamRelation.Ally;

        if (!IsEnemy(otherTeam))
            Debug.LogError("IsAlly or IsEnemy is not implemented properly : two different teams should either be allies or enemies");

        return TeamRelation.Enemy;
    }

    public virtual bool IsAlly(TeamStateBase otherTeam)
    {
        // ally teams are currently not supported, so every other team is an enemy team
        return false;
    }

    public bool IsEnemy(TeamStateBase otherTeam)
    {
        // Other team is an enemy if :
        // - it is not the same team
        // - it is not an ally
        return this != otherTeam && !IsAlly(otherTeam);
    }

    #endregion

    // Unity Functions
    // often based around space, because units could be stored efficiently with a Quadtree,
    // and the implementation of these functions would then change depending on this class' units' storage
    #region Utility 

    // Returns the first unit with the predicate to true
    public GameObject GetUnit(System.Func<TeamComponent, bool> predicate)
    {
        foreach (TeamComponent unit in Units)
        {
            if (predicate.Invoke(unit))
            {
                return unit.gameObject;
            }
        }
        return null;
    }

    public ICollection<GameObject> GetUnits(System.Func<TeamComponent, bool> predicate)
    {
        List<GameObject> returnedUnits = new List<GameObject>(Units.Count);
        foreach (TeamComponent unit in Units)
        {
            if (predicate.Invoke(unit))
            {
                returnedUnits.Add(unit.gameObject);
            }
        }
        return returnedUnits;
    }

    public GameObject GetUnit(System.Func<GameObject, GameObject, GameObject> predicate)
    {
        GameObject returned = null;
        foreach (TeamComponent unit in Units)
        {
            returned = predicate.Invoke(returned, unit.gameObject);
        }
        return returned;
    }

    public GameObject GetClosestUnit(Vector3 pos)
    {
        float sqrDist = float.MaxValue;

        return GetUnit((GameObject unit, GameObject otherUnit) =>
        {
            float currentSqrDist = (otherUnit.transform.position - pos).sqrMagnitude;
            if (currentSqrDist > sqrDist)
            {
                sqrDist = currentSqrDist;
                return otherUnit;
            }
            return unit;
        });
    }

    public ICollection<GameObject> GetUnitsInRange(Vector3 pos, float maxDist)
    {
        float maxDistSqr = maxDist * maxDist;
        return GetUnits((TeamComponent unit) =>
        {
            float currentSqrDist = (unit.transform.position - pos).sqrMagnitude;

            return currentSqrDist <= maxDistSqr;
        });
    }

    #endregion
}
