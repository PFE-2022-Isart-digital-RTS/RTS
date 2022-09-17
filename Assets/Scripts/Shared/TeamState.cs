using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// The resources of the players
[System.Serializable]
public struct TeamResources
{
    public float nbGolds;
    public float nbSupplies;

    public static TeamResources operator +(TeamResources a) => a;
    public static TeamResources operator -(TeamResources a) => new TeamResources()
    {
        nbGolds = - a.nbGolds,
        nbSupplies = - a.nbSupplies
    };

    public static TeamResources operator +(TeamResources a, TeamResources b) => new TeamResources()
    {
        nbGolds = a.nbGolds + b.nbGolds,
        nbSupplies = a.nbSupplies + b.nbSupplies
    };

    public static TeamResources operator -(TeamResources a, TeamResources b) => a + (-b);
    public static TeamResources operator *(TeamResources a, float b) => new TeamResources()
    {
        nbGolds = a.nbGolds * b,
        nbSupplies = a.nbSupplies * b
    };

    public static bool operator <=(TeamResources a, TeamResources b)
    {
        return a.nbGolds <= b.nbGolds && a.nbSupplies <= b.nbSupplies;
    }
    public static bool operator >=(TeamResources a, TeamResources b)
    {
        return a.nbGolds >= b.nbGolds && a.nbSupplies >= b.nbSupplies;
    }
}

// Contains Team data that is dependant on the game
// Team that is manageable (has resources, can win, lose...)
// e.g. : The team of a Player or a StrategyAI, etc
// Strategy AIs and Players should have the same TeamState instance (so that we can switch them at runtime, for debugging etc)
public class TeamState : TeamStateBase
{
    //public List<RTSPlayerState> playersInTheTeam;

    // Resources
    TeamResources resources = new TeamResources()
    {
        nbGolds = 10,
        nbSupplies = 10
    };

    public TeamResources Resources { get => resources; set => resources = value; }

    // Upgrades
    public float unitsDamageMultiplier = 1.0f;

    // The list of buildings the player can potentially construct
    // This is a safety for networking ; a cheater won't be able to spawn a prefab that is not in the list
    // Used server only
    public List<GameObject> availableBuildings = new List<GameObject>();

    #region OnGameEnd

    // Called when the team loses
    public virtual void OnTeamLose()
    {
        // TODO : display a "you lost" message to the players of the team
        // and make them able to leave the game?

        throw new System.NotImplementedException();
    }

    // Called when the team wins
    public virtual void OnTeamWin()
    {
        // TODO : display a "you won" message to the players of the team?

        throw new System.NotImplementedException();
    }

    #endregion
}
