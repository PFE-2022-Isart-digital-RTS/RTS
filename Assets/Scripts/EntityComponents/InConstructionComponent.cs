using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanConstruct
{ 

}


public class InConstructionComponent : MonoBehaviour
{
    // Called when a unit starts constructing this entity
    public void OnConstructionHelpStart(ICanConstruct helper)
    {

    }
    // Called when a unit stops constructing this entity
    public void OnConstructionHelpStop(ICanConstruct helper)
    {

    }
    // Called when this entity has finished being constructed
    public void OnConstructionFinished()
    {

    }

}
