using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildContext", menuName = "ScriptableObjects/BuildContext", order = 1)]
public class BuildContext : ContextualMenuItemBase
{
    [SerializeField]
    private GameObject buildingToSpawnPrefab;

    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        new Context() { Data = this }.OnInvoked(contextualizables);
    }

    public new class Context : ContextualMenuItemBase.Context
    {
        private NetworkObjectReference[] m_targets;
        public new BuildContext Data
        {
            get => (BuildContext) data;
            set => data = value;
        }

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

        void OnPositionIndicate(Vector3 position)
        {
            RTSPlayerController.LocalInstance.TryBuildServerRPC(m_targets, position, Data.buildingToSpawnPrefab.name);
        }
    }
}