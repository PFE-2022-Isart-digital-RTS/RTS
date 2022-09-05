using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildContext", menuName = "ScriptableObjects/BuildContext", order = 1)]
public class BuildContext : ContextualMenuItem
{
    [SerializeField]
    private GameObject buildingToSpawnPrefab;

    private NetworkObjectReference[] m_targets;

    public override void OnInvoked(List<HaveOptionsComponent> targets)
    {
        int length = targets.Count;
        m_targets = new NetworkObjectReference[length];
        for (int i = 0; i < length; i++)
        {
            m_targets[i] = targets[i].GetComponent<NetworkObject>();
        }

        RTSPlayerController.LocalInstance.RequestPosition += OnPositionIndicate;
    }

    // Unused because of rpc
    public override void OnPurchaseEnd(HaveOptionsComponent purchasedFrom)
    {
        throw new NotImplementedException();
    }

    void OnPositionIndicate(Vector3 position)
    {
        RTSPlayerController.LocalInstance.TryBuildServerRPC(m_targets, position, buildingToSpawnPrefab.name);
    }
}