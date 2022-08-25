using System.Collections.Generic;
using ContextualMenuPackage;
using UnityEngine;

public class Stop : ITask<HaveOptionsComponent>
{
    public void OnInvoked(List<HaveOptionsComponent> targets)
    {
        // TODO : This code should not run on the client, but on the server
        //foreach (Entity target in targets)
        //{
        //    target.StopMovement();
        //}
    }
}