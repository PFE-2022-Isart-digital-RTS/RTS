using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public static PlayerController LocalInstance { get; private set; }

    [ClientRpc()]
    public void SetLocalInstance_ClientRpc(NetworkBehaviourReference pStateRef, ClientRpcParams clientRpcParams = default)
    {
        if (pStateRef.TryGet(out PlayerState pState))
        {
            PlayerState = pState;
        }
        LocalInstance = this;
    }
    
    [ClientRpc]
    public void SetEnable_ClientRpc(bool isEnabled)
    {
        enabled = isEnabled;
    }
}
