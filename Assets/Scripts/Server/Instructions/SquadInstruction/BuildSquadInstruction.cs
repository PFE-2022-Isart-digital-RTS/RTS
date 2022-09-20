using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

class BuildSquadInstruction : SquadInstructionWithMove
{
    public Vector3 targetPosition;
    public GameObject buildingPrefab;
    public TeamState team;

    protected override Vector3 TargetPos { get; set; }

    public GameObject SpawnBuilding()
    {
        GameObject go = GameObject.Instantiate(buildingPrefab, targetPosition, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<TeamComponent>().Team = team;

        return go;
    }

    protected override void OnStart()
    {
        TargetPos = targetPosition;

        base.OnStart();
    }

    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);

        GameObject go = SpawnBuilding();

        RepairSquadInstruction repairSquadInstruction;
        repairSquadInstruction = new RepairSquadInstruction { inConstructionComp = go.GetComponent<CanBeRepairedComponent>(), Next = Next };
        Next = repairSquadInstruction;

        moveSquadInstruction.Next = Next;
        repairSquadInstruction.moveSquadInstruction = moveSquadInstruction;

        TryEnd();
    }
}