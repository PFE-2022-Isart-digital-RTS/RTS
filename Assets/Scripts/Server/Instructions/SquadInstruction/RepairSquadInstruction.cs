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

    protected override Vector3 TargetPos { get; set; }

    protected override void OnStart()
    {
        TargetPos = inConstructionComp.transform.position;
        base.OnStart();
        inConstructionComp.onConstructionEnd.AddListener(OnConstructionEnd);
    }

    public override void OnStop()
    {
        base.OnStop();
        inConstructionComp.onConstructionEnd.RemoveListener(OnConstructionEnd);
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

        End();
        moveSquadInstruction.RunNextInstruction();
    }
}