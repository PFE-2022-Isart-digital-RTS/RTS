using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public NetworkClient client;


}


// Contains server data that is player specific
public class RTSPlayerState : PlayerState
{
    public TeamState team;


}

public class RTSSpectatorState : PlayerState
{



}
