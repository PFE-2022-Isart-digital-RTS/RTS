using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeComponent))]
public class MonsterComponent : MonoBehaviour
{
    LifeComponent life;

    [SerializeField]
    ContextualMenuItemBase itemToAdd;

    private void Awake()
    {
        life = GetComponent<LifeComponent>();
        life.OnKilled.AddListener(OnDeath);
    }

    void OnDeath(WeaponComponent killer, float nbDamages)
    {
        TeamComponent teamComp = killer.GetComponent<TeamComponent>();
        if (teamComp.Team is TeamState team)
        {
            team.AddItemFromMonster(itemToAdd);
        }
    }
}
