using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RepairSquadInstruction = SubscribeSquadInstruction<CanBeRepairedComponent, CanRepairComponent>;
using WagonSubscriptionInstruction = SubscribeSquadInstruction<WagonSubscriptionComponent, CanSubToWagonComponent>;

class SubscribeSquadInstruction<SUBSCRIBE_COMP, SUBSCRIBER> : SquadInstructionWithMove 
    where SUBSCRIBE_COMP : CanBeSubscribedToComponent 
    where SUBSCRIBER : CanSubscribeComponent
{
    public SUBSCRIBE_COMP inConstructionComp;

    public GameObject InConstructionGO
    {
        set
        {
            inConstructionComp = value.GetComponent<SUBSCRIBE_COMP>();
            if (inConstructionComp == null)
            {
                Debug.LogWarning("Can't construct an entity that doesn't have a SUBSCRIBE_COMP");
            }
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        SetTarget(inConstructionComp.gameObject);
        inConstructionComp.onEnd.AddListener(OnConstructionEnd);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        inConstructionComp.onEnd.RemoveListener(OnConstructionEnd);
    }

    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);

        SUBSCRIBER canConstructComp = unit.GetComponent<SUBSCRIBER>();
        if (canConstructComp != null)
        {
            inConstructionComp.Subscribe(canConstructComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a SUBSCRIBER");
        }
    }

    protected override void OnUnitStop(GameObject unit)
    {
        SUBSCRIBER canConstructComp = unit.GetComponent<SUBSCRIBER>();
        if (canConstructComp != null)
        {
            inConstructionComp.Unsubscribe(canConstructComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a SUBSCRIBER");
        }

        base.OnUnitStop(unit);
    }

    public void OnConstructionEnd()
    {
        inConstructionComp.onEnd.RemoveListener(OnConstructionEnd);

        TryEnd();
        moveSquadInstruction.RunNextInstruction();
    }
}
