using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveInstructionGenerator
{ 
    Instruction GenerateInstruction(MoveComponent moveComponent, Vector3 targetLocation);
}

// Should Instruction become a MonoBehaviour ?
public abstract class Instruction
{
    protected HaveInstructions ownerEntity;

    public abstract void OnStart();
    public abstract void OnEnd();
    public abstract void OnStop();
    public abstract void OnUpdate(); // ??? (should not be used, coroutines ran from OnStart() are better)
    public Coroutine StartCoroutine(IEnumerator e)
    {
        return ownerEntity.StartCoroutine(e);
    }

    // Returns if the entity can do the action or not depending on itself (e.g. a building can't move since it doesn't have the MovementComponent)
    public abstract bool CanDoInstruction();
}
