using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MoveInstructionGenerator : ScriptableObject, IMoveInstructionGenerator
{
    public float distanceToReach = 0.1f;

    public Instruction GenerateInstruction(MoveComponent moveComponent, Vector3 targetLocation)
    {
        return new MoveInstruction { moveComponent = moveComponent, targetLocation = targetLocation, settings = this };
    }

    public class MoveInstruction : Instruction
    {
        public MoveInstructionGenerator settings;
        public MoveComponent moveComponent;
        public Vector3 targetLocation;

        public override bool CanDoInstruction()
        {
            return moveComponent != null;
        }

        public override void OnEnd()
        {

        }

        public override void OnStart()
        {
            // TODO : Use moveComponent.MoveTo to use the navmesh movement instead of OnUpdate()
            //moveComponent.MoveTo(targetLocation);
        }

        public override void OnStop()
        {

        }

        public override void OnUpdate()
        {
            Transform selfTransform = moveComponent.transform;
            Vector3 position = selfTransform.position;
            Vector3 posToTarget = targetLocation - position;
            float posToTargetDistance = posToTarget.magnitude;
            Vector3 direction = posToTarget / posToTargetDistance;
            position += moveComponent.speed * Time.fixedDeltaTime * direction;
            selfTransform.position = position;

            bool isMoving = posToTargetDistance > settings.distanceToReach;
            if (!isMoving)
                moveComponent.Stop();
        }
    }

}
