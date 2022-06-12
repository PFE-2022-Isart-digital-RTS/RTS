using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSGameMode : MonoBehaviour
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
    [HideInInspector]
    public List<TeamState> teams;

    [HideInInspector]
    public RTSGameState gameState;


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


    public void StartGame(RTSGameStartData gameStartData)
    {
        // GameState
        gameState = Instantiate(gameStatePrefab).GetComponent<RTSGameState>();

        for (int i = 0; i < gameStartData.nbTeams; i++)
        {
            // TeamState
            TeamState teamState = Instantiate(teamStatePrefab, transform).GetComponent<TeamState>();
            {
                teamState.gameObject.name = "TeamState " + i;

                NetworkObject teamStateNetwork = teamState.GetComponent<NetworkObject>();
                teamStateNetwork.Spawn();

                teams.Add(teamState);
            }
        }

        for (int i = 0; i < gameStartData.playersStartData.Count; i++)
        {
            RTSPlayerStartData playerStartData = gameStartData.playersStartData[i];

            // PlayerState
            RTSPlayerState playerState = Instantiate(playerStatePrefab, transform).GetComponent<RTSPlayerState>();
            {
                playerState.gameObject.name = "PlayerState " + i;
                playerState.client = playerStartData.client;
                playerState.team = teams[playerStartData.teamID];
                gameState.playerStates.Add(playerState);

                NetworkObject pStateNetwork = playerState.GetComponent<NetworkObject>();
                pStateNetwork.SpawnWithOwnership(playerState.client.ClientId);
            }

            // PlayerController
            RTSPlayerController playerController = Instantiate(playerControllerPrefab, transform).GetComponent<RTSPlayerController>();
            {
                playerController.gameObject.name = "PlayerController " + i;

                NetworkObject pControllerNetwork = playerController.GetComponent<NetworkObject>();
                pControllerNetwork.SpawnWithOwnership(playerState.client.ClientId);

                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerState.client.ClientId }
                    }
                };
                playerController.SetLocalInstance(playerController, clientRpcParams);

                playerControllers.Add(playerController);
            }
        }
    }
}
