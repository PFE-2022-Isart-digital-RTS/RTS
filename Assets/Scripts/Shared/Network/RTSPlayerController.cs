using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// TODO : Link each PlayerController to the PlayerState through network?

// Local to each client, responsible of the ui and the selection, 
// and sends the inputs to the server.
public class RTSPlayerController : PlayerController
{
    public new RTSPlayerState PlayerState { get => (RTSPlayerState) base.PlayerState; set => base.PlayerState = value; }
    public static new RTSPlayerController LocalInstance { get => (RTSPlayerController)PlayerController.LocalInstance; }

    // TODO : Set ownership ?
    // However, if two players are in the same team and they can move each other's units,
    // then ownership should be false and team should be checked
    [ServerRpc(RequireOwnership = false)]
    public void TryMoveToServerRPC(NetworkBehaviourReference[] moveComponents, Vector3 targetPosition, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        Vector3 OffsetFromStart;
        int unitCount = moveComponents.Length;
        float unitCountSqrt = Mathf.Sqrt(unitCount);
        int NumberOfCharactersRow = (int)unitCountSqrt;
        int NumberOfCharactersColumn = (int)unitCountSqrt + unitCount - NumberOfCharactersRow * NumberOfCharactersRow;
        float Distance = 1f;

        OffsetFromStart = new Vector3(NumberOfCharactersRow * Distance / 2f, 0f,
            NumberOfCharactersColumn * Distance / 2f);

        for (int i = 0; i < unitCount; i++)
        {
            if (moveComponents[i].TryGet(out MoveComponent moveComponent))
            {
                int r = i / NumberOfCharactersRow;
                int c = i % NumberOfCharactersRow;
                Vector3 offset = new Vector3(r * Distance, 0f, c * Distance);
                Vector3 pos = targetPosition + offset - OffsetFromStart;
                //entity.MoveTo(pos);
                Instruction moveInstruction = new MoveInstruction { moveComponent = moveComponent, targetLocation = pos };
                // to modify, the instruction should be added to the HaveInstruction component
                moveComponent.moveInstruction = moveInstruction;
            }
        }
    }

    // List<ICanBeSelected> selectedEntities;

    // TODO : UI


    //void OnEntitySpawned(Entity newEntity)
    //{
    //    var m = newEntity.GetComponent<LifeComponent>();
    //    if (m != null)
    //    {
    //        HealthBar health = Instantiate(HealthBar3D, m.transform);
    //        health.life = m.lifeRatio;
    //    }
    //}
}
