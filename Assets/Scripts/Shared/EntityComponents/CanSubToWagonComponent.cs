using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TeamComponent))]
public class CanSubToWagonComponent : CanSubscribeComponent
{
    TeamComponent teamComp;

    public TeamStateBase Team { get => teamComp.Team; }

    private void Awake()
    {
        teamComp = GetComponent<TeamComponent>();
    }
}
