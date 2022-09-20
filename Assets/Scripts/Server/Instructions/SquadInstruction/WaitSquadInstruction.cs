using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSquadInstruction : SquadInstruction
{
    public override void OnPreviousTaskRemove(SquadInstruction instructionToRemove)
    {
        base.OnPreviousTaskRemove(instructionToRemove);

        if (PreviousTasksCount == 0)
            TryEnd();
        
    }
}
