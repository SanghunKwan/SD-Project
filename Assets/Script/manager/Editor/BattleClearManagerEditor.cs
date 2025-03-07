using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BattleClearManager))]
public class BattleClearManagerEditor : Editor
{
    BattleClearManager battleClearManager;
    SerializedProperty[] properties;

    private void OnEnable()
    {
        battleClearManager = (BattleClearManager)target;
        properties = new SerializedProperty[2];
        properties[0] = serializedObject.FindProperty("StageEndButton");
        properties[1] = serializedObject.FindProperty("characterList");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.indentLevel++;
        ShowProperty((int)battleClearManager.ManagerType);
        EditorGUI.indentLevel--;
    }
    void ShowProperty(int propertyIndex)
    {
        EditorGUILayout.PropertyField(properties[propertyIndex]);
        serializedObject.ApplyModifiedProperties();
    }

}
