using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ContextualMenuItemBase : ScriptableObject, ITask<HaveOptionsComponent>
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; }

    //public abstract void OnInvoked(List<HaveOptionsComponent> contextualizables);

    public virtual string ItemName
    {
        get => name;
    }

    public virtual string ActionName
    {
        get => GetActionName(ItemName);
    }

    public static string GetActionName(string itemName)
    {
        return "buy " + itemName;
    }

    public abstract void OnInvoked(List<HaveOptionsComponent> contextualizables);

    // Server Side
    public abstract class InstructionGenerator
    {
        protected ContextualMenuItemBase data;
        public ContextualMenuItemBase Data
        {
            get => data;
            set => data = value;
        }
    }

    // Client Side
    public abstract class Context : ITask<HaveOptionsComponent> 
    {
        protected ContextualMenuItemBase data;
        public ContextualMenuItemBase Data
        {
            get => data;
            set => data = value;
        }

        public NetworkObjectReference[] HaveOptionsToNetworkRefs(List<HaveOptionsComponent> targets)
        {
            int length = targets.Count;
            NetworkObjectReference[] targetRefs = new NetworkObjectReference[length];
            for (int i = 0; i < length; i++)
            {
                targetRefs[i] = targets[i].GetComponent<NetworkObject>();
            }
            return targetRefs;
        }

        public abstract void OnInvoked(List<HaveOptionsComponent> contextualizables);
    }
}


[CreateAssetMenu(fileName = "ContextualMenuItem", menuName = "ScriptableObjects/ContextualMenuItem", order = 1)]
public abstract class ContextualMenuItem : ContextualMenuItemBase
{
    [SerializeField]
    TeamResources finalPrice;

    public TeamResources FinalPrice { get => finalPrice; }

    public float buyDuration = 2f;
    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        new Context() { Data = this }.OnInvoked(contextualizables);
    }

    public abstract ContextualMenuItemBase.InstructionGenerator GetInstructionGenerator();

    // Server Side
    public abstract new class InstructionGenerator : ContextualMenuItemBase.InstructionGenerator
    {
        public new ContextualMenuItem Data
        {
            get => (ContextualMenuItem)data;
            set => data = value;
        }

        virtual public bool CanPurchaseFinalPrice(TeamState teamState)
        {
            return teamState.Resources >= Data.FinalPrice;
        }
        virtual public void PayFinalPrice(TeamState teamState)
        {
            teamState.Resources -= Data.FinalPrice;
        }

        public virtual void OnPurchaseStart(List<HaveOptionsComponent> purchasedFromList)
        {
            foreach (HaveOptionsComponent purchasedFrom in purchasedFromList)
            {
                OnPurchaseStart(purchasedFrom);
            }
        }

        public virtual void OnPurchaseStart(HaveOptionsComponent purchasedFrom)
        {
            TeamComponent teamComp = purchasedFrom.GetComponent<TeamComponent>();
            if (teamComp == null)
            {
                Debug.LogError("Entity should have a TeamComponent");
                return;
            }

            TeamState teamState = (TeamState)teamComp.Team;
            if (teamState == null)
            {
                Debug.LogError("TeamStateBase should be a TeamState to buy something");
                return;
            }

            if (CanPurchaseFinalPrice(teamState)) // Can pay
            {
                teamState.Resources -= Data.FinalPrice;
                OnPurchaseEnd(purchasedFrom);
            }
        }
        public abstract void OnPurchaseEnd(HaveOptionsComponent purchasedFrom);
    }

    // Client Side
    public new class Context : ContextualMenuItemBase.Context
    {
        public new ContextualMenuItem Data
        {
            get => (ContextualMenuItem) data;
            set => data = value;
        }

        public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
        {
            NetworkBehaviourReference[] behaviours = Array.ConvertAll(contextualizables.ToArray(), item => (NetworkBehaviourReference)item);
            RTSPlayerController.LocalInstance.TryBuyItemServerRPC(data.ActionName, behaviours);
        }
    }
}

