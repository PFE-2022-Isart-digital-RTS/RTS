using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAIBase : MonoBehaviour
{
    protected TeamComponent team;
    protected LifeComponent lifeComp;

    private void Awake()
    {
        team = GetComponent<TeamComponent>();
        lifeComp = GetComponent<LifeComponent>();
    }

    private void OnEnable()
    {
        lifeComp.OnAttacked += OnAttacked;
    }

    private void OnDisable()
    {
        lifeComp.OnAttacked -= OnAttacked;
    }

    protected virtual void OnAttacked(WeaponComponent attacker, float nbDamages)
    {

    }

    protected void InsertInstruction(SquadInstruction newInstruction)
    {
        RTSGameMode.Instance.instructionsManager.InsertInstruction(gameObject, newInstruction);
    }

    // Inserts an instruction for every unit of the same team around that unit
    protected void InsertInstruction(SquadInstruction newInstruction, float radius)
    {
        ICollection<GameObject> units = team.Team.GetUnitsInRange(transform.position, radius);

        RTSGameMode.Instance.instructionsManager.InsertInstruction(units, newInstruction);
    }

    protected bool TryAttack(GameObject target)
    {
        LifeComponent attackerLifeComp = target.GetComponent<LifeComponent>();
        if (attackerLifeComp == null)
            return false;

        InsertInstruction(new AttackSquadInstruction()
        {
            attackedComp = attackerLifeComp,
        });
        return true;
    }

    protected bool TryAttack(GameObject target, float callRadius)
    {
        LifeComponent attackerLifeComp = target.GetComponent<LifeComponent>();
        if (attackerLifeComp == null)
            return false;

        InsertInstruction(new AttackSquadInstruction()
        {
            attackedComp = attackerLifeComp,
        }, callRadius);

        return true;
    }

    protected bool TryAttack(MonoBehaviour target, float callRadius)
    {
        return TryAttack(target.gameObject, callRadius);
    }

    protected bool TryAttack(MonoBehaviour target)
    {
        return TryAttack(target.gameObject);
    }
}
