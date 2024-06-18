using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveModule))]
public class SaveModuleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveModule saveModule = (SaveModule)target;
        if (GUILayout.Button("Reset Data")) 
        {
            saveModule.resetData();
        }
    }
}