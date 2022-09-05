using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemQueueComponent), typeof(TeamComponent))]
public class ItemQueueComponent : MonoBehaviour
{
    List<ContextualMenuItem> items = new List<ContextualMenuItem>();
    HaveOptionsComponent haveOptionsComp;
    TeamComponent team;

    float timer = 0f;

    public void AddItem(ContextualMenuItem item)
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

        timer += Time.deltaTime;
        //TeamResources framePrice = currentItem.Price * Time.deltaTime;

        while (items.Count > 0 && timer > items[0].buyDuration && items[0].CanPurchaseFinalPrice(team.Team))
        {
            ContextualMenuItem currentItem = items[0];

            timer -= currentItem.buyDuration;
            currentItem.PayFinalPrice(team.Team);
            team.Team.Resources -= currentItem.FinalPrice;

            currentItem.OnPurchaseEnd(haveOptionsComp);

            items.RemoveAt(0);
        }
    }


}
