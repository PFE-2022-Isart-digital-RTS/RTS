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
    protected override void OnStart()
    {
        base.OnStart();
        SetTarget(attackedComp.gameObject);
        instructionManager.AddUpdate(moveSquadInstruction); // The position has to be updated even when no units has joined the task for IsInRange()
        attackedComp.OnNoLife.AddListener(OnTargetKilled);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        attackedComp.OnNoLife.RemoveListener(OnTargetKilled);
        if (moveSquadInstruction != null)
            moveSquadInstruction.TryEnd();
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

    public override void OnUpdate()
    {
        base.OnUpdate();

        List<GameObject> unitsToUpdate = new List<GameObject>(units);
        foreach (GameObject unit in unitsToUpdate)
        {
            TryMoveTo(unit, attackedComp.gameObject);
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

        if (moveSquadInstruction != null)
            moveSquadInstruction.TryEnd();

        TryEnd();
    }
}