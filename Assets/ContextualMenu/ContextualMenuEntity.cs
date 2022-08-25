using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextualMenuEntity", menuName = "ScriptableObjects/ContextualMenuEntity", order = 1)]
public class ContextualMenuEntity : ContextualMenuItem
{
    [SerializeField] GameObject entityToSpawnPrefab;

    protected override void OnPurchase(List<HaveOptionsComponent> purchasedFrom)
    {
        HaveOptionsComponent buildBuyingFrom = purchasedFrom[0]; // TODO : function evaluating from which building the entity should be spawned from

        // TODO : Spawn units around building
        GameObject go = Instantiate(entityToSpawnPrefab, buildBuyingFrom.transform.position + new Vector3(5, 0, 0), Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn(); 
        TeamComponent teamComp = go.GetComponent<TeamComponent>();
        teamComp.Team = buildBuyingFrom.GetComponent<TeamComponent>().Team;
    }
}
