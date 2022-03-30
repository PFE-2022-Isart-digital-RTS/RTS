using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameStartData 
{
    // Defines the nb of players in the game as well as their team.
    // If two players are in the same team, they should have the same id
    // ids are arbitrary (e.g. 10, 4, -3, 68)
    // eachPlayerTeamID.Count must be equal to nbPlayers
    public List<int> eachPlayerTeamID; 
}
