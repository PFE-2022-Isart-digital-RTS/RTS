using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveSquadInstruction : SquadInstruction
{
    public Vector3 targetPosition;
    public float stopDistSqr = 3 * 3;

    public override bool CanDoInstruction()
    {
        return true;
    }

    public override void OnStart()
    {
        base.OnStart();
        UpdatePositions();
    }

    public override void OnUnitStart(GameObject unit)
    {
        MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
        Instruction moveInstruction = new MoveInstruction { moveComponent = moveComponent, targetLocation = unit.transform.position };
        // to modify, the instruction should be added to the HaveInstruction component
        moveComponent.moveInstruction = moveInstruction;
    }

    public override void OnUnitStop(GameObject unit)
    {
        MoveComponent moveComponent = unit.GetComponent<MoveComponent>();

        moveComponent.Stop();

        // to modify, the instruction should be added to the HaveInstruction component
        moveComponent.moveInstruction = null;
    }

    public override void OnEnd()
    {
        OnStop(); // TODO : not call this function?
    }

    void UpdatePositions()
    {
        Vector3 OffsetFromStart;
        int unitCount = squad.squadUnits.Count;
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
            GameObject unit = squad.squadUnits[i];
            Vector3 finalLoc = unit.transform.position + (targetPosition - unit.transform.position).normalized * 5;

            //Vector3 offset = Vector3.zero;
            foreach (GameObject otherUnit in squad.squadUnits)  
            {
                if (unit == otherUnit)
                    continue;

                float distMin = 3;
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
                    finalLoc -= n * (distMin - dist);
                }


            }

            MoveInstruction moveInstruction = (MoveInstruction) unit.GetComponent<MoveComponent>().moveInstruction;
            if (moveInstruction != null)
                moveInstruction.targetLocation = finalLoc; 
        }
    }

    public override void OnUpdate()
    {
        UpdatePositions();

        if ((squad.GetCenter() - targetPosition).sqrMagnitude < stopDistSqr)
        {
            RunNextInstruction();
        }
    }
}