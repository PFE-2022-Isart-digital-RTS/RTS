using ContextualMenuPackage;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextualMenuEntity", menuName = "ScriptableObjects/ContextualMenuEntity", order = 1)]
public class ContextualMenuEntity : ContextualMenuItem
{
    [SerializeField] GameObject entityToSpawnPrefab;

    public override ContextualMenuItemBase.InstructionGenerator GetInstructionGenerator()
    {
        return new InstructionGenerator() { Data = this };
    }

    // Server Side
    public new class InstructionGenerator : ContextualMenuItem.InstructionGenerator
    {
        public new ContextualMenuEntity Data
        {
            get => (ContextualMenuEntity)data;
            set => data = value;
        }

        public override void OnPurchaseStart(List<HaveOptionsComponent> purchasedFromList)
        {
            HaveOptionsComponent purchasedFrom = purchasedFromList[0]; // TODO : function evaluating from which building the entity should be spawned from
            OnPurchaseStart(purchasedFrom);
        }

        public override void OnPurchaseStart(HaveOptionsComponent purchasedFrom)
        {
            ItemQueueComponent itemQueue = purchasedFrom.GetComponent<ItemQueueComponent>();
            if (itemQueue == null)
            {
                Debug.LogError("Entity should have a ItemQueueComponent.");
                return;
            }

            itemQueue.AddItem(this);
        }


        public override void OnPurchaseEnd(HaveOptionsComponent purchasedFrom)
        {
            // TODO : Spawn units around building
            GameObject go = Instantiate(Data.entityToSpawnPrefab, purchasedFrom.transform.position + new Vector3(5, 0, 0), Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
            TeamComponent teamComp = go.GetComponent<TeamComponent>();
            teamComp.Team = purchasedFrom.GetComponent<TeamComponent>().Team;
        }
    }
}
