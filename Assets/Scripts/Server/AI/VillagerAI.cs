using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerAI : EntityAIBase
{
    [SerializeField]
    float fightbackCallRadius = 15;

    protected override void OnAttacked(WeaponComponent attacker, float nbDamages)
    {
        if (IsIdle())
        {
            TryAttack(attacker, fightbackCallRadius);
        }
    }
}
