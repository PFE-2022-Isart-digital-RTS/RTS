using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextualMenuItem", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
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

    protected void Purchase(HaveOptionsComponent purchasedFrom)
    {
        RTSPlayerController.LocalInstance.PlayerState.Team.nbGolds -= nbGoldsRequired;
        OnPurchase(purchasedFrom);
    }

    protected abstract void OnPurchase(HaveOptionsComponent purchasedFrom);

    public void TryPurchase(HaveOptionsComponent purchasedFrom)
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

    public void OnInvoked(List<HaveOptionsComponent> contextualizables)
    {
        NetworkBehaviourReference[] behaviours = Array.ConvertAll(contextualizables.ToArray(), item => (NetworkBehaviourReference)item);

        //TryPurchase();
        RTSPlayerController.LocalInstance.TryBuyItemServerRPC(ActionName, behaviours);
    }
}

