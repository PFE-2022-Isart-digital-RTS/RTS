using System.Collections;
using System.Collections.Generic;
using ContextualMenuPackage;
using UnitSelectionPackage;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(HaveInstructions), typeof(MoveComponent))]
public class HaveOptionsComponent : NetworkBehaviour, ISelectable, IContextualizable
{
    private bool m_isSelected = false;
    private Material m_material;

    private Color m_baseColor;

    public List<string> actions;

    //private bool isMoving;
    //private Vector3 positionToReach;
    //public float speed = 2f;
    //public float distanceToReach = 0.1f;

    [HideInInspector] public HaveInstructions haveInstructionsComponent;
    [HideInInspector] public MoveComponent moveComponent;

    public List<ContextualMenuItem> items = new List<ContextualMenuItem>();

    #region MonoBehaviour
    protected void Awake()
    {
        haveInstructionsComponent = GetComponent<HaveInstructions>();
        moveComponent = GetComponent<MoveComponent>();
        m_material = GetComponentInChildren<Renderer>().material;
        m_baseColor = m_material.color;

        foreach (ContextualMenuItem entityItem in items)
        {
            actions.Add(entityItem.ActionName);
        }
    }

    public void AddOption(string option)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("Can't add option from client.");
            return;
        }

        actions.Add(option);
        AddOptionClientRPC(option);
    }

    [ClientRpc]
    public void AddOptionClientRPC(string option)
    {
        actions.Add(option);
    }

    public void AddOption(ContextualMenuItem option)
    {
        AddOption(option.ActionName);
    }

    [ClientRpc]
    public void RemoveOptionClientRPC(string option)
    {
        actions.Remove(option);
    }


    public void RemoveOption(string option)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("Can't remove option from client.");
            return;
        }

        actions.Remove(option);
        RemoveOptionClientRPC(option);
    }

    public void RemoveOption(ContextualMenuItem option)
    {
        RemoveOption(option.ActionName);
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
}

