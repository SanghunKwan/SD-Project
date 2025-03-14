using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeSlot))]
public class UpgradeSlotEditor : Editor
{
    UpgradeSlot upgradeSlot;
    SerializedProperty itemProperty;
    SerializedProperty skillProperty;
    SerializedProperty typeProperty;
    private void OnEnable()
    {
        upgradeSlot = (UpgradeSlot)target;
        itemProperty = serializedObject.FindProperty("itemPreview");
        skillProperty = serializedObject.FindProperty("skillPreview");
        typeProperty = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(typeProperty);

        if (upgradeSlot.type == UpgradeSlot.SlotType.Item)
            EditorGUILayout.PropertyField(itemProperty);
        else
            EditorGUILayout.PropertyField(skillProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
