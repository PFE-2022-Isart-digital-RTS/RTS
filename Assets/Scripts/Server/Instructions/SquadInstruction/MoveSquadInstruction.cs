using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SquadInstructionWithMove : SquadInstruction
{
    public MoveToEntitySquadInstruction moveSquadInstruction;

    public void SetTarget(Vector3 target)
    {
        if (moveSquadInstruction == null)
        {
            moveSquadInstruction = new MoveToEntitySquadInstruction
            {
                targetPosition = target,
                Next = this
            };

            moveSquadInstruction.OnPreviousTaskAdd(this);
        }
        else
        {
            moveSquadInstruction.targetPosition = target;
            moveSquadInstruction.goTarget = null;
        }
    }

    public void SetTarget(GameObject target)
    {
        if (moveSquadInstruction == null)
        {
            moveSquadInstruction = new MoveToEntitySquadInstruction
            {
                goTarget = target,
                targetPosition = target.transform.position,
                Next = this
            };

            moveSquadInstruction.OnPreviousTaskAdd(this);
        }
        else
        {
            moveSquadInstruction.targetPosition = target.transform.position;
            moveSquadInstruction.goTarget = target;
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        if (moveSquadInstruction != null)
            moveSquadInstruction.OnPreviousTaskRemove(this);
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

    public bool TryMoveTo(GameObject unit, Vector3 target)
    {
        if (!moveSquadInstruction.IsInRange(unit, target))
        {
            instructionManager.AssignInstruction(unit, moveSquadInstruction);
            return true;
        }
        return false;
    }

    public bool TryMoveTo(GameObject unit, GameObject target)
    {
        return TryMoveTo(unit, target.transform.position);
    }

    public bool TryMoveTo(GameObject unit)
    {
        return TryMoveTo(unit, moveSquadInstruction.targetPosition);
    }
}

public class MoveSquadInstruction : SquadInstruction
{
    public Vector3 targetPosition;
    public float stopDistSqr = 3 * 3;
    public float distMin = 5;

    protected override void OnStart()
    {
        base.OnStart();

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

    public bool IsInRange(GameObject unit, Vector3 target)
    {
        return (unit.transform.position - target).sqrMagnitude < stopDistSqr;
    }

    public bool IsInRange(GameObject unit)
    {
        return IsInRange(unit, targetPosition);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

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

public class MoveToEntitySquadInstruction : MoveSquadInstruction
{
    public GameObject goTarget;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (goTarget != null)
            targetPosition = goTarget.transform.position;
    }

    public bool IsInRange(GameObject unit, GameObject target)
    {
        return IsInRange(unit, target.transform.position);
    }
}