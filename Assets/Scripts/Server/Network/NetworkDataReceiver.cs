using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkDataReceiver : NetworkBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += id =>
        {
            if (IsServer)
                NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<NetworkDataExchanger>().DoActionEventServer += OnDataReceive;
        };
        
        NetworkManager.Singleton.OnClientDisconnectCallback += id =>
        {
            if (IsServer)
                NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<NetworkDataExchanger>().DoActionEventServer -= OnDataReceive;
        };
    }
    
    void OnDataReceive(NetworkGameData data)
    {
        switch (data.header)
        {
            case EDataHeader.MoveTo:
                SerializableVector3 pos = (SerializableVector3)data.dataByte.DeserializeData();
                Debug.Log($"Move to {(Vector3)pos}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
