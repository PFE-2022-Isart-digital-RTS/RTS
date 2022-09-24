using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildContext", menuName = "ScriptableObjects/BuildContext", order = 1)]
public class BuildContext : ContextualMenuItemBase
{
    public GameObject buildingToSpawnPrefab;

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
            m_targets = HaveOptionsToNetworkRefs(targets);

            RTSPlayerController.LocalInstance.RequestPosition += OnPositionIndicate;
        }

        void OnPositionIndicate(Vector3 position)
        {
            RTSPlayerController.LocalInstance.TryBuildServerRPC(m_targets, position, Data.ActionName /* Data.buildingToSpawnPrefab.name */);
        }
    }
}