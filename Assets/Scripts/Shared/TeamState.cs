using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Contains server data that is team specific
public class TeamState : NetworkBehaviour
{
    //public List<RTSPlayerState> playersInTheTeam;

    // Resources
    public float nbGolds = 10;
    public float nbSupplies = 10;

    // Upgrades
    public float unitsDamageMultiplier = 1.0f;

    //// Buildings and units that the team owns
    //List<GameObject> entities;
    [SerializeField]
    List<TeamComponent> units = new List<TeamComponent>();

    List<HaveOptionsComponent> selectables = new List<HaveOptionsComponent>();

    public List<TeamComponent> Units { get => units; }
    public List<HaveOptionsComponent> Selectables { get => selectables; }

    // The list of buildings the player can potentially construct
    // This is a safety for networking ; a cheater won't be able to spawn a prefab that is not in the list
    // Used server only
    public List<GameObject> availableBuildings = new List<GameObject>();

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

    private void OnEnable()
    {
        if (!IsServer)
            return;

        foreach (TeamComponent e in GameObject.FindObjectsOfType<TeamComponent>())
        {
            if (e.Team == this)
            {
                RegisterUnit(e);
            }
        }
    }

    private void OnDisable()
    {
        if (!IsServer)
            return;

        foreach (TeamComponent e in GameObject.FindObjectsOfType<TeamComponent>())
        {
            if (e.Team == this)
            {
                UnregisterUnit(e);
            }
        }
    }

    #region UnitRegistration

    public void RegisterUnit(TeamComponent unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);

        HaveOptionsComponent haveOptionsComp = unit.GetComponent<HaveOptionsComponent>();
        if (haveOptionsComp != null && !selectables.Contains(haveOptionsComp))
            selectables.Add(haveOptionsComp);
        
        RegisterUnitClientRpc(unit);
    }

    public void UnregisterUnit(TeamComponent unit)
    {
        units.Remove(unit);
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

                HaveOptionsComponent haveOptionsComp = addedUnit.GetComponent<HaveOptionsComponent>();
                if (haveOptionsComp != null && !selectables.Contains(haveOptionsComp))
                    selectables.Add(haveOptionsComp);
            }
    }

    [ClientRpc]
    public void UnregisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out TeamComponent removedUnit))
            {
                units.Remove(removedUnit);
            }
    }

    #endregion 

    #region OnGameEnd

    // Called when the team loses
    public void OnTeamLose()
    {
        // TODO : display a "you lost" message to the players of the team
        // and make them able to leave the game?

        throw new System.NotImplementedException();
    }

    // Called when the team wins
    public void OnTeamWin()
    {
        // TODO : display a "you won" message to the players of the team?

        throw new System.NotImplementedException();
    }

    #endregion

    #region OnGameLoad

    [ClientRpc]
    public void SetEnable_ClientRpc(bool isEnabled)
    {
        enabled = isEnabled;
    }

    #endregion
}
