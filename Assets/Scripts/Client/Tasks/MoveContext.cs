using System.Collections.Generic;
using ContextualMenuPackage;
using Shared.Task;
using Unity.Netcode;
using UnityEngine;

public class MoveContext : ITask<Entity>
{
    private List<Entity> m_targets; // ref

    public void OnInvoked(List<Entity> targets)
    {
        m_targets = targets;
        ClientGameManager.Instance.RequestPosition += OnPositionIndicate;
    }

    void OnPositionIndicate(Vector3 position)
    {
        Vector3 OffsetFromStart;
        int unitCount = m_targets.Count;
        float unitCountSqrt = Mathf.Sqrt(unitCount);
        int NumberOfCharactersRow = (int) unitCountSqrt;
        int NumberOfCharactersColumn = (int) unitCountSqrt + unitCount - NumberOfCharactersRow * NumberOfCharactersRow;
        float Distance = 1f;

        OffsetFromStart = new Vector3(NumberOfCharactersRow * Distance / 2f, 0f,
            NumberOfCharactersColumn * Distance / 2f);

        for (int i = 0; i < unitCount; i++)
        {
            int r = i / NumberOfCharactersRow;
            int c = i % NumberOfCharactersRow;
            Vector3 offset = new Vector3(r * Distance, 0f, c * Distance);
            SerializableVector3 pos = position + offset - OffsetFromStart;

            EntityPositionData taskData = new EntityPositionData
                {targetPos = pos, entityId = m_targets[i].NetworkObject.NetworkObjectId};

            NetworkGameData networkGameData = new NetworkGameData {header = EDataHeader.MoveTo, obj = taskData};

            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkDataExchanger>()
                .SendReceiveDataServerRPC(networkGameData);
        }
    }
}