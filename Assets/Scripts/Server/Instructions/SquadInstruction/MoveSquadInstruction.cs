using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SquadInstructionWithMove : SquadInstruction
{
    public MoveSquadInstruction moveSquadInstruction;

    protected abstract Vector3 TargetPos { get; set; }

    protected override void OnStart()
    {
        base.OnStart();

        if (moveSquadInstruction == null)
        {
            moveSquadInstruction = new MoveSquadInstruction
            {
                targetPosition = TargetPos,
                Next = this
            };
        }
    }

    public override void UnitStart(GameObject unit)
    {
        units.Add(unit);

        if (HasEnded)
        {
            RunNextInstruction(unit);
            return;
        }

        TryStart();

        if (TryMoveTo(unit))
            return;

        OnUnitStart(unit);            
    }

    public bool TryMoveTo(GameObject unit)
    {
        if (!moveSquadInstruction.IsInRange(unit))
        {
            instructionManager.AssignInstruction(unit, moveSquadInstruction);
            return true;
        }
        return false;
    }
}

public class MoveSquadInstruction : SquadInstruction
{
    public Vector3 targetPosition;
    public float stopDistSqr = 3 * 3;
    public float distMin = 5;

    protected override void OnStart()
    {
        UpdatePositions();
    }

    protected override void OnUnitStart(GameObject unit)
    {
        base.OnUnitStart(unit);

        MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
        Instruction moveInstruction = new MoveInstruction { moveComponent = moveComponent, targetLocation = unit.transform.position };
        // to modify, the instruction should be added to the HaveInstruction component
        moveComponent.moveInstruction = moveInstruction;
        UpdatePositions();
    }

    protected override void OnUnitStop(GameObject unit)
    {
        MoveComponent moveComponent = unit.GetComponent<MoveComponent>();

        moveComponent.Stop();

        // to modify, the instruction should be added to the HaveInstruction component
        moveComponent.moveInstruction = null;
        
        base.OnUnitStop(unit);

        UpdatePositions();
    }

    void UpdatePositions()
    {
        Vector3 OffsetFromStart;
        int unitCount = units.Count;
        float unitCountSqrt = Mathf.Sqrt(unitCount);
        int NumberOfCharactersRow = (int)unitCountSqrt;
        int NumberOfCharactersColumn = (int)unitCountSqrt + unitCount - NumberOfCharactersRow * NumberOfCharactersRow;
        float Distance = 1f;

        OffsetFromStart = new Vector3(NumberOfCharactersRow * Distance / 2f, 0f,
            NumberOfCharactersColumn * Distance / 2f);

        for (int i = 0; i < unitCount; i++)
        {
            //int r = i / NumberOfCharactersRow;
            //int c = i % NumberOfCharactersRow;
            //Vector3 offset = new Vector3(r * Distance, 0f, c * Distance);
            //Vector3 pos = targetPosition + offset - OffsetFromStart;
            GameObject unit = units[i];
            Vector3 finalLoc = unit.transform.position + (targetPosition - unit.transform.position).normalized * 5;

            //Vector3 offset = Vector3.zero;
            foreach (GameObject otherUnit in units)  
            {
                if (unit == otherUnit)
                    continue;

                float dist = Vector3.Distance(otherUnit.transform.position, unit.transform.position);
                if (dist < distMin)
                {
                    Vector3 n = (otherUnit.transform.position - unit.transform.position).normalized;
                    if (n.Equals(Vector3.zero))
                    {
                        Vector2 rand = Random.insideUnitCircle;
                        n.x = rand.x;
                        n.z = rand.y;
                    }
                    n.y = 0;
                    finalLoc -= n * (distMin - dist);
                }


            }

            MoveInstruction moveInstruction = (MoveInstruction) unit.GetComponent<MoveComponent>().moveInstruction;
            if (moveInstruction != null)
                moveInstruction.targetLocation = finalLoc; 
        }
    }

    public Vector3 GetCenter()
    {
        Vector3 center = new Vector3();
        foreach (GameObject moveComp in units)
        {
            center += moveComp.transform.position;
        }
        return center / units.Count;
    }

    public bool IsInRange(GameObject unit)
    {
        return (unit.transform.position - targetPosition).sqrMagnitude < stopDistSqr;
    }

    public override void OnUpdate()
    {
        UpdatePositions();

        List<GameObject> unitsCopy = new List<GameObject>(units);
        foreach (GameObject unit in unitsCopy)
        { 
            if (IsInRange(unit))
            {
                RunNextInstruction(unit); 
            }
        }
    }
}