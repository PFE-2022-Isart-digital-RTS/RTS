using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<GameObject> squadUnits;
    InstructionQueue instructionQueue = new InstructionQueue();

    // TODO : use TeamComponent?
    [SerializeField, EditInPlayModeOnly]
    TeamState team;

    [HideInInspector]
    public TeamState Team
    {
        set
        {
            if (team == value)
                return;

            if (team != null)
                team.UnregisterSquad(this);

            team = value;

            if (team != null)
                team.RegisterSquad(this);
        }
        get => team;
    }

    public void RemoveUnit(GameObject gameUnit)
    {
        SquadInstruction currentInstruction = (SquadInstruction)instructionQueue.CurrentInstruction;
        if (currentInstruction != null)
        {
            currentInstruction.OnUnitStop(gameUnit);
        }

        squadUnits.Remove(gameUnit); 

        if (squadUnits.Count == 0)
        {
            onSquadDestroyed();
        }
    }

    private void onSquadDestroyed()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        instructionQueue.UpdateCurrentInstruction();
    }

    public Vector3 GetCenter()
    {
        Vector3 center = new Vector3();
        foreach (GameObject moveComp in squadUnits)
        {
            center += moveComp.transform.position;
        }
        return center / squadUnits.Count;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        instructionQueue.AssignNewInstruction(new MoveSquadInstruction { squad = this, targetPosition = targetPosition } );
    }

    public void AddMoveTo(Vector3 targetPosition)
    {
        instructionQueue.AddInstruction(new MoveSquadInstruction { squad = this, targetPosition = targetPosition });
    }

    public void AddBuild(Vector3 targetPosition, GameObject buildingPrefab)
    {
        instructionQueue.AddInstruction(new BuildSquadInstruction { squad = this, targetPosition = targetPosition, buildingPrefab = buildingPrefab });
    }

    public void AddRepair(CanBeRepairedComponent canBeRepaired)
    {
        instructionQueue.AddInstruction(new RepairSquadInstruction { squad = this, inConstructionComp = canBeRepaired });
    }

    public void AddAttack(LifeComponent lifeComponent)
    {
        instructionQueue.AddInstruction(new AttackSquadInstruction { squad = this, attackedComp = lifeComponent });
    }
}
