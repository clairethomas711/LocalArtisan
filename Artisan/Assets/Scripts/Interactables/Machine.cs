using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    public abstract List<ItemData> AcceptedItems { get; set; }
    public abstract List<ItemData> ProducedItems { get; set; }

}
