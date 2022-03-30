using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInstruction : Instruction
{
    MoveComponent moveComponent;
    Vector3 targetLocation;

    public MoveInstruction(HaveInstructions ownerEntity, Vector3 targetLocation)
    {
        base.ownerEntity = ownerEntity;
        this.targetLocation = targetLocation;
        moveComponent = ownerEntity.GetComponent<MoveComponent>();
    }

    public override bool CanDoInstruction()
    {
        return moveComponent != null;
    }

    public override void OnEnd()
    {

    }

    public override void OnStart()
    {
        moveComponent.MoveTo(targetLocation);
    }

    public override void OnStop()
    {
        moveComponent.Stop();
    }

    public override void OnUpdate()
    {

    }
}
