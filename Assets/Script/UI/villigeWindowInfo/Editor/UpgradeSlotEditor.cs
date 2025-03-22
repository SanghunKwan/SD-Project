using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeSlot))]
public class UpgradeSlotEditor : Editor
{
    UpgradeSlot upgradeSlot;
    SerializedProperty[] itemPropertys;
    SerializedProperty[] skillPropertys;
    SerializedProperty typeProperty;
    int propertyLength = 2;
    private void OnEnable()
    {
        upgradeSlot = (UpgradeSlot)target;
        itemPropertys = new SerializedProperty[propertyLength];
        skillPropertys = new SerializedProperty[propertyLength];

        itemPropertys[0] = serializedObject.FindProperty("itemPreview");
        itemPropertys[1] = serializedObject.FindProperty("equipImage");
        skillPropertys[0] = serializedObject.FindProperty("skillPreview");
        skillPropertys[1] = serializedObject.FindProperty("previewImage");
        typeProperty = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(typeProperty);


        if (upgradeSlot.type == UpgradeSlot.SlotType.Item)
        {
            for (int i = 0; i < propertyLength; i++)
                EditorGUILayout.PropertyField(itemPropertys[i]);
        }
        else
        {
            for (int i = 0; i < propertyLength; i++)
                EditorGUILayout.PropertyField(skillPropertys[i]);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
