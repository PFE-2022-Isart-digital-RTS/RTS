using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    List<PlayerState> players = new List<PlayerState>();
    Dictionary<int, TeamData> teams = new Dictionary<int, TeamData>();

    [SerializeField]
    GameObject playerStatesRoot;

    [SerializeField]
    GameObject teamsDataRoot;

    // Should be called to start the game and initialize all the data
    // i.e. as soon as the scene is loaded
    public void StartGame(GameStartData gameStartData)
    {
        players.Capacity = gameStartData.eachPlayerTeamID.Count;

        foreach (int teamID in gameStartData.eachPlayerTeamID)
        {
            if (!teams.ContainsKey(teamID))
            {
                GameObject teamDataGO = new GameObject("TeamData");
                teamDataGO.transform.parent = teamsDataRoot.transform;
                TeamData teamData = teamDataGO.AddComponent<TeamData>();

                teams.Add(teamID, teamData);
            }

            GameObject playerStateGO = new GameObject("PlayerState");
            playerStateGO.transform.parent = playerStatesRoot.transform;
            PlayerState playerState = playerStateGO.AddComponent<PlayerState>();

            playerState.team = teams[teamID];
            players.Add(playerState);
        }

        // TODO : call the start function  in the main buildings

        throw new System.NotImplementedException();
    }

    // Should be called when the game is finished
    // i.e. after a player has won and other players has lost
    public void OnGameFinished()
    {
        // TODO : Endscsreen etc ?
        throw new System.NotImplementedException();
    }
}
