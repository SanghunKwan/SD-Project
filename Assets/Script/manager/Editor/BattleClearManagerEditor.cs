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
    SerializedProperty[][] properties2;

    private void OnEnable()
    {
        battleClearManager = (BattleClearManager)target;
        properties = new SerializedProperty[2];
        properties2 = new SerializedProperty[2][];
        properties2[0] = new SerializedProperty[0];
        properties2[1] = new SerializedProperty[2];

        properties[0] = serializedObject.FindProperty("StageEndButton");
        properties[1] = serializedObject.FindProperty("characterList");

        properties2[1][0] = serializedObject.FindProperty("heroTeam");
        properties2[1][1] = serializedObject.FindProperty("saveStageView");

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
        foreach (var item in properties2[propertyIndex])
        {
            EditorGUILayout.PropertyField(item);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
