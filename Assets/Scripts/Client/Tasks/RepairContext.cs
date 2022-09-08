using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "RepairContext", menuName = "ScriptableObjects/RepairContext", order = 1)]
public class RepairContext : ContextualMenuItemBase
{
    NetworkBehaviourReference canBeRepairedComp;

    public CanBeRepairedComponent CanBeRepairedComp
    {
        set
        {
            canBeRepairedComp = value;
        }
    }

    private NetworkObjectReference[] m_targets;

    public override void OnInvoked(List<HaveOptionsComponent> targets)
    {
        int length = targets.Count;
        m_targets = new NetworkObjectReference[length];
        for (int i = 0; i < length; i++)
        {
            m_targets[i] = targets[i].GetComponent<NetworkObject>();
        }

        RTSPlayerController.LocalInstance.TryRepairServerRPC(m_targets, canBeRepairedComp);
    }
}