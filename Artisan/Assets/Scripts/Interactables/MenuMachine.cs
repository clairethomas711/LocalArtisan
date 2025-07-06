using UnityEngine;
using System.Collections.Generic;

public class MenuMachine : Machine
{
    [SerializeField] GameObject craftingUIMenu;
    List<ItemData> acceptedItems;
    List<ItemData> productedItems;
    public List<CraftingRecipe> recipes;
    public override List<ItemData> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<ItemData> ProducedItems
    {
        get { return productedItems; }
        set { ProducedItems = value; }
    }

    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        PlayerStateManager p = farm.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.busyState);
        craftingUIMenu.SetActive(true);
    }
}
