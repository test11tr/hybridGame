using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestCreator))]
public class QuestCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestCreator QuestCreator = (QuestCreator)target;
        if (GUILayout.Button("Create Collect Quest")) 
        {
            QuestCreator.CreateCollectQuest();
        }
        if (GUILayout.Button("Create Level/EXP Quest")) 
        {
            QuestCreator.CreateCharacterLevelQuest();
        }
        if (GUILayout.Button("--Create Explore Quest")) 
        {
            //QuestCreator.CreateExploreQuest();
        }
        if (GUILayout.Button("--Create Build Quest")) 
        {
            //QuestCreator.CreateExploreQuest();
        }
        if (GUILayout.Button("Create Kill Quest")) 
        {
            QuestCreator.CreateKillQuest();
        }
    }
}