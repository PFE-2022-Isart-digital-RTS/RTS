using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanAttack
{
    void Attack(ICanBeAttacked target);
}


public class AttackComponent : MonoBehaviour, ICanAttack
{
    public float nbDamages;

    public void Attack(ICanBeAttacked target)
    {
        target.OnAttacked(this);
    }
}
