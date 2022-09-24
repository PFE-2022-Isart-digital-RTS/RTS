using System.Collections;
using System.Collections.Generic;
using ContextualMenuPackage;
using UnitSelectionPackage;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class HaveOptionsComponent : NetworkBehaviour, ISelectable, IContextualizable
{
    private bool m_isSelected = false;
    private Material m_material;

    private Color m_baseColor;

    public List<string> actions;

    public List<ContextualMenuItemBase> items = new List<ContextualMenuItemBase>();

    #region MonoBehaviour
    protected void Awake()
    {
        //m_material = GetComponentInChildren<Renderer>().material;
        //if (m_material.HasProperty("m_color"))
        //    m_baseColor = m_material.color;

        foreach (ContextualMenuItemBase entityItem in items)
        {
            actions.Add(entityItem.ActionName);
        }
    }
    #endregion

    #region Options
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

    public void AddOption(ContextualMenuItemBase option)
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

    public void RemoveOption(ContextualMenuItemBase option)
    {
        RemoveOption(option.ActionName);
    }

    public List<string> GetTasks()
    {
        return actions;
    }

    public bool ContainsOption(string option)
    {
        return actions.Contains(option);
    }

    #endregion

    #region Selection

    public void SetSelected(bool selected)
    {
        m_isSelected = selected;
        //if (m_material.HasProperty("m_color"))
        //    m_material.color = m_isSelected ? Color.yellow : m_baseColor;
    }
    
    public bool IsSelected()
    {
        return m_isSelected;
    }

    #endregion
}

