using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "RepairContext", menuName = "ScriptableObjects/RepairContext", order = 1)]
public class RepairContext : ContextualMenuItemBase
{
    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        new Context() { Data = this }.OnInvoked(contextualizables);
    }

    public override string ItemName
    {
        get => "Repair";
    }
    public override string ActionName
    {
        get => ItemName;
    }

    public new class Context : ContextualMenuItemBase.Context
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
            m_targets = HaveOptionsToNetworkRefs(targets);

            RTSPlayerController.LocalInstance.TryRepairServerRPC(m_targets, canBeRepairedComp);
        }
    }
}