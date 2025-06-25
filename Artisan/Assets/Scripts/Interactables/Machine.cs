using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    public abstract List<InventorySlot> AcceptedItems { get; set; }
    public abstract List<InventorySlot> ProducedItems { get; set; }

}
