using System.Collections;
using System.Collections.Generic;
using EntSelect;
using UnityEngine;

public class Entity : MonoBehaviour, ISelectable
{
    private bool m_isSelected;
    private Material m_material;

    protected void Awake()
    {
        m_material = GetComponent<Renderer>().material;
    }

    public void SetSelected(bool selected)
    {
        m_isSelected = selected;
        m_material.color = m_isSelected ? Color.red : Color.white;
    }
    
    public bool IsSelected()
    {
        return m_isSelected;
    }
}
