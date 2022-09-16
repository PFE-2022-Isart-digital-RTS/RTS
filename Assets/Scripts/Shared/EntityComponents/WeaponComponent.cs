using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponComponent : MonoBehaviour
{
    public abstract void StartAttack(LifeComponent target);

    public abstract void StopAttack();

}
