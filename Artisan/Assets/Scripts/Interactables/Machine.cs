using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    public abstract List<InventoryItem> AcceptedItems { get; set; }
    public abstract List<InventoryItem> ProducedItems { get; set; }

}
