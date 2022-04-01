using System;
using System.Collections.Generic;
using UnitSelectionPackage;
using UnityEngine;

public class Entity : MonoBehaviour, ISelectable, IHaveOptions
{
    private bool m_isSelected = false;
    private Material m_material;
    public ETeam team;
    private Color m_baseColor;

    private void OnEnable()
    {
        GameManager.Instance.RegisterUnit(team, this);
    }

    private void OnDisable()
    {
        if(gameObject.scene.isLoaded)
            GameManager.Instance.UnregisterUnit(team, this);
    }

    protected void Awake()
    {
        m_material = GetComponent<Renderer>().material;
        m_baseColor = m_material.color;
    }

    public void SetSelected(bool selected)
    {
        m_isSelected = selected;
        m_material.color = m_isSelected ? Color.yellow : m_baseColor;
    }
    
    public bool IsSelected()
    {
        return m_isSelected;
    }
    
    public List<Option> GetOptions()
    {
        throw new NotImplementedException();
    }
}
