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


    public void OnEnable()
    {
        SetDictionary();

        needAddressable = target as NeedAddressable;

        if (AssetDatabase.Contains(target))
            needAddressable = null;

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.ObjectField("Script", needAddressable, typeof(NeedAddressable), false);
        needAddressable.image = EditorGUILayout.ObjectField("Image", needAddressable.image, typeof(UnityEngine.UI.Image), true) as UnityEngine.UI.Image;

        needAddressable.Type = (NeedAddressable.EnumType)EditorGUILayout.EnumPopup("EnumType", needAddressable.Type);

        EnumsActions[(int)needAddressable.Type]();
    }
    void SetDictionary()
    {
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.previewImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.equipsImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.buildingImage));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.itemQuality));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.video));
        EnumsActions.Add(() => EnumPopSubstitute(ref needAddressable.mainMenuImage));
    }
    void EnumPopSubstitute<T>(ref T imageTag) where T : struct, Enum
    {
        imageTag = (T)EditorGUILayout.EnumPopup(needAddressable.Type.ToString(), imageTag);
    }
}
