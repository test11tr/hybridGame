using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveModule))]
public class SaveModuleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Varsayılan Inspector öğelerini çiz

        SaveModule saveModule = (SaveModule)target;
        if (GUILayout.Button("Reset Data")) // Bir buton oluştur ve butona basıldığında
        {
            saveModule.resetData(); // resetData fonksiyonunu çağır
        }
    }
}