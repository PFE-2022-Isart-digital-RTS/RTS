using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


// Contains minimal server data that is team specific
// Contains : 
// - The units of the team
// - The squads of the units 
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

    // ======== Squads ======== //

    #region Squads

    private HashSet<Squad> squads = new HashSet<Squad>();
    private Dictionary<GameObject, Squad> unitToSquadMap = new Dictionary<GameObject, Squad>();

    public HashSet<Squad> Squads { get => squads; }
    public Dictionary<GameObject, Squad> UnitToSquadMap { get => unitToSquadMap; }

    public void RegisterSquad(Squad squad)
    {
        squads.Add(squad);
    }

    public void UnregisterSquad(Squad squad)
    {
        squads.Remove(squad);
    }

    #endregion

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
        Ally,
        Enemy
    }

    public TeamRelation GetRelationTo(TeamState otherTeam)
    {
        if (this == otherTeam)
            return TeamRelation.Equal;

        if (IsAlly(otherTeam))
            return TeamRelation.Ally;

        if (!IsEnemy(otherTeam))
            Debug.LogError("IsAlly or IsEnemy is not implemented properly : two different teams should either be allies or enemies");

        return TeamRelation.Enemy;
    }

    public virtual bool IsAlly(TeamState otherTeam)
    {
        // ally teams are currently not supported, so every other team is an enemy team
        return false; 
    }

    public bool IsEnemy(TeamState otherTeam)
    {
        // Other team is an enemy if :
        // - it is not the same team
        // - it is not an ally
        return this != otherTeam && !IsAlly(otherTeam);
    }

    #endregion
}
