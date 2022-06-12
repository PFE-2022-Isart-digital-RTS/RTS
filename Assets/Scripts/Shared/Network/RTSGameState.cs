using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSGameState : MonoBehaviour
{
    [HideInInspector]
    public List<RTSPlayerState> playerStates;

    [HideInInspector]
    public List<RTSSpectatorState> spectatorStates;
}
