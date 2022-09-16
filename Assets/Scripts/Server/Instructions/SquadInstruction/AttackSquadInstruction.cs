using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AttackSquadInstruction : SquadInstruction
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

    public override bool CanDoInstruction()
    {
        return attackedComp != null;
    }

    public override void OnStart()
    {
        base.OnStart();
        attackedComp.OnNoLife.AddListener(OnTargetKilled);
    }

    public override void OnStop()
    {
        base.OnStop();
        attackedComp.OnNoLife.RemoveListener(OnTargetKilled);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        OnStop();
    }


    public override void OnUnitStart(GameObject unit)
    {
        WeaponComponent canAttackComp = unit.GetComponent<WeaponComponent>();
        if (canAttackComp != null)
        {
            canAttackComp.StartAttack(attackedComp);
            //attackedComp.OnConstructionHelpStart(canAttackComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a WeaponComponent");
        }
    }

    public override void OnUnitStop(GameObject unit)
    {
        WeaponComponent canAttackComp = unit.GetComponent<WeaponComponent>();
        if (canAttackComp != null)
        {
            canAttackComp.StopAttack();
            //attackedComp.OnConstructionHelpStop(canConstructComp);
        }
        else
        {
            Debug.LogWarning("Unit should possess a WeaponComponent");
        }
    }

    public void OnTargetKilled()
    {
        attackedComp.OnNoLife.RemoveListener(OnTargetKilled);

        if (next == null)
        {
            // TODO : search for nearby entities to attack,
            // and assign a new AttackSquadInstruction if there is
        }

        RunNextInstruction();
    }
}