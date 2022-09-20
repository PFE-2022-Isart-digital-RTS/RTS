using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsManager
{
    Dictionary<GameObject, SquadInstruction> unitsToInstr = new Dictionary<GameObject, SquadInstruction>();
    HashSet<SquadInstruction> instructionsToUpdate = new HashSet<SquadInstruction>();

    public void Update()
    {
        HashSet<SquadInstruction> instructionsCurrentlyUpdated = new HashSet<SquadInstruction>(instructionsToUpdate);
        foreach (SquadInstruction instr in instructionsCurrentlyUpdated)
        {
            instr.OnUpdate();
        }
    }

    public void AddUpdate(SquadInstruction instr)
    {
        instructionsToUpdate.Add(instr);
    }

    public void RemoveUpdate(SquadInstruction instr)
    {
        instructionsToUpdate.Remove(instr);
    }

    public void AssignInstruction(IEnumerable<GameObject> units, SquadInstruction newInstruction)
    {
        foreach (GameObject unit in units)
        {
            AssignInstruction(unit, newInstruction);
        }
    }

    public void AssignInstruction(GameObject unit, SquadInstruction newInstruction)
    {
        if (unitsToInstr.TryGetValue(unit, out SquadInstruction oldInstr) && oldInstr != null)
            oldInstr.UnitStop(unit);

        unitsToInstr[unit] = newInstruction;

        if (newInstruction != null)
        {
            newInstruction.instructionManager = this;
            AddUpdate(newInstruction);
            newInstruction.UnitStart(unit);
        }
    }

    public void InsertInstruction(IEnumerable<GameObject> units, SquadInstruction newInstruction)
    {
        foreach (GameObject unit in units)
        {
            InsertInstruction(unit, newInstruction);
        }
    }

    public void InsertInstruction(GameObject unit, SquadInstruction newInstruction)
    {
        SquadInstruction oldInstr;
        if (unitsToInstr.TryGetValue(unit, out oldInstr) && oldInstr != null)
            oldInstr.UnitStop(unit);

        unitsToInstr[unit] = newInstruction;

        if (newInstruction != null)
        {
            newInstruction.Next = oldInstr;
            newInstruction.UnitStart(unit);
        }
    }
}
