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

    public abstract void OnInvoked(List<HaveOptionsComponent> contextualizables);

    public virtual string ItemName
    {
        get => name;
    }

    public virtual string ActionName
    {
        get => "buy " + ItemName;
    }
}


[CreateAssetMenu(fileName = "ContextualMenuItem", menuName = "ScriptableObjects/ContextualMenuItem", order = 1)]
public abstract class ContextualMenuItem : ContextualMenuItemBase
{
    [SerializeField]
    TeamResources finalPrice;

    public TeamResources FinalPrice { get => finalPrice; }

    public float buyDuration = 2f;

    virtual public bool CanPurchaseFinalPrice(TeamState teamState)
    {
        return teamState.Resources >= FinalPrice;
    }
    virtual public void PayFinalPrice(TeamState teamState)
    {
        teamState.Resources -= FinalPrice;
    }

    // Client Side
    public override void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        NetworkBehaviourReference[] behaviours = Array.ConvertAll(contextualizables.ToArray(), item => (NetworkBehaviourReference)item);
        RTSPlayerController.LocalInstance.TryBuyItemServerRPC(ActionName, behaviours);
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
        if (CanPurchaseFinalPrice(teamComp.Team)) // Can pay
        {
            teamComp.Team.Resources -= FinalPrice;
            OnPurchaseEnd(purchasedFrom);
        }
    }
    public abstract void OnPurchaseEnd(HaveOptionsComponent purchasedFrom);
}

