using System;
using Unity.Netcode;

public class NetworkDataExchanger : NetworkBehaviour
{
    /// <summary>
    /// This event is raised on the server when an action request arrives
    /// </summary>
    public event Action<NetworkGameData> DoActionEventServer;
    
    /// <summary>
    /// Client->Server RPC that sends a request to play an action.
    /// </summary>
    /// <param name="data">Data about which action to play and its associated details. </param>
    [ServerRpc]
    public void SendReceiveDataServerRPC(NetworkGameData data)
    {
        DoActionEventServer?.Invoke(data);
    }
}
