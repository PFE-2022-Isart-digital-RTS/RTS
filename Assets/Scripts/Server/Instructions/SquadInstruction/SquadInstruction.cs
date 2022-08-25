using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class SquadInstruction : InstructionWithNext
{
    public Squad squad;

    public virtual void OnUnitStart(GameObject unit)
    {

    }

    public virtual void OnUnitStop(GameObject unit)
    {

    }

    public override void OnStart()
    {
        foreach (GameObject unit in squad.squadUnits)
        {
            OnUnitStart(unit);
        }
    }

    public override void OnStop()
    {
        foreach (GameObject unit in squad.squadUnits)
        {
            OnUnitStop(unit);
        }
    }
}