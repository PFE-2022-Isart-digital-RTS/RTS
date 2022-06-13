using System.Collections;
using System.Collections.Generic;
using ContextualMenuPackage;
using UnitSelectionPackage;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(HaveInstructions), typeof(MoveComponent))]
public class Entity : NetworkBehaviour, ISelectable, IContextualizable
{
    private bool m_isSelected = false;
    private Material m_material;
    //public ETeam team;

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


    private Color m_baseColor;

    public List<string> actions;

    //private bool isMoving;
    //private Vector3 positionToReach;
    //public float speed = 2f;
    //public float distanceToReach = 0.1f;

    [HideInInspector] public HaveInstructions haveInstructionsComponent;
    [HideInInspector] public MoveComponent moveComponent;

    #region MonoBehaviour
    protected void Awake()
    {
        haveInstructionsComponent = GetComponent<HaveInstructions>();
        moveComponent = GetComponent<MoveComponent>();
        m_material = GetComponentInChildren<Renderer>().material;
        m_baseColor = m_material.color;
    }

    //IEnumerator reg()
    //{
    //    yield return null;
    //    team.RegisterUnit(this);
    //}

    //private void OnEnable()
    //{
    //    StartCoroutine(reg());
    //    //team.RegisterUnit(this);
    //    //SharedGameManager.Instance.onRegisterEntity(team, this);
    //}

    //private void OnDisable()
    //{
    //    team.UnregisterUnit(this);

    //    //if (gameObject.scene.isLoaded)
    //    //    SharedGameManager.Instance.onUnregisterEntity(team, this);
    //}

    //private void FixedUpdate()
    //{
    //    if (isMoving)
    //    {
    //        Transform selfTransform = transform;
    //        Vector3 position = selfTransform.position;
    //        Vector3 posToTarget = positionToReach - position;
    //        float posToTargetDistance = posToTarget.magnitude;
    //        Vector3 direction = posToTarget / posToTargetDistance;
    //        position += speed * Time.fixedDeltaTime * direction;
    //        selfTransform.position = position;

    //        isMoving = posToTargetDistance > distanceToReach;
    //    }
    //}
    #endregion

    public void SetSelected(bool selected)
    {
        m_isSelected = selected;
        m_material.color = m_isSelected ? Color.yellow : m_baseColor;
    }
    
    public bool IsSelected()
    {
        return m_isSelected;
    }

    public List<string> GetTasks()
    {
        return actions;
    }

    //public void StopMovement()
    //{
    //    moveComponent.Stop();
    //}

    //public void MoveTo(Vector3 target)
    //{
    //    moveComponent.MoveTo(target);
    //}

    //// TODO : Set ownership ?
    //// However, if two players are in the same team and they can move each other's units,
    //// then ownership should be false and team should be checked
    //[ServerRpc(RequireOwnership = false)]
    //public void TryMoveToServerRPC(Vector3 pos, ServerRpcParams serverRpcParams = default)
    //{
    //    // TODO : verify if the call is correct, depending on the client id calling this function
    //    //ulong playerID = serverRpcParams.Receive.SenderClientId;

    //    MoveTo(pos);
    //}

    [ClientRpc]
    public void SetTeam_ClientRpc(NetworkBehaviourReference newTeamRef)
    {
        if (newTeamRef.TryGet(out TeamState newTeam))
        {
            Team = newTeam;
        }
    }
}




[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Entity entity = (Entity) target;

        // Get value before change
        TeamState previousValue = entity.Team;

        // Make all the public and serialized fields visible in Inspector
        base.OnInspectorGUI();

        // Load changed values
        serializedObject.Update();

        TeamState newValue = ((Entity)serializedObject.targetObject).Team;

        // Check if value has changed
        if (Application.isPlaying && previousValue != newValue)
        {
            if (previousValue != null)
                previousValue.UnregisterUnit(entity);

            if (newValue != null)
                newValue.RegisterUnit(entity);

            entity.SetTeam_ClientRpc(newValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}