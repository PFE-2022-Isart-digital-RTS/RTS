using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    NetworkVariable<NetworkBehaviourReference> playerStateRef = new NetworkVariable<NetworkBehaviourReference>();

    public PlayerState PlayerState { get => (PlayerState) playerStateRef.Value; set => playerStateRef.Value = value; }
    public static PlayerController LocalInstance { get; private set; }

    [ClientRpc()]
    public void SetLocalInstance_ClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LocalInstance = this;
    }

    public void SetEnable(bool isEnabled, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner)
            enabled = isEnabled;
        else
            gameObject.SetActive(false);

        SetEnable_ClientRpc(isEnabled, clientRpcParams);
    }

    [ClientRpc]
    private void SetEnable_ClientRpc(bool isEnabled, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner)
            enabled = isEnabled;
        else
            gameObject.SetActive(false);
    }
}
