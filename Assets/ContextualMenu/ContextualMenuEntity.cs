using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextualMenuEntity", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ContextualMenuEntity : ContextualMenuItem
{
    [SerializeField] GameObject entityToSpawnPrefab;

    protected override void OnPurchase(HaveOptionsComponent purchasedFrom)
    {
        // TODO : Spawn units around building
        GameObject go = Instantiate(entityToSpawnPrefab, purchasedFrom.transform.position + new Vector3(5, 0, 0), Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn(); 
        TeamComponent teamComp = go.GetComponent<TeamComponent>();
        teamComp.Team = purchasedFrom.GetComponent<TeamComponent>().Team;
    }
}
