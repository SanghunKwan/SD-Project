using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedAddressable : InitObject
{


    [HideInInspector] public AddressableManager.LabelName label;

    public enum EnumType
    {
        PreviewImage = 0,
        EquipsImage,
        BuildingImage,
        ItemQuality,
        Video,
        MainMenuImage,
        StageSettlementImage,
        VilligeWindowImage
    }

    [HideInInspector] public EnumType Type;

    [HideInInspector] public AddressableManager.PreviewImage previewImage;
    [HideInInspector] public AddressableManager.EquipsImage equipsImage;
    [HideInInspector] public AddressableManager.BuildingImage buildingImage;
    [HideInInspector] public AddressableManager.ItemQuality itemQuality;
    [HideInInspector] public AddressableManager.Video video;
    [HideInInspector] public AddressableManager.MainMenuImage mainMenuImage;
    [HideInInspector] public AddressableManager.StageSettlementImage stageSettlementImage;
    [HideInInspector] public AddressableManager.VilligeWindowImage villigeWindowImage;


    [HideInInspector] public Image m_image;
    public Image image { get { return m_image; } set { m_image = value; } }
    List<Func<Sprite>> actions = new List<Func<Sprite>>();

    public override void Init()
    {
        SetActions();
        image.sprite = actions[(int)Type]();
    }
    void SetActions()
    {
        actions.Add(() => FuncAction(previewImage));
        actions.Add(() => FuncAction(equipsImage));
        actions.Add(() => FuncAction(buildingImage));
        actions.Add(() => FuncAction(itemQuality));
        actions.Add(() => FuncAction(video));
        actions.Add(() => FuncAction(mainMenuImage));
        actions.Add(() => FuncAction(stageSettlementImage));
        actions.Add(() => FuncAction(villigeWindowImage));
    }
    protected void GetData<T>(T type, out Sprite sprite) where T : struct, Enum
    {
        AddressableManager.manager.GetData(label, type, out sprite);
    }
    Sprite FuncAction<T>(T type) where T : struct, Enum
    {
        GetData(type, out Sprite sprite);
        return sprite;
    }
}
