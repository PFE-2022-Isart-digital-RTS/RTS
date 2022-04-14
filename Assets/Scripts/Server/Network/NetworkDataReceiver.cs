using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkDataReceiver : NetworkBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    public override void OnNetworkSpawn()
    {
        NetworkDataExchanger.Instance.DoActionEventServer += OnDataReceive;
    }

    public override void OnNetworkDespawn()
    {
        NetworkDataExchanger.Instance.DoActionEventServer -= OnDataReceive;
    }

    void OnDataReceive(NetworkGameData data)
    {
        switch (data.header)
        {
            case EDataHeader.MoveTo:
                Vector3 pos = (Vector3)data.dataByte.DeserializeData();
                Debug.Log($"Move to {pos}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
