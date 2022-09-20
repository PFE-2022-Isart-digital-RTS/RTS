using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AttackSquadInstruction : SquadInstructionWithMove
{
    public LifeComponent attackedComp;

    public GameObject InConstructionGO
    {
        set
        {
            attackedComp = value.GetComponent<LifeComponent>();
            if (attackedComp == null)
            {
                Debug.LogWarning("Can't attack an entity that doesn't have a LifeComponent");
            }
        }
    }

    protected override Vector3 TargetPos { get; set; }

    protected override void OnStart()
    {
        TargetPos = attackedComp.transform.position;
        base.OnStart();
        attackedComp.OnNoLife.AddListener(OnTargetKilled);
    }

    public override void OnStop()
    {
        base.OnStop();
        attackedComp.OnNoLife.RemoveListener(OnTargetKilled);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        OnStop();
    }


    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);
        WeaponComponent canAttackComp = unit.GetComponent<WeaponComponent>();
        if (canAttackComp != null)
        {
            canAttackComp.StartAttack(attackedComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a WeaponComponent");
        }
    }

    protected override void OnUnitStop(GameObject unit)
    {
        base.OnUnitStop(unit);
        WeaponComponent canAttackComp = unit.GetComponent<WeaponComponent>();
        if (canAttackComp != null)
        {
            canAttackComp.StopAttack();
        }
        else
        {
            Debug.LogWarning("Unit should possess a WeaponComponent");
        }
    }

    public void OnTargetKilled()
    {
        attackedComp.OnNoLife.RemoveListener(OnTargetKilled);

        if (Next == null)
        {
            // TODO : search for nearby entities to attack,
            // and assign a new AttackSquadInstruction if there is
        }

        RunNextInstruction();
    }
}