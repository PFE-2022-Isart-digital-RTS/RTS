using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RepairSquadInstruction : SquadInstructionWithMove
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

    protected override void OnStart()
    {
        base.OnStart();
        SetTarget(inConstructionComp.gameObject);
        inConstructionComp.onConstructionEnd.AddListener(OnConstructionEnd);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);
    }

    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);

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

    protected override void OnUnitStop(GameObject unit)
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

        base.OnUnitStop(unit);
    }

    public void OnConstructionEnd()
    {
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);

        TryEnd();
        moveSquadInstruction.RunNextInstruction();
    }
}