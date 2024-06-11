using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(T11Joystick))]
public class T11JoystickEditor : JoystickEditor
{
    private SerializedProperty moveThreshold;
    private SerializedProperty defaultJoystick;

    protected override void OnEnable()
    {
        base.OnEnable();
        moveThreshold = serializedObject.FindProperty("moveThreshold");
        defaultJoystick = serializedObject.FindProperty("defaultJoystick");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (background != null)
        {
            RectTransform backgroundRect = (RectTransform)background.objectReferenceValue;
            backgroundRect.anchorMax = Vector2.zero;
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.pivot = center;
        }
    }

    protected override void DrawValues()
    {
        base.DrawValues();
        EditorGUILayout.PropertyField(moveThreshold, new GUIContent("Move Threshold", "The distance away from the center input has to be before the joystick begins to move."));
        EditorGUILayout.PropertyField(defaultJoystick, new GUIContent("Default Joystick", "default Joystick Object for T11 Joystick Component"));
    }
}