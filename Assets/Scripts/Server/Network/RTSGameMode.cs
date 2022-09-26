using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSGameMode : NetworkBehaviour
{
    [SerializeField, Header("Game")]
    GameObject gameStatePrefab;

    [SerializeField, Header("Player")]
    GameObject playerStatePrefab;

    [SerializeField]
    GameObject playerControllerPrefab;

    [SerializeField, Header("Spectator")]
    GameObject spectatorStatePrefab;

    [SerializeField]
    GameObject spectatorControllerPrefab;

    [SerializeField, Header("Team")]
    GameObject teamStatePrefab;


    [HideInInspector]
    public List<RTSPlayerController> playerControllers = new List<RTSPlayerController>();
    [HideInInspector]
    public List<RTSSpectatorController> spectatorControllers = new List<RTSSpectatorController>();
    public List<TeamState> teams;

    [HideInInspector]
    public RTSGameState gameState;

    public static RTSGameMode Instance = null;
    public InstructionsManager instructionsManager = new InstructionsManager();

    public class RTSPlayerStartData
    {
        public NetworkClient client;
        public int teamID;
    }

    public class RTSSpectatorStartData
    {
        public NetworkClient client;
    }

    public class RTSGameStartData
    {
        public int nbTeams = 2;
        public readonly List<RTSPlayerStartData> playersStartData = new List<RTSPlayerStartData>();
        public readonly List<RTSSpectatorStartData> spectatorsStartData = new List<RTSSpectatorStartData>();
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Instance = this;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    void OnClientConnect(ulong clientID)
    {
        Debug.Log($"Client {clientID} connected");

        RTSSpectatorState spectatorState = Instantiate(spectatorStatePrefab).GetComponent<RTSSpectatorState>();
        {
            spectatorState.gameObject.name = "SpectatorState " + clientID;
            spectatorState.client = NetworkManager.Singleton.ConnectedClients[clientID];
            gameState.spectatorStates.Add(spectatorState);

            NetworkObject pStateNetwork = spectatorState.GetComponent<NetworkObject>();
            pStateNetwork.SpawnWithOwnership(spectatorState.client.ClientId);
        }

        // PlayerController
        RTSSpectatorController spectatorController = Instantiate(spectatorControllerPrefab).GetComponent<RTSSpectatorController>();
        {
            spectatorController.gameObject.name = "PlayerController " + clientID;

            NetworkObject pControllerNetwork = spectatorController.GetComponent<NetworkObject>();
            pControllerNetwork.SpawnWithOwnership(spectatorState.client.ClientId);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { spectatorState.client.ClientId }
                }
            };
            spectatorController.PlayerState = spectatorState;
            spectatorController.SetLocalInstance_ClientRpc(spectatorState, clientRpcParams);

            spectatorControllers.Add(spectatorController);
        }
    }

    void OnClientDisconnect(ulong clientID)
    {
        Debug.Log($"Client {clientID} disconnected");

        spectatorControllers.RemoveAll((RTSSpectatorController specController) => specController.PlayerState.client.ClientId == clientID);
        gameState.spectatorStates.RemoveAll((RTSSpectatorState specState) => specState.client.ClientId == clientID);
    }

    public void StartGame(RTSGameStartData gameStartData)
    {
        // GameState
        gameState = Instantiate(gameStatePrefab).GetComponent<RTSGameState>();
        gameState.GetComponent<NetworkObject>().Spawn();
        gameState.playerTeams = teams;

        for (int i = 0; i < gameStartData.playersStartData.Count; i++)
        {
            RTSPlayerStartData playerStartData = gameStartData.playersStartData[i];

            // PlayerState
            RTSPlayerState playerState = Instantiate(playerStatePrefab).GetComponent<RTSPlayerState>();
            {
                playerState.gameObject.name = "PlayerState " + playerStartData.client.ClientId;
                playerState.client = playerStartData.client;
                playerState.Team = teams[playerStartData.teamID];
                gameState.playerStates.Add(playerState);

                NetworkObject pStateNetwork = playerState.GetComponent<NetworkObject>();
                pStateNetwork.SpawnWithOwnership(playerState.client.ClientId);
            }

            // PlayerController
            RTSPlayerController playerController = Instantiate(playerControllerPrefab).GetComponent<RTSPlayerController>();
            {
                playerController.gameObject.name = "PlayerController " + playerStartData.client.ClientId;

                NetworkObject pControllerNetwork = playerController.GetComponent<NetworkObject>();
                pControllerNetwork.SpawnWithOwnership(playerState.client.ClientId);

                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerState.client.ClientId }
                    }
                };
                playerController.PlayerState = playerState;
                playerController.SetLocalInstance_ClientRpc(playerState, clientRpcParams);

                playerControllers.Add(playerController);
            }
        }

        for (int i = 0; i < playerControllers.Count; i++)
        {
            gameState.playerStates[i].enabled = true;
            gameState.playerStates[i].SetEnable_ClientRpc(true);
            playerControllers[i].SetEnable(true);
        }

        foreach (TeamState team in teams)
        {
            foreach (TeamComponent e in GameObject.FindObjectsOfType<TeamComponent>())
            {
                if (e.Team != null)
                    e.Team.RegisterUnit(e);
            }
        }
    }

    private void Update()
    {
        instructionsManager.Update();
    }
}
