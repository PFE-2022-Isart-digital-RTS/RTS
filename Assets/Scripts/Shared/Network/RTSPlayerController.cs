using ContextualMenuPackage;
using System;
using System.Collections;
using System.Collections.Generic;
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

    List<Squad> squads = new List<Squad>();

    // TODO : Set ownership ?
    // However, if two players are in the same team and they can move each other's units,
    // then ownership should be false and team should be checked
    [ServerRpc(RequireOwnership = false)]
    public void TryMoveToServerRPC(NetworkBehaviourReference[] moveComponents, Vector3 targetPosition, ServerRpcParams serverRpcParams = default)
    {
        // TODO : verify if the call is correct, depending on the client id calling this function
        //ulong playerID = serverRpcParams.Receive.SenderClientId;

        List<MoveComponent> units = new List<MoveComponent>();
        foreach (NetworkBehaviourReference unitRef in moveComponents)
        {
            if (unitRef.TryGet(out MoveComponent moveComp))
            {
                TeamComponent team = moveComp.GetComponent<TeamComponent>();
                if (team != null && team.team == PlayerState.Team)
                    units.Add(moveComp);
            }
        }

        squads.Add(new Squad { squadUnits = units });

        Vector3 OffsetFromStart;
        int unitCount = moveComponents.Length;
        float unitCountSqrt = Mathf.Sqrt(unitCount);
        int NumberOfCharactersRow = (int)unitCountSqrt;
        int NumberOfCharactersColumn = (int)unitCountSqrt + unitCount - NumberOfCharactersRow * NumberOfCharactersRow;
        float Distance = 1f;

        OffsetFromStart = new Vector3(NumberOfCharactersRow * Distance / 2f, 0f,
            NumberOfCharactersColumn * Distance / 2f);

        for (int i = 0; i < unitCount; i++)
        {
            if (moveComponents[i].TryGet(out MoveComponent moveComponent))
            {
                int r = i / NumberOfCharactersRow;
                int c = i % NumberOfCharactersRow;
                Vector3 offset = new Vector3(r * Distance, 0f, c * Distance);
                Vector3 pos = targetPosition + offset - OffsetFromStart;
                //entity.MoveTo(pos);
                Instruction moveInstruction = new MoveInstruction { moveComponent = moveComponent, targetLocation = pos };
                // to modify, the instruction should be added to the HaveInstruction component
                moveComponent.moveInstruction = moveInstruction;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryBuyItemServerRPC(string actionName, NetworkBehaviourReference[] contextualizables, ServerRpcParams serverRpcParams = default)
    {
        List<HaveOptionsComponent> haveOptionsCompsList = new List<HaveOptionsComponent>(contextualizables.Length);
        foreach (NetworkBehaviourReference entitySelected in contextualizables)
        {
            if (entitySelected.TryGet(out NetworkBehaviour newNetBehaviour))
            {
                if (newNetBehaviour is HaveOptionsComponent optionComp)
                {
                    haveOptionsCompsList.Add(optionComp);
                }
            }
        }

        List<HaveOptionsComponent> validOptionComps = haveOptionsCompsList.FindAll((HaveOptionsComponent optionsComp) => optionsComp.actions.Contains(actionName));
        if (validOptionComps.Count == 0)
            return;

        ContextualMenuItem item = availableItems.Find((ContextualMenuItem menuItem) => menuItem.ActionName == actionName);
        if (item == null)
            Debug.LogWarning("Player can't purchase item : item not listed in RTSPlayerController");
        else 
            item.TryPurchase(validOptionComps[0]);

    }

    // List<ICanBeSelected> selectedEntities;

    // TODO : UI


    //void OnEntitySpawned(Entity newEntity)
    //{
    //    var m = newEntity.GetComponent<LifeComponent>();
    //    if (m != null)
    //    {
    //        HealthBar health = Instantiate(HealthBar3D, m.transform);
    //        health.life = m.lifeRatio;
    //    }
    //}


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

    public List<ContextualMenuItem> availableItems = new List<ContextualMenuItem>();

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
            m_contextualMenu.InvokeTask("Move");
        });

        btnStop.onClick.AddListener(delegate
        {
            m_contextualMenu.InvokeTask("Stop");
        });

        m_contextualMenu.AddTask("Move", new MoveContext());
        m_contextualMenu.AddTask("Stop", new Stop());
        foreach (ContextualMenuEntity entityItem in availableItems)
        {
            m_contextualMenu.AddTask(entityItem.ActionName, entityItem);
        }
    }

    private void OnEnable()
    {
        if (!IsOwner)
            return;

        m_unitSelection.OnSelection += selected =>
        {
            m_contextualMenu.SetContextualizable(selected);

            btnMove.gameObject.SetActive(false);
            btnMove.isOn = false;

            btnStop.gameObject.SetActive(false);

            var v = m_contextualMenu.GetTasks();

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
                        if (m_contextualMenu.GetTask(task) is ContextualMenuItem menuItem)
                        {
                            Button itemButton = Instantiate(buyItemButtonPrefab, btnStop.transform.parent).GetComponent<Button>();
                            Image itemIcon = itemButton.gameObject.GetComponent<Image>();
                            itemButton.onClick.RemoveAllListeners();
                            itemButton.onClick.AddListener(delegate
                            {
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
    }

    private void Update()
    {
        if (!IsOwner)
            return;

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
                    m_unitSelection.SetObserver(PlayerState.Team.Selectables);
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
}
