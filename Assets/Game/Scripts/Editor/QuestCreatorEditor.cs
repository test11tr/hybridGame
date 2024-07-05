using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestCreator))]
public class QuestCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestCreator QuestCreator = (QuestCreator)target;
        if (GUILayout.Button("Create Collect Currency Quest")) 
        {
            QuestCreator.CreateCollectQuest();
        }
        if (GUILayout.Button("Create Level/EXP Quest")) 
        {
            QuestCreator.CreateCharacterLevelQuest();
        }
        if (GUILayout.Button("Create Character Upgrade Quest")) 
        {
            QuestCreator.CreateUpgradeQuest();
        }
        if (GUILayout.Button("Create Kill Quest")) 
        {
            QuestCreator.CreateKillQuest();
        }
        if (GUILayout.Button("Create Unlock Quest")) 
        {
            QuestCreator.CreateUnlockQuest();
        }
        if (GUILayout.Button("Save Quests!")) {
            GameObject gameManagerObject = GameObject.Find("GameManager");
            if (PrefabUtility.IsPartOfPrefabInstance(gameManagerObject))
            {
                GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(gameManagerObject) as GameObject;
                if (prefabSource != null)
                {
                    PrefabUtility.ApplyPrefabInstance(gameManagerObject, InteractionMode.UserAction);
                }
                else
                {
                    Debug.LogError("GameManager is not part of a prefab.");
                }
            }
        }
    }
}