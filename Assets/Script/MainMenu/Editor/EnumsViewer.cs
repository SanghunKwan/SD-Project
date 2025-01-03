using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NeedAddressable))]
[CanEditMultipleObjects]
public class EnumsViewer : Editor
{
    public NeedAddressable needAddressable;
    public ScriptableObject obj;

    List<Action> EnumsActions = new List<Action>();
    SerializedProperty propertyLabel;


    public void OnEnable()
    {
        SetDictionary();

        needAddressable = target as NeedAddressable;

        if (AssetDatabase.Contains(target))
            needAddressable = null;

        propertyLabel = serializedObject.FindProperty("label");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.ObjectField("Script", needAddressable, typeof(NeedAddressable), false);
        needAddressable.image = EditorGUILayout.ObjectField("Image", needAddressable.image, typeof(UnityEngine.UI.Image), true) as UnityEngine.UI.Image;
        EditorGUILayout.PropertyField(propertyLabel);
        needAddressable.Type = (NeedAddressable.EnumType)EditorGUILayout.EnumPopup("EnumType", needAddressable.Type);

        EnumsActions[(int)needAddressable.Type]();

        serializedObject.ApplyModifiedProperties();
    }
    void SetDictionary()
    {
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.previewImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.equipsImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.buildingImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.itemQuality));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.video));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.mainMenuImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.stageSettlementImage));
    }
    void EnumPopSubstitute<T>(ref T imageTag) where T : struct, Enum
    {
        imageTag = (T)EditorGUILayout.EnumPopup(needAddressable.Type.ToString(), imageTag);
    }
}
