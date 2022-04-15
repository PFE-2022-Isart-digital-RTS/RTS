using Unity.Netcode;
using UnityEngine;

namespace Shared.Task
{
    [System.Serializable]
    public class EntityPositionData
    {
        public ulong entityId;
        public SerializableVector3 targetPos;
    }
}