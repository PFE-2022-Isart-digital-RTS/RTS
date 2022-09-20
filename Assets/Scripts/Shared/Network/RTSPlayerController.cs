using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitSelectionPackage;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO : Link each PlayerController to the PlayerState through network?

// Local to each client, responsible of the ui and the selection, 
// and sends the inputs to the server.
public class RTSPlayerController : PlayerController
{
    public new RTSPlayerState PlayerState { get => (RTSPlayerState) base.PlayerState; set => base.PlayerState = value; }
    public static new RTSPlayerController LocalInstance { get => (RTSPlayerController)PlayerController.LocalInstance; }

    List<HaveOptionsComponent> selectables = new List<HaveOptionsComponent>();

    #region Utility
    List<T> NetBehavioursToComponents<T>(NetworkBehaviourReference[] netBehaviours) where T : NetworkBehaviour
    {
        List<T> units = new List<T>(netBehaviours.Length);
        foreach (NetworkBehaviourReference unitRef in netBehaviours)
        {
            if (unitRef.TryGet(out T moveComp))
            {
                TeamComponent team = moveComp.GetComponent<TeamComponent>();
                if (team != null && team.team == PlayerState.Team)
                    units.Add(moveComp);
            }
        }
        return units;
    }

    List<GameObject> NetObjectsToGameObjects(NetworkObjectReference[] netUnits) 
    {
        List<GameObject> units = new List<GameObject>();
        foreach (NetworkObjectReference unitRef in netUnits)
        {
            if (unitRef.TryGet(out NetworkObject netUnit))
            {
                TeamComponent team = netUnit.GetComponent<TeamComponent>();
                if (team != null && team.team == PlayerState.Team)
                    units.Add(netUnit.gameObject);
            }
        }
        return units;
    }

    #endregion

    #region ServerActions

    public void AssignInstruction(IEnumerable<GameObject> units, SquadInstruction newInstruction)
    {
        RTSGameMode.Instance.instructionsManager.AssignInstruction(units, newInstruction);
    }

    // TODO : Set ownership ?
    // However, if two players are in the same team and they can move each other's units,
    // then ownership should be false and team should be checked
    [ServerRpc(RequireOwnership = false)]
    public void TryMoveToServerRPC(NetworkObjectReference[] unitsReferences, Vector3 targetPosition, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        // Retrieve the list of units
        List<GameObject> units = NetObjectsToGameObjects(unitsReferences);

        AssignInstruction(units, new MoveSquadInstruction { targetPosition = targetPosition });
    }

    // TODO : Set ownership ?
    // However, if two players are in the same team and they can move each other's units,
    // then ownership should be false and team should be checked
    [ServerRpc(RequireOwnership = false)]
    public void TryStopActionServerRPC(NetworkObjectReference[] unitsReferences, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        // Retrieve the list of units
        List<GameObject> units = NetObjectsToGameObjects(unitsReferences);

        AssignInstruction(units, null);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryBuildServerRPC(NetworkObjectReference[] unitsReferences, Vector3 targetPosition, string buildingName, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        // Retrieve the list of units
        List<GameObject> units = NetObjectsToGameObjects(unitsReferences);

        GameObject prefabBuildingToSpawn = PlayerState.Team.availableBuildings.Find((GameObject go) => go.name == buildingName);
        if (prefabBuildingToSpawn == null)
            Debug.LogWarning("Prefab not found in the team state's available buildings : hacks?");
        else
        {
            AssignInstruction(units, new BuildSquadInstruction 
            { 
                targetPosition = targetPosition, 
                buildingPrefab = prefabBuildingToSpawn, 
                team = PlayerState.Team 
            });
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void TryRepairServerRPC(NetworkObjectReference[] unitsReferences, NetworkBehaviourReference canBeRepairedComp, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        // Retrieve the list of units
        List<GameObject> units = NetObjectsToGameObjects(unitsReferences);

        CanBeRepairedComponent canBeRepaired;
        if (canBeRepairedComp.TryGet(out canBeRepaired))
        {
            AssignInstruction(units, new RepairSquadInstruction { inConstructionComp = canBeRepaired });
        }
        else
        {
            Debug.LogError("Invalid component through rpc.");
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TryAttackEntityServerRPC(NetworkObjectReference[] unitsReferences, NetworkObjectReference entity, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        // Retrieve the list of units
        List<GameObject> units = NetObjectsToGameObjects(unitsReferences);

        NetworkObject toAttack;
        if (entity.TryGet(out toAttack))
        {
            LifeComponent lifeComponent = toAttack.GetComponent<LifeComponent>();

            // TODO : MoveToTargetSquadInstruction
            AssignInstruction(units, new AttackSquadInstruction { attackedComp = lifeComponent });
        }
        else
        {
            Debug.LogError("Invalid component through rpc.");
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TryBuyItemServerRPC(string actionName, NetworkBehaviourReference[] contextualizables, ServerRpcParams serverRpcParams = default)
    {
        List<HaveOptionsComponent> haveOptionsCompsList = NetBehavioursToComponents<HaveOptionsComponent>(contextualizables);

        List<HaveOptionsComponent> validOptionComps = haveOptionsCompsList.FindAll((HaveOptionsComponent optionsComp) => optionsComp.actions.Contains(actionName));
        if (validOptionComps.Count == 0)
            return;

        ContextualMenuItem item = (ContextualMenuItem) availableItems.Find((ContextualMenuItemBase menuItem) => menuItem.ActionName == actionName);
        if (item == null)
            Debug.LogWarning("Player can't purchase item : item not listed in RTSPlayerController");
        else
        {
            ContextualMenuItem.InstructionGenerator instructionGenerator = (ContextualMenuItem.InstructionGenerator)item.GetInstructionGenerator();
            instructionGenerator.OnPurchaseStart(validOptionComps);

        }

    }

#endregion

    private SharedContextualMenu<HaveOptionsComponent> m_contextualMenu = new SharedContextualMenu<HaveOptionsComponent>();

    public Camera mainCamera;
    private UnitSelection<HaveOptionsComponent> m_unitSelection = new UnitSelection<HaveOptionsComponent>();
    private bool m_isSelecting;
    public EventSystem m_eventSystem;
    private int m_layerGround;

    public Toggle btnMove;
    public Button btnStop;
    private Button[] btnsBuyItem = new Button[0];
    [SerializeField]
    private GameObject buyItemButtonPrefab;

    public Action<Vector3> RequestPosition { get; set; }
    public Action<GameObject> RequestEntity { get; set; }

    public List<ContextualMenuItemBase> availableItems = new List<ContextualMenuItemBase>();

    List<HaveOptionsComponent> m_selectedEntities = new List<HaveOptionsComponent>();

    #region MonoBehaviour

    private void Awake()
    {
        //if (!IsOwner)
        //if (LocalInstance != this) // Is Owner not working
        //    return;

        //SharedGameManager.Instance.onRegisterEntity += RegisterEntity;
        //SharedGameManager.Instance.onUnregisterEntity += UnregisterEntity;

        m_layerGround = 1 << LayerMask.NameToLayer("Floor");

        btnMove.onValueChanged.AddListener(delegate
        {
            OnContextualMenuButtonPreClick();
            if (btnMove.isOn)
                m_contextualMenu.InvokeTask("Move");
        });

        btnStop.onClick.AddListener(delegate
        {
            OnContextualMenuButtonPreClick();
            m_contextualMenu.InvokeTask("Stop");
        });

        m_contextualMenu.AddTask("Move", new MoveContext());
        m_contextualMenu.AddTask("Stop", new Stop());
        foreach (ContextualMenuItemBase entityItem in availableItems)
        {
            if (entityItem != null)
                m_contextualMenu.AddTask(entityItem.ActionName, entityItem);
            else
                Debug.LogWarning("null element in RTSPlayerController : availableItems");
        }
    }

    public void OnUnitRegistered(TeamComponent unit)
    {
        HaveOptionsComponent haveOptionsComp = unit.GetComponent<HaveOptionsComponent>();
        if (haveOptionsComp != null && !selectables.Contains(haveOptionsComp))
            selectables.Add(haveOptionsComp);
    }

    public void OnUnitUnregistered(TeamComponent unit)
    {
        HaveOptionsComponent haveOptionsComp = unit.GetComponent<HaveOptionsComponent>();
        if (haveOptionsComp != null)
            selectables.Remove(haveOptionsComp);
    }

    void OnContextualMenuButtonPreClick()
    {
        RequestPosition = null;
    }

    private void OnEnable()
    {
        if (!IsOwner)
            return;

        m_unitSelection.OnSelection += selected =>
        {
            m_selectedEntities = selected;
            m_contextualMenu.SetContextualizable(selected);

            btnMove.gameObject.SetActive(false);
            btnMove.isOn = false;

            btnStop.gameObject.SetActive(false);

            foreach (Button button in btnsBuyItem)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }

            string[] tasks = m_contextualMenu.GetTasks();
            int nbTasks = tasks.Length;
            btnsBuyItem = new Button[nbTasks];
            for (int i = 0; i < nbTasks; i++)
            {
                string task = tasks[i];
                switch (task)
                {
                    case "Move":
                        btnMove.gameObject.SetActive(true);
                        break;
                    case "Stop":
                        btnStop.gameObject.SetActive(true);
                        break;
                    default:
                        if (m_contextualMenu.GetTask(task) is ContextualMenuItemBase menuItem)
                        {
                            Button itemButton = Instantiate(buyItemButtonPrefab, btnStop.transform.parent).GetComponent<Button>();
                            Image itemIcon = itemButton.gameObject.GetComponent<Image>();
                            itemButton.onClick.RemoveAllListeners();
                            itemButton.onClick.AddListener(delegate
                            {
                                OnContextualMenuButtonPreClick();
                                m_contextualMenu.InvokeTask(task);
                            });
                            itemIcon.sprite = menuItem.Icon;
                            itemButton.gameObject.SetActive(true);
                            btnsBuyItem[i] = itemButton;
                        }
                        break;
                }
            }
        };

        PlayerState.onTeamChange.AddListener((TeamState oldTeam, TeamState newTeam) =>
        {
            if (oldTeam != null)
            {
                oldTeam.onUnitRegistered.RemoveListener(OnUnitRegistered);
                oldTeam.onUnitUnregistered.RemoveListener(OnUnitUnregistered);
            }

            if (newTeam != null)
            {
                newTeam.onUnitRegistered.AddListener(OnUnitRegistered);
                newTeam.onUnitUnregistered.AddListener(OnUnitUnregistered);
            }
        });
        PlayerState.Team.onUnitRegistered.AddListener(OnUnitRegistered);
        PlayerState.Team.onUnitUnregistered.AddListener(OnUnitUnregistered);
    }

    #region Mouse

    private bool OnMouseOnEnemyEntity(RaycastHit hit, TeamComponent entity)
    {
        LifeComponent lifeComp = entity.GetComponent<LifeComponent>();
        if (lifeComp != null)
        {
            // TODO : Change mouse icon into attack
            if (Input.GetMouseButtonDown(1))
            {
                // TODO : Attack Context
                AttackEntityContext.Context attackEntity = new AttackEntityContext.Context() { };
                attackEntity.OnInvoked(m_selectedEntities);
                RequestEntity.Invoke(lifeComp.gameObject);
                RequestEntity = null;
                return true;
            }
        }
        return false;
    }

    private bool OnMouseOnAllyEntity(RaycastHit hit, TeamComponent entity)
    {
        CanBeRepairedComponent canBeRepairedComp = entity.GetComponent<CanBeRepairedComponent>();
        if (canBeRepairedComp != null)
        {
            // TODO : Change mouse icon into repair
            if (Input.GetMouseButtonDown(1))
            {
                // TODO : Repair Context
                RepairContext.Context repairContext = new RepairContext.Context() { CanBeRepairedComp = canBeRepairedComp };
                repairContext.OnInvoked(m_selectedEntities);
                return true;
            }
        }

        return false;
    }

    private bool OnMouseOnOwnedEntity(RaycastHit hit, TeamComponent entity)
    {
        return OnMouseOnAllyEntity(hit, entity);
    }

    private bool OnMouseNotOnEntity(RaycastHit hit)
    {
        if (RequestPosition == null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                MoveContext moveContext = new MoveContext();
                moveContext.OnInvoked(m_selectedEntities);
                RequestPosition.Invoke(hit.point);
                RequestPosition = null;
                return true;
            }
        }

        return false;
    }

    // Returns true if an action has been done
    private bool OnMouseOnEntity(RaycastHit hit)
    {
        TeamComponent teamComp = hit.collider.transform.parent.GetComponent<TeamComponent>();
        if (teamComp != null)
        {
            switch (teamComp.Team.GetRelationTo(PlayerState.Team))
            {
                case TeamState.TeamRelation.Equal:
                    return OnMouseOnOwnedEntity(hit, teamComp);

                case TeamState.TeamRelation.Ally:
                    return OnMouseOnAllyEntity(hit, teamComp);

                case TeamState.TeamRelation.Enemy:
                    return OnMouseOnEnemyEntity(hit, teamComp);

                default:
                    return false;
            }
        }
        else
        {
            return OnMouseNotOnEntity(hit);
        }
    }

    #endregion

    void UpdateSelection(bool isPointerOverGameObject)
    {
        if (Input.GetMouseButtonDown(0) && !isPointerOverGameObject)
        {
            if (!m_isSelecting)
            {
                m_unitSelection.SetObserver(selectables);
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

    float zoomScale = 300;

    void UpdateZoom()
    {

        mainCamera.transform.position += Input.mouseScrollDelta.y * zoomScale * mainCamera.transform.forward * Time.deltaTime;
    }

    void UpdateMovement()
    {
        Vector3 deltaMov = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            deltaMov += new Vector3(0,0,1);
        }

        if (Input.GetKey(KeyCode.S))
        {
            deltaMov += new Vector3(0, 0, -1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            deltaMov += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            deltaMov += new Vector3(1, 0, 0);
        }

        mainCamera.transform.position += deltaMov * Time.deltaTime * 20;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        // TODO : Reset mouse icon
        if (m_eventSystem != null)
        {
            // On click on world
            bool isPointerOverGameObject = m_eventSystem.IsPointerOverGameObject();

            if (!isPointerOverGameObject)
            {
                if (RequestPosition != null)
                {
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,
                        m_layerGround))
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            RequestPosition.Invoke(hit.point);
                        }
                    }
                }
                else
                {
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                    {
                        if (hit.collider != null)
                        {
                            OnMouseOnEntity(hit);
                        }
                    }
                }
            }

            UpdateSelection(isPointerOverGameObject);
            UpdateZoom();
            UpdateMovement();
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
}
