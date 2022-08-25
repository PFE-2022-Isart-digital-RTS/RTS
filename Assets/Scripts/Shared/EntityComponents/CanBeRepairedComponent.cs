using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LifeComponent))]
public class CanBeRepairedComponent : MonoBehaviour
{
    [SerializeField]
    float constructionSpeedMultiplier = 1f;

    HashSet<CanRepairComponent> helpers = new HashSet<CanRepairComponent>();
    LifeComponent lifeComponent;

    public UnityEngine.Events.UnityEvent onConstructionSuccess;
    public UnityEngine.Events.UnityEvent onConstructionDestroyed;
    public UnityEngine.Events.UnityEvent onConstructionEnd;

    private void Awake()
    {
        lifeComponent = GetComponent<LifeComponent>();

        lifeComponent.OnFullLife.AddListener(OnConstructionSuccess);
        lifeComponent.OnNoLife.AddListener(OnConstructionDestroyed);
    }

    // Called when a unit starts constructing this entity
    public void OnConstructionHelpStart(CanRepairComponent helper)
    {
        helpers.Add(helper);
    }

    // Called when a unit stops constructing this entity
    public void OnConstructionHelpStop(CanRepairComponent helper)
    {
        helpers.Remove(helper);
    }

    public void OnConstructionDestroyed()
    {
        onConstructionDestroyed?.Invoke();
        OnConstructionFinished();
    }

    public void OnConstructionSuccess()
    {
        onConstructionSuccess?.Invoke();
        OnConstructionFinished();
    }

    // Called when this entity has finished being constructed
    public void OnConstructionFinished()
    {
        //foreach (CanRepairComponent helper in helpers)
        //{
        //    helper.OnConstructionEnd();
        //}

        HashSet<CanRepairComponent> helpersCopy = new HashSet<CanRepairComponent>(helpers);
        foreach (CanRepairComponent helper in helpersCopy)
        {
            OnConstructionHelpStop(helper);
        }

        onConstructionEnd?.Invoke();
    }

    private void OnDisable()
    {
        lifeComponent.OnFullLife.RemoveListener(OnConstructionSuccess);
        lifeComponent.OnNoLife.RemoveListener(OnConstructionDestroyed);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return; 

        float heal = 0f;
        foreach (CanRepairComponent helper in helpers)
        {
            heal += helper.constructionSpeed;
        }
        if (lifeComponent.lifeRatio < 1f)
            lifeComponent.Heal(heal * Time.deltaTime * constructionSpeedMultiplier);
    }
}
