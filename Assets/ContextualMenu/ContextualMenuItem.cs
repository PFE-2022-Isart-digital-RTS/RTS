using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextualMenuItem", menuName = "ScriptableObjects/ContextualMenuItem", order = 1)]
public abstract class ContextualMenuItem : ScriptableObject, ITask<HaveOptionsComponent>
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; }
    
    public int nbGoldsRequired = 5;

    virtual public bool CanPurchase
    {
        get => nbGoldsRequired <= RTSPlayerController.LocalInstance.PlayerState.Team.nbGolds;
    }

    protected void Purchase(List<HaveOptionsComponent> purchasedFrom)
    {
        RTSPlayerController.LocalInstance.PlayerState.Team.nbGolds -= nbGoldsRequired;
        OnPurchase(purchasedFrom);
    }

    protected abstract void OnPurchase(List<HaveOptionsComponent> purchasedFrom);

    public void TryPurchase(List<HaveOptionsComponent> purchasedFrom)
    {
        if (CanPurchase)
        {
            Purchase(purchasedFrom);
        }
    }

    public virtual string ItemName
    {
        get => name;
    }

    public virtual string ActionName
    {
        get => "buy " + ItemName;
    }

    // Client Side
    public virtual void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        NetworkBehaviourReference[] behaviours = Array.ConvertAll(contextualizables.ToArray(), item => (NetworkBehaviourReference)item);
        RTSPlayerController.LocalInstance.TryBuyItemServerRPC(ActionName, behaviours);
    }
}

