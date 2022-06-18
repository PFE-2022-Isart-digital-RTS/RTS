using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class TeamComponent : NetworkBehaviour
{
    public TeamState team;

    [HideInInspector]
    public TeamState Team
    {
        set
        {
            if (team == value)
                return;

            if (team != null)
                team.UnregisterUnit(this);

            team = value;

            if (team != null)
                team.RegisterUnit(this);
        }
        get => team;
    }

    [ClientRpc]
    public void SetTeam_ClientRpc(NetworkBehaviourReference newTeamRef)
    {
        if (newTeamRef.TryGet(out TeamState newTeam))
        {
            Team = newTeam;
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TeamComponent))]
public class EntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TeamComponent teamComponent = (TeamComponent)target;

        // Get value before change
        TeamState previousValue = teamComponent.Team;

        // Make all the public and serialized fields visible in Inspector
        base.OnInspectorGUI();

        // Load changed values
        serializedObject.Update();

        TeamState newValue = ((TeamComponent)serializedObject.targetObject).Team;

        // Check if value has changed
        if (Application.isPlaying && previousValue != newValue)
        {
            if (previousValue != null)
                previousValue.UnregisterUnit(teamComponent);

            if (newValue != null)
                newValue.RegisterUnit(teamComponent);

            teamComponent.SetTeam_ClientRpc(newValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif