using System.Collections.Generic;
using ContextualMenuPackage;
using UnityEngine;

public class Stop : ITask<Entity>
{
    public void OnInvoked(List<Entity> targets)
    {
        // TODO : This code should not run on the client, but on the server
        //foreach (Entity target in targets)
        //{
        //    target.StopMovement();
        //}
    }
}