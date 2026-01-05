using UnityEngine;
using System.Collections.Generic;

public class ChestManager : MonoBehaviour
{
    public Dictionary<string, List<InventoryItem>> chestManifest = new Dictionary<string, List<InventoryItem>>();
    public ChestMenu defaultChestMenu;
    public RequestChestMenu requestChestMenu;
}
