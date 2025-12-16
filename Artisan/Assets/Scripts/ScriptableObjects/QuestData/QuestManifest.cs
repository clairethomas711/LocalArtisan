using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestManifest", menuName = "Scriptable Objects / Quest Manifest")]
public class QuestManifest : ScriptableObject
{
    public List<QuestData> quests;
}
