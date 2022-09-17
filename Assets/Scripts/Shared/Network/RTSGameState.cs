using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSGameState : NetworkBehaviour
{
    [HideInInspector]
    public List<RTSPlayerState> playerStates;

    [HideInInspector]
    public List<RTSSpectatorState> spectatorStates;

    [HideInInspector]
    public List<TeamState> playerTeams;

    public static RTSGameState Instance { get; set; }

    private void OnEnable()
    {
        Instance = this;
    }
}
