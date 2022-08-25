using System.Collections.Generic;
using ContextualMenuPackage;
using Unity.Netcode;
using UnityEngine;

public class Stop : ITask<HaveOptionsComponent>
{
    private NetworkObjectReference[] m_targets;

    public void OnInvoked(List<HaveOptionsComponent> targets)
    {
        int length = targets.Count;
        m_targets = new NetworkObjectReference[length];
        for (int i = 0; i < length; i++)
        {
            m_targets[i] = targets[i].GetComponent<NetworkObject>();
        }

        RTSPlayerController.LocalInstance.TryStopActionServerRPC(m_targets);
    }
}