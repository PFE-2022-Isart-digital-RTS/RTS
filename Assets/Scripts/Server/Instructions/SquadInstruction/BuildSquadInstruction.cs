using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

class BuildSquadInstruction : SquadInstruction
{
    public Vector3 targetPosition;
    public GameObject buildingPrefab;

    public override bool CanDoInstruction()
    {
        return true;
    }

    public override void OnStart()
    {
        GameObject go = GameObject.Instantiate(buildingPrefab, targetPosition, Quaternion.identity);

        go.GetComponent<NetworkObject>().Spawn();

        go.GetComponent<TeamComponent>().Team = squad.Team;

        next = new RepairSquadInstruction { squad = squad, inConstructionComp = go.GetComponent<CanBeRepairedComponent>(), next = next };

        RunNextInstruction();
    }
}