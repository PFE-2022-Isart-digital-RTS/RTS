using System;
using System.Collections.Generic;
using ContextualMenuPackage;
using UnitSelectionPackage;
using UnityEngine;

public class Entity : MonoBehaviour, ISelectable, IContextualizable
{
    private bool m_isSelected = false;
    private Material m_material;
    public ETeam team;
    private Color m_baseColor;

    public List<string> actions;
    
    private bool isMoving;
    private Vector3 positionToReach;
    public float speed = 2f;
    public float distanceToReach = 0.1f;

    #region MonoBehaviour
    protected void Awake()
    {
        m_material = GetComponent<Renderer>().material;
        m_baseColor = m_material.color;
    }
    
    private void OnEnable()
    {
        GameManager.Instance.RegisterEntity(team, this);
    }

    private void OnDisable()
    {
        if(gameObject.scene.isLoaded)
            GameManager.Instance.UnregisterEntity(team, this);
    }
    
    private void FixedUpdate()
    {
        if (isMoving)
        {
            Transform selfTransform = transform;
            Vector3 position = selfTransform.position;
            Vector3 posToTarget = positionToReach - position;
            float posToTargetDistance = posToTarget.magnitude;
            Vector3 direction = posToTarget / posToTargetDistance;
            position += speed * Time.fixedDeltaTime * direction;
            selfTransform.position = position;

            isMoving = posToTargetDistance > distanceToReach;
        }
    }
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
    
    public void StopMovement()
    {
        isMoving = false;
    }
    
    public void MoveTo(Vector3 target)
    {
        isMoving = true;
        positionToReach = target;
    }
}
