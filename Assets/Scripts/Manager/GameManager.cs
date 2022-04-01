using System.Collections.Generic;
using UnitSelectionPackage;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    private UnitSelection<Entity> m_unitSelection = new UnitSelection<Entity>();
    private List<Entity>[] m_teamsUnits = new List<Entity>[(int) ETeam.TeamCount];
    private bool m_isSelecting;

    private static GameManager m_Instance = null;

    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<GameManager>();
                if (m_Instance == null)
                {
                    GameObject newObj = new GameObject("GameManager");
                    m_Instance = Instantiate(newObj).AddComponent<GameManager>();
                }
            }

            return m_Instance;
        }
    }

    private void Awake()
    {
        for (var index = 0; index < m_teamsUnits.Length; index++)
            m_teamsUnits[index] = new List<Entity>();
    }

    private void OnEnable()
    {
        m_unitSelection.SetObserver(m_teamsUnits[(int) ETeam.Team1]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!m_isSelecting)
            {
                m_unitSelection.OnSelectionBegin(Input.mousePosition);
                m_isSelecting = true;
            }
        }

        if (m_isSelecting)
            m_unitSelection.OnSelectionProcess(mainCamera, Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            m_unitSelection.OnSelectionEnd();
            m_isSelecting = false;
        }
    }

    private void OnGUI()
    {
        if (m_isSelecting)
            m_unitSelection.DrawGUI(Input.mousePosition);
    }

    public void RegisterUnit(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Add(unit);
    }

    public void UnregisterUnit(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Remove(unit);
    }
}