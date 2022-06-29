using System.Collections;
using System.Collections.Generic;
using FogOfWarPackage;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

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
    List<TeamComponent> units = new List<TeamComponent>();

    List<HaveOptionsComponent> selectables = new List<HaveOptionsComponent>();
    
    public TerrainFogOfWar[] fogsOfWar;

    public List<TeamComponent> Units { get => units; }
    public List<HaveOptionsComponent> Selectables { get => selectables; }

    private void OnEnable()
    {
        if (IsClient)
        {
            Volume postProcessVolume = FindObjectOfType<Volume>();
            fogsOfWar = FindObjectsOfType<TerrainFogOfWar>();
            postProcessVolume.profile.TryGet(out FogOfWarPostProcess fow);
            fow.terrainsFogOfWar.value = fogsOfWar;
        }

        if (IsServer)
        {
            foreach (TeamComponent e in GameObject.FindObjectsOfType<TeamComponent>())
            {
                if (e.Team == this)
                {
                    RegisterUnit(e);
                }
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

    public void RegisterUnit(TeamComponent unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);

        HaveOptionsComponent haveOptionsComp = unit.GetComponent<HaveOptionsComponent>();
        if (haveOptionsComp != null && !selectables.Contains(haveOptionsComp))
            selectables.Add(haveOptionsComp);

        // Server
        RegisterUnitInFogOfWar(unit);
        
        RegisterUnitClientRpc(unit);
    }

    void RegisterUnitInFogOfWar(TeamComponent unit)
    {
        for (int index = 0; index < fogsOfWar.Length; index++)
        {
            TerrainFogOfWar fogOfWar = fogsOfWar[index];
            fogOfWar.RegisterEntity(unit);
        }
    }

    public void UnregisterUnit(TeamComponent unit)
    {
        units.Remove(unit);
        
        // Server
        UnregisterUnitInFogOfWar(unit);
        
        UnregisterUnitClientRpc(unit);
    }
    
    void UnregisterUnitInFogOfWar(TeamComponent unit)
    {
        for (int index = 0; index < fogsOfWar.Length; index++)
        {
            TerrainFogOfWar fogOfWar = fogsOfWar[index];
            fogOfWar.UnregisterEntity(unit);
        }
    }

    [ClientRpc]
    void RegisterUnitClientRpc(NetworkBehaviourReference unit)
    {
        if (!IsServer)
            if (unit.TryGet(out TeamComponent addedUnit))
            {
                RegisterUnitInFogOfWar(addedUnit);
                
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
                UnregisterUnitInFogOfWar(removedUnit);
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
