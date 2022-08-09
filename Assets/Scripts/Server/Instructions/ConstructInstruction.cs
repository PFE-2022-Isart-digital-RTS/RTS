using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructInstruction : Instruction
{
    HaveInstructions ownerEntity;
    MoveComponent moveComponent;
    ICanConstruct canConstructComponent;
    InConstructionComponent entityToConstruct;
    bool isBuilding = false;

    public ConstructInstruction(HaveInstructions ownerEntity, InConstructionComponent entityToConstruct)
    {
        this.ownerEntity = ownerEntity;
        this.entityToConstruct = entityToConstruct;
        moveComponent = ownerEntity.GetComponent<MoveComponent>();
        canConstructComponent = ownerEntity.GetComponent<ICanConstruct>();
    }

    public override bool CanDoInstruction()
    {
        return moveComponent != null && canConstructComponent != null; 
    }

    public override void OnEnd()
    {

    }

    public override void OnStart()
    {
        moveComponent.MoveTo(entityToConstruct.transform.position);
    }

    public override void OnStop()
    {
        if (isBuilding)
        {
            entityToConstruct.OnConstructionHelpStop(canConstructComponent);
        }
        else
        {
            moveComponent.Stop();
        }
    }

    public override void OnUpdate()
    {
        // Can be optimized with square distance
        // TODO : Should be modified, this is just an example, 10 should not be a raw value
        // Example : use something like GetComponent<Building> and get the size of the building etc
        if (Vector3.Distance(ownerEntity.transform.position, entityToConstruct.transform.position) < 10)
        {
            moveComponent.Stop();
            isBuilding = true;
            entityToConstruct.OnConstructionHelpStart(canConstructComponent);
        }
    }
}
