using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSSpectatorController : PlayerController
{
    public new RTSSpectatorState PlayerState { get => (RTSSpectatorState)base.PlayerState; set => base.PlayerState = value; }
    public static new RTSSpectatorController LocalInstance { get => (RTSSpectatorController)PlayerController.LocalInstance; }
}
