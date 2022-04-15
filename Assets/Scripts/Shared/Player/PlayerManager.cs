using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : SingletonNetworkBehaviour<PlayerManager>
{
    private readonly NetworkVariable<int> m_playerCount = new NetworkVariable<int>();

    public int PlayerCount => m_playerCount.Value;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += id =>
        {
            if (IsServer)
            {
                m_playerCount.Value++;
                Debug.Log($"Client {id} connected");
            }
        };
        
        NetworkManager.Singleton.OnClientDisconnectCallback += id =>
        {
            if (IsServer)
            {
                m_playerCount.Value++;
                Debug.Log($"Client {id} disconnected");
            }
        };
    }
}
