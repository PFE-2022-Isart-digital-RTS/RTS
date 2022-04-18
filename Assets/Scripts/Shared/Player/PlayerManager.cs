using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : SingletonNetworkBehaviour<PlayerManager>
{
    private readonly NetworkVariable<int> m_playerCount = new NetworkVariable<int>();
    [SerializeReference] ScriptableObject moveToInstructionGenerator;

    public int PlayerCount => m_playerCount.Value;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += id =>
        {
            if (IsServer)
            {
                m_playerCount.Value++;
                Debug.Log($"Client {id} connected");
            }
        };
        
        NetworkManager.Singleton.OnClientDisconnectCallback += id =>
        {
            if (IsServer)
            {
                m_playerCount.Value++;
                Debug.Log($"Client {id} disconnected");
            }
        };
    }

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
                Instruction moveInstruction = ((IMoveInstructionGenerator) moveToInstructionGenerator).GenerateInstruction(moveComponent, pos);
                moveComponent.moveInstruction = moveInstruction;
            }
        }
    }

    //[ServerRpc]
    //public void ShootTargetServerRpc(NetworkBehaviourReference health, int damage)
    //{
    //    if (health.TryGet(out Health healthComponent))
    //    {
    //        healthComponent.Health.Value -= damage;
    //    }
    //}
}
