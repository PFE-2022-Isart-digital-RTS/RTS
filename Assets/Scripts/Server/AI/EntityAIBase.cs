using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAIBase : MonoBehaviour
{
    protected TeamComponent team;
    protected LifeComponent lifeComp;

    protected InstructionsManager InstrManager { get => RTSGameMode.Instance.instructionsManager; }

    #region MonoBehaviour
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

    #endregion

    #region Callbacks
    protected virtual void OnAttacked(WeaponComponent attacker, float nbDamages) { }
    #endregion

    #region Utility
    protected void InsertInstruction(SquadInstruction newInstruction)
    {
        InstrManager.InsertInstruction(gameObject, newInstruction);
    }

    // Inserts an instruction for every unit of the same team around that unit
    protected void InsertInstruction(SquadInstruction newInstruction, float radius)
    {
        ICollection<GameObject> units = team.Team.GetUnitsInRange(transform.position, radius);

        InstrManager.InsertInstruction(units, newInstruction);
    }

    protected SquadInstruction GetCurrentInstruction()
    {
        return InstrManager.GetInstruction(gameObject);
    }

    protected bool IsIdle()
    {
        return GetCurrentInstruction() == null;
    }

    #region TryAttack
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
    #endregion

    #endregion
}
