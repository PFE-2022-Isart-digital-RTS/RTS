using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LifeComponent), typeof(TeamComponent))]
public class WagonSubscriptionComponent : CanBeSubscribedToComponent
{
    LifeComponent lifeComponent;
    TeamComponent teamComp;

    TeamStateBase currentTeam = null;
    List<TeamStateBase> teams = new List<TeamStateBase>();
    Dictionary<TeamStateBase, float> pointsPerTeamPerFrame = new Dictionary<TeamStateBase, float>();

    private void Awake()
    {
        lifeComponent = GetComponent<LifeComponent>();
        teamComp = GetComponent<TeamComponent>();

        lifeComponent.OnNoLife.AddListener(() => currentTeam = null);
        lifeComponent.OnFullLife.AddListener(CompleteSubscription);
    }

    public void ResetWagonSubscription()
    {
        currentTeam = null;
        lifeComponent.Life = 0;
    }

    private void OnEnable()
    {
        ResetWagonSubscription();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lifeComponent.OnFullLife.RemoveListener(CompleteSubscription);
    }

    public override void CompleteSubscription()
    {
        base.CompleteSubscription();
        teamComp.Team = currentTeam;
    }

    public override void Subscribe(CanSubscribeComponent helper)
    {
        CanSubToWagonComponent sub = (CanSubToWagonComponent) helper;
        if (sub == null)
        {
            Debug.LogError("Subscriber \"" + helper.name + "\" has to have a CanSubToWagonComponent.");
            return;
        }

        if (sub.Team == null)
        {
            Debug.LogError("Subscriber \"" + helper.name + "\" should have a Team assigned.");
            return;
        }

        base.Subscribe(helper);

        if (!teams.Contains(sub.Team))
            teams.Add(sub.Team);
    }

    public static KeyValuePair<TeamStateBase, float> Max(IDictionary<TeamStateBase, float> dict)
    {
        KeyValuePair<TeamStateBase, float> max = new KeyValuePair<TeamStateBase, float>();
        foreach (var entry in dict)
        {
            if (entry.Value > max.Value)
            {
                max = entry;
            }
        }
        return max;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        pointsPerTeamPerFrame.Clear();
        foreach (TeamStateBase team in teams)
        {
            pointsPerTeamPerFrame.Add(team, 0f);
        }

        float total = 0f;

        foreach (CanSubToWagonComponent sub in Subscribers)
        {
            float teamPoints = sub.SpeedMultiplier;

            pointsPerTeamPerFrame[sub.Team] += teamPoints;
            total += teamPoints;
        }

        if (currentTeam == null)
        {
            KeyValuePair<TeamStateBase, float> max = Max(pointsPerTeamPerFrame);
            float otherTeamsTotalPoints = total - max.Value;
            float diff = max.Value - otherTeamsTotalPoints;
            if (diff <= 0)
                return;

            currentTeam = max.Key;
            lifeComponent.Heal(diff * Time.deltaTime * SpeedMultiplier);
        }
        else
        {
            float teamPoints = pointsPerTeamPerFrame[currentTeam];
            float otherTeamsTotalPoints = total - teamPoints;
            float diff = teamPoints - otherTeamsTotalPoints;

            float life = lifeComponent.Life + diff * Time.deltaTime * SpeedMultiplier;
            lifeComponent.Life += diff * Time.deltaTime * SpeedMultiplier;
            if (life <= 0f)
            {
                currentTeam = null;
                // TODO : try giving points
            }

        }
    }
}
