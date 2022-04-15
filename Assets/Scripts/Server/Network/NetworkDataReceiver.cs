using System;
using Shared.Task;
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
                EntityPositionData taskData = (EntityPositionData)data.obj;

                NetworkObject obj;
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(taskData.entityId, out obj);
                Debug.Log($"{obj.gameObject.name} moving to {(Vector3)taskData.targetPos}");
                obj.gameObject.GetComponent<Entity>().MoveTo((Vector3)taskData.targetPos);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
