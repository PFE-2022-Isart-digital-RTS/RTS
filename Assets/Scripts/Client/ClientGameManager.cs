using System;
using System.Collections.Generic;
using System.Linq;
using ContextualMenuPackage;
using UnitSelectionPackage;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*public class NetworkEntityList : INetworkSerializable
{
    private List<int> m_teamsUnits;
        
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        if (serializer.IsWriter)
        {
            length = m_teamsUnits.Count;
        }
 
        serializer.SerializeValue(ref length);
 
        // Array
        if (serializer.IsReader)
        {
            m_teamsUnits = new List<int>(length);
            
            for (int n = 0; n < length; ++n)
            {
                int val = 0;
                serializer.SerializeValue(ref val);
                m_teamsUnits.Add(val);
            }
        }
        else
        {
            for (int n = 0; n < length; ++n)
            {
                int val = m_teamsUnits[n];
                serializer.SerializeValue(ref val);
            }
        }
    }
}*/

public class ClientGameManager : SingletonMonoBehaviour<ClientGameManager>
{
    List<Entity>[] m_teamsUnits = new List<Entity>[(int) ETeam.TeamCount];
    
    private SharedContextualMenu<Entity> m_contextualMenu = new SharedContextualMenu<Entity>();
    
    public Camera mainCamera;
    private UnitSelection<Entity> m_unitSelection = new UnitSelection<Entity>();
    private bool m_isSelecting;
    public EventSystem m_eventSystem;
    private int m_layerGround;
    
    public Toggle btnMove;
    public Button btnStop;
    
    public Action<Vector3> RequestPosition { get; set; }

    
    #region MonoBehaviour

    private void Awake()
    {
        SharedGameManager.Instance.onRegisterEntity += RegisterEntity;
        SharedGameManager.Instance.onUnregisterEntity += UnregisterEntity;
        
        m_layerGround = 1 << LayerMask.NameToLayer("Floor");
        
        for (var index = 0; index < m_teamsUnits.Length; index++)
        {
            m_teamsUnits[index] = new List<Entity>();
        }
        
        btnMove.onValueChanged.AddListener(delegate
        {
            m_contextualMenu.InvokeTask("Move");
        });
        
        btnStop.onClick.AddListener(delegate
        {
            m_contextualMenu.InvokeTask("Stop");
        });
        
        m_contextualMenu.AddTask("Move", new MoveContext());
        m_contextualMenu.AddTask("Stop", new Stop());
    }

    private void OnEnable()
    {
        m_unitSelection.SetObserver(m_teamsUnits[(int) ETeam.Team1]);
        
        m_unitSelection.OnSelection += selected =>
        {
            m_contextualMenu.SetContextualizable(selected);

            btnMove.gameObject.SetActive(false);
            btnMove.isOn = false;

            btnStop.gameObject.SetActive(false);

            foreach (string task in m_contextualMenu.GetTasks())
            {
                switch (task)
                {
                    case "Move":
                        btnMove.gameObject.SetActive(true);
                        break;
                    case "Stop":
                        btnStop.gameObject.SetActive(true);
                        break;
                }
            }
        };
    }
    
    private void Update()
    {
        if (m_eventSystem != null)
        {
            // On click on world
            bool isPointerOverGameObject = m_eventSystem.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(1) && !isPointerOverGameObject)
            {
                if (RequestPosition != null)
                {
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,
                        m_layerGround))
                    {
                        RequestPosition.Invoke(hit.point);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && !isPointerOverGameObject)
            {
                if (!m_isSelecting)
                {
                    m_unitSelection.OnSelectionBegin(Input.mousePosition);
                    m_isSelecting = true;
                }
            }

            if (m_isSelecting)
            {
                m_unitSelection.OnSelectionProcess(mainCamera, Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0) && !isPointerOverGameObject)
            {
                RequestPosition = null;
                m_unitSelection.OnSelectionEnd();
                m_isSelecting = false;
            }
        }
    }

    private void OnGUI()
    {
        if (m_isSelecting)
        {
            m_unitSelection.DrawGUI(Input.mousePosition);
        }
    }

    #endregion
    
    /// <summary>
    /// Need to be called in OnEnable
    /// </summary>
    /// <example>
    ///private void OnEnable()
    ///{
    ///    GameManager.Instance.RegisterUnit(team, this);
    ///}
    /// </example>
    /// <param name="team"></param>
    public void RegisterEntity(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Add(unit);
    }

    /// <summary>
    /// Need to be called in OnDisable
    /// </summary>
    /// <example>
    ///private void OnDisable()
    ///{
    ///    if(gameObject.scene.isLoaded)
    ///        GameManager.Instance.UnregisterUnit(team, this);
    ///}
    /// </example>
    /// <param name="team"></param>
    public void UnregisterEntity(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Remove(unit);
    }
}
