using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RepairSquadInstruction : SquadInstruction
{
    public CanBeRepairedComponent inConstructionComp;

    public GameObject InConstructionGO
    {
        set
        {
            inConstructionComp = value.GetComponent<CanBeRepairedComponent>();
            if (inConstructionComp == null)
            {
                Debug.LogWarning("Can't construct an entity that doesn't have a CanBeRepairedComponent");
            }
        }
    }

    public override bool CanDoInstruction()
    {
        return inConstructionComp != null;
    }

    public override void OnStart()
    {
        base.OnStart();
        inConstructionComp.onConstructionEnd.AddListener(OnConstructionEnd);
    }

    public override void OnStop()
    {
        base.OnStop();
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);
    }

    public override void OnUnitStart(GameObject unit)
    {
        CanRepairComponent canConstructComp = unit.GetComponent<CanRepairComponent>();
        if (canConstructComp != null)
        {
            inConstructionComp.OnConstructionHelpStart(canConstructComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a CanRepairComponent");
        }
    }

    public override void OnUnitStop(GameObject unit)
    {
        CanRepairComponent canConstructComp = unit.GetComponent<CanRepairComponent>();
        if (canConstructComp != null)
        {
            inConstructionComp.OnConstructionHelpStop(canConstructComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a CanRepairComponent");
        }
    }

    public void OnConstructionEnd()
    {
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);
        RunNextInstruction();
    }
}