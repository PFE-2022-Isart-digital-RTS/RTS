using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectSquadInstruction : SquadInstruction
{
    Dictionary<GameObject, SquadInstruction> unitToNextInstruction = new Dictionary<GameObject, SquadInstruction>();

    public Dictionary<GameObject, SquadInstruction> UnitToNextInstruction
    {
        set
        {
            unitToNextInstruction = value;

            foreach (KeyValuePair<GameObject, SquadInstruction> pair in unitToNextInstruction)
            {
                if (pair.Value != null)
                    pair.Value.OnPreviousTaskAdd(this);
            }
        }
    }

    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);

        if (unitToNextInstruction.TryGetValue(unit, out SquadInstruction nextInstruction))
        {
            instructionManager.AssignInstruction(unit, nextInstruction);
            
        }
        else
        {
            // Default case :
            RunNextInstruction(unit);
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        foreach (KeyValuePair<GameObject, SquadInstruction> pair in unitToNextInstruction)
        {
            if (pair.Value != null)
                pair.Value.OnPreviousTaskRemove(this);
        }
    }
}
