using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "SubToWagonContext", menuName = "ScriptableObjects/SubToWagonContext", order = 1)]
public class SubToWagonContext : ContextualMenuItemBase
{
    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        new Context() { Data = this }.OnInvoked(contextualizables);
    }

    public override string ItemName
    {
        get => "SubToWagon";
    }
    public override string ActionName
    {
        get => ItemName;
    }

    public new class Context : ContextualMenuItemBase.Context
    {
        NetworkBehaviourReference canBeRepairedComp;

        public WagonSubscriptionComponent CanBeRepairedComp
        {
            set => canBeRepairedComp = value; 
        }

        public override void OnInvoked(List<HaveOptionsComponent> targets)
        {
            NetworkObjectReference[] m_targets = HaveOptionsToNetworkRefs(targets);

            RTSPlayerController.LocalInstance.TrySubToWagonServerRPC(m_targets, canBeRepairedComp);
        }
    }
}