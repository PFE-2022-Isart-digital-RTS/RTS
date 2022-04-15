using System.Collections.Generic;
using ContextualMenuPackage;
using UnityEngine;

public class Stop : ITask<Entity>
{
    public void OnInvoked(List<Entity> targets)
    {
        foreach (Entity target in targets)
        {
            target.StopMovement();
        }
    }
}