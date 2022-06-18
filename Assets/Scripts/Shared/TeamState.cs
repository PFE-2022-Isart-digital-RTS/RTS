using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Contains server data that is team specific
public class TeamState : NetworkBehaviour
{
    //public List<RTSPlayerState> playersInTheTeam;

    // Resources
    public float nbGolds;
    public float nbSupplies;

    //// Buildings and units that the team owns
    //List<GameObject> entities;
    [SerializeField]
    List<Entity> units = new List<Entity>();

    public List<Entity> Units { get => units; }

    private void OnEnable()
    {
        if (!IsServer)
            return;

        foreach (Entity e in GameObject.FindObjectsOfType<Entity>())
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

        foreach (Entity e in GameObject.FindObjectsOfType<Entity>())
        {
            if (e.Team == this)
            {
                UnregisterUnit(e);
            }
        }
    }

    public void RegisterUnit(Entity unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);
        RegisterUnitClientRpc(unit);
    }

    public void UnregisterUnit(Entity unit)
    {
        units.Remove(unit);
        UnregisterUnitClientRpc(unit);
    }

    [ClientRpc]
    void RegisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out Entity addedUnit))
            {
                if (!units.Contains(addedUnit))
                    units.Add(addedUnit);
            }
    }

    [ClientRpc]
    public void UnregisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out Entity removedUnit))
            {
                units.Remove(removedUnit);
            }
    }


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

    [ClientRpc]
    public void SetEnable_ClientRpc(bool isEnabled)
    {
        enabled = isEnabled;
    }
}
