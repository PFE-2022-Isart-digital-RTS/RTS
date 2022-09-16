using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackEntityContext", menuName = "ScriptableObjects/AttackEntityContext", order = 1)]
public class AttackEntityContext : ContextualMenuItemBase
{
    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        new Context() { Data = this }.OnInvoked(contextualizables);
    }

    public new class Context : ContextualMenuItemBase.Context
    {
        private NetworkObjectReference[] m_targets;
        public new AttackEntityContext Data
        {
            get => (AttackEntityContext)data;
            set => data = value;
        }

        public override void OnInvoked(List<HaveOptionsComponent> targets)
        {
            m_targets = HaveOptionsToNetworkRefs(targets);

            RTSPlayerController.LocalInstance.RequestEntity += OnEntityIndicate;
        }

        void OnEntityIndicate(GameObject entity)
        {
            RTSPlayerController.LocalInstance.TryAttackEntityServerRPC(m_targets, entity);
        }
    }
}