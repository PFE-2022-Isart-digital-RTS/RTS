using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveComponent : NetworkBehaviour
{
    public float speed = 2f;

    public Instruction moveInstruction = null;

    public void MoveTo(Vector3 targetLocation)
    {
        // TODO : use navmesh
    }

    public void Stop()
    {
        //isMoving = false;
        if (moveInstruction != null)
            moveInstruction.OnStop();

        moveInstruction = null;
    }

    private void FixedUpdate()
    {
        if (moveInstruction != null)
            moveInstruction.OnUpdate();

        //if (isMoving)
        //{
        //    Transform selfTransform = transform;
        //    Vector3 position = selfTransform.position;
        //    Vector3 posToTarget = positionToReach - position;
        //    float posToTargetDistance = posToTarget.magnitude;
        //    Vector3 direction = posToTarget / posToTargetDistance;
        //    position += speed * Time.fixedDeltaTime * direction;
        //    selfTransform.position = position;

            //    isMoving = posToTargetDistance > distanceToReach;
            //}
    }
}
