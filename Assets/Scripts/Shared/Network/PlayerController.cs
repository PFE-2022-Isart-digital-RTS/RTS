using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public static PlayerController LocalInstance { get; private set; }

    [ClientRpc()]
    public void SetLocalInstance_ClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LocalInstance = this;
    }
}
