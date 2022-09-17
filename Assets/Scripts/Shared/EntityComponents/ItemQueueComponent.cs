using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemQueueComponent), typeof(TeamComponent))]
public class ItemQueueComponent : MonoBehaviour
{
    List<ContextualMenuItem.InstructionGenerator> items = new List<ContextualMenuItem.InstructionGenerator>();
    HaveOptionsComponent haveOptionsComp;
    TeamComponent team;

    float timer = 0f;

    public void AddItem(ContextualMenuItem.InstructionGenerator item)
    {
        items.Add(item);
    }

    private void Awake()
    {
        haveOptionsComp = GetComponent<HaveOptionsComponent>();
        team = GetComponent<TeamComponent>();
    }

    private void Update()
    {
        if (items.Count == 0)
        {
            timer = 0f;
            return;
        }

        TeamState teamState = (TeamState) team.Team;
        if (teamState == null)
            return;

        timer += Time.deltaTime;
        //TeamResources framePrice = currentItem.Price * Time.deltaTime;

        while (items.Count > 0 && timer > items[0].Data.buyDuration && items[0].CanPurchaseFinalPrice(teamState))
        {
            ContextualMenuItem.InstructionGenerator currentItem = items[0];

            timer -= currentItem.Data.buyDuration;
            currentItem.PayFinalPrice(teamState);
            teamState.Resources -= currentItem.Data.FinalPrice;

            currentItem.OnPurchaseEnd(haveOptionsComp);

            items.RemoveAt(0);
        }
    }


}
