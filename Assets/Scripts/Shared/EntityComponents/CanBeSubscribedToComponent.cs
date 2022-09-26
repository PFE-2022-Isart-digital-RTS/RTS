using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CanBeSubscribedToComponent : NetworkBehaviour
{
    [SerializeField]
    float speedMultiplier = 1f;

    [SerializeField]
    bool shouldDestroyOnComplete = false;

    bool hasBeenCompletedOnce = false;

    private HashSet<CanSubscribeComponent> subscribers = new HashSet<CanSubscribeComponent>();

    public float SpeedMultiplier { get => speedMultiplier; }
    public HashSet<CanSubscribeComponent> Subscribers { get => subscribers; }

    public UnityEvent onFirstComplete;
    public UnityEvent onComplete;
    public UnityEvent onInterrupted;
    public UnityEvent onEnd;

    // Called when a unit starts constructing this entity
    public virtual void Subscribe(CanSubscribeComponent helper)
    {
        subscribers.Add(helper);
    }

    // Called when a unit stops constructing this entity
    public virtual void Unsubscribe(CanSubscribeComponent helper)
    {
        subscribers.Remove(helper);
    }

    public virtual void InterruptSubscription()
    {
        onInterrupted?.Invoke();
        EndSubscription();
    }

    public virtual void CompleteSubscription()
    {
        if (!hasBeenCompletedOnce)
        {
            onFirstComplete?.Invoke();
            hasBeenCompletedOnce = true;
        }
        onComplete?.Invoke();
        EndSubscription();

        if (shouldDestroyOnComplete)
            Destroy(this);
    }

    // Called when this entity has finished being constructed
    protected void EndSubscription()
    {
        HashSet<CanSubscribeComponent> subsCopy = new HashSet<CanSubscribeComponent>(subscribers);
        foreach (CanSubscribeComponent sub in subsCopy)
        {
            Unsubscribe(sub);
        }

        onEnd?.Invoke();
    }

    protected virtual void OnDisable()
    {
        InterruptSubscription();
    }
}
