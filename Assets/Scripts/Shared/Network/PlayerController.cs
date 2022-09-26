using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Controller
{
    //NetworkVariable<NetworkBehaviourReference> playerStateRef = new NetworkVariable<NetworkBehaviourReference>();
    PlayerState playerState;

    //public PlayerState PlayerState { get => (PlayerState) playerStateRef.Value; set => playerStateRef.Value = value; }
    public PlayerState PlayerState { get => playerState; set => playerState = value; }
    public static PlayerController LocalInstance { get; private set; }

    [ClientRpc()]
    public void SetLocalInstance_ClientRpc(NetworkBehaviourReference associatedPlayerState, ClientRpcParams clientRpcParams = default)
    {
        LocalInstance = this;
        playerState = (PlayerState) associatedPlayerState;
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
