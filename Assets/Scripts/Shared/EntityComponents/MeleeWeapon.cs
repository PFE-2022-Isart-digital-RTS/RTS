using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponComponent
{
    public float attackDamages = 1f;
    public float attackSpeed = 0.4f; // nbAttacks per second
    public float attackHitRatio = 0.5f; // the enemy will receive damages at attackSpeed * attackHitRatio seconds after the start

    Coroutine attackCoroutine;
    LifeComponent target;

    public override void StartAttack(LifeComponent target)
    {
        StopAttack();

        this.target = target;
        if (target != null)
        {
            target.OnNoLife.AddListener(StopAttack);
            attackCoroutine = StartCoroutine(Attack(target));
        }
    }

    public IEnumerator Attack(LifeComponent target)
    {
        yield return new WaitForSeconds(attackHitRatio * attackSpeed);

        target.DealMeleeDamages(this, attackDamages);

        yield return new WaitForSeconds((1f - attackHitRatio) * attackSpeed);

        attackCoroutine = StartCoroutine(Attack(target));
        yield break;
    }

    public override void StopAttack()
    {
        if (target != null)
            target.OnNoLife.RemoveListener(StopAttack);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }

}
