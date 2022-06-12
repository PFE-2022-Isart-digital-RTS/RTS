using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains server data that is team specific
public class TeamState : MonoBehaviour
{
    //public List<RTSPlayerState> playersInTheTeam;

    // Resources
    public float nbGolds;
    public float nbSupplies;

    //// Buildings and units that the team owns
    //List<GameObject> entities;


    // Called when the team loses
    public void OnTeamLose()
    {
        // TODO : display a "you lost" message to the players of the team
        // and make them able to leave the game?

        throw new System.NotImplementedException();
    }

    // Called when the team wins
    public void OnTeamWin()
    {
        // TODO : display a "you won" message to the players of the team?

        throw new System.NotImplementedException();
    }
}
