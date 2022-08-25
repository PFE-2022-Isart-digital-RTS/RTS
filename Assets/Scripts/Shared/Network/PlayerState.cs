using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerState : NetworkBehaviour
{
    public NetworkClient client;

    [ClientRpc]
    public void SetEnable_ClientRpc(bool isEnabled)
    {
        enabled = isEnabled;
    }
}

