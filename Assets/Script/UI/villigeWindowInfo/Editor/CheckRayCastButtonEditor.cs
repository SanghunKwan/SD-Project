using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(CheckRaycastButton))]
public class CheckRayCastButtonEditor : Editor
{
    CheckRaycastButton checkRaycastButton;
    SerializedProperty equipImage;
    SerializedProperty previewImage;
    SerializedProperty typeProperty;
    private void OnEnable()
    {
        checkRaycastButton = (CheckRaycastButton)target;
        equipImage = serializedObject.FindProperty("equipImage");
        previewImage = serializedObject.FindProperty("previewImage");
        typeProperty = serializedObject.FindProperty("type");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(typeProperty);

        if (checkRaycastButton.type == UpgradeSlot.SlotType.Item)
            EditorGUILayout.PropertyField(equipImage);
        else
            EditorGUILayout.PropertyField(previewImage);

        serializedObject.ApplyModifiedProperties();

    }
}