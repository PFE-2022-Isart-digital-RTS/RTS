using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LifeComponent))]
public class CanBeRepairedComponent : CanBeSubscribedToComponent
{
    LifeComponent lifeComponent;

    private void Awake()
    {
        lifeComponent = GetComponent<LifeComponent>();

        lifeComponent.OnFullLife.AddListener(CompleteSubscription);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lifeComponent.OnFullLife.RemoveListener(CompleteSubscription);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return; 

        float heal = 0f;
        foreach (CanRepairComponent sub in Subscribers)
        {
            heal += sub.SpeedMultiplier;
        }
        if (lifeComponent.LifeRatio < 1f)
            lifeComponent.Heal(heal * Time.deltaTime * SpeedMultiplier);
    }
}
