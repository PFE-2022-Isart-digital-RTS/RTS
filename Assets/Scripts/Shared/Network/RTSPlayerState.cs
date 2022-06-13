using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

// Contains server data that is player specific
public class RTSPlayerState : PlayerState
{
    NetworkVariable<NetworkBehaviourReference> teamRef = new NetworkVariable<NetworkBehaviourReference>();

    public TeamState Team
    {
        get
        {
            if (teamRef.Value.TryGet(out TeamState teamState))
            {
                return teamState;
            }

            Debug.LogError("Invalid team ref");
            return null;
        }
        set
        {
            teamRef.Value = value;
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(RTSPlayerState))]
public class RTSPlayerStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RTSPlayerState entity = (RTSPlayerState)target;

        // Make all the public and serialized fields visible in Inspector
        base.OnInspectorGUI();

        TeamState newTeam = (TeamState) EditorGUILayout.ObjectField("Team:", entity.Team, typeof(TeamState), true);
        if (newTeam != entity.Team)
        {
            entity.Team = newTeam;
        }

        // Load changed values
        serializedObject.Update();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif