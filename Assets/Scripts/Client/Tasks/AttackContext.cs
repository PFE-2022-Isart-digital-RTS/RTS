using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//[CreateAssetMenu(fileName = "AttackContext", menuName = "ScriptableObjects/AttackContext", order = 1)]
//public class AttackContext : ContextualMenuItemBase
//{
//    NetworkBehaviourReference entityToAttackLifeComp;

//    public LifeComponent EntityToAttack
//    {
//        set
//        {
//            entityToAttackLifeComp = value;
//        }
//    }

//    private NetworkObjectReference[] m_targets;

//    public override void OnInvoked(List<HaveOptionsComponent> targets)
//    {
//        int length = targets.Count;
//        m_targets = new NetworkObjectReference[length];
//        for (int i = 0; i < length; i++)
//        {
//            m_targets[i] = targets[i].GetComponent<NetworkObject>();
//        }

//        //RTSPlayerController.LocalInstance.TryAttackServerRPC(m_targets, entityToAttackLifeComp);
//    }

//    public override ITask<HaveOptionsComponent> GetTask()
//    {
//        throw new NotImplementedException();
//    }
//}