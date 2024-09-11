using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;

public class AddressableManager : MonoBehaviour
{
    Dictionary<string, AsyncOperationHandle<IList<Sprite>>> loadedData = new();
    Dictionary<string, AsyncOperationHandle<VideoClip>> loadedVideo = new();

    [SerializeField] AssetLabelReference[] label;


    public enum PreviewImage
    {
        WeaponType,
        ArmorType,
        Skill,
        SpecialMove,
    }
    public enum EquipsImage
    {
        Helmet = 0,
        Armor,
        Weapon,
    }
    public enum BuildingImage
    {
        Tomb = 0,
        Smith,
        Tent,
        LogHouse,
        Bench,
        TimberHouse,
        Canopy,
        GoodHouse,
        StoneHouse,
        Tower
    }
    public enum ItemQuality
    {
        Base = 1,
        Noraml,
        Upgraded,
        MAX,

        기본,
        일반,
        전용
    }
    public enum Video
    {
        None = 0,
        VideoMouseMove = 1,
        VideoView = 2,
    }



    private void Start()
    {
        for (int i = 0; i < label.Length; i++)
        {
            LoadData(label[i].labelString);
        }
    }
    public void LoadData(in string LabelName)
    {
        if (!loadedData.ContainsKey(LabelName))
        {
            AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(LabelName, null);
            loadedData.Add(LabelName, handle);
            string name = LabelName;
            handle.Completed += (asy) =>
            {
                Debug.Log(loadedData[name].Result.Count);
            };
        }
    }
    public void LoadVideo(in string LabelName, Action<VideoClip> action)
    {
        if (!loadedVideo.ContainsKey(LabelName))
        {
            AsyncOperationHandle<VideoClip> handle = Addressables.LoadAssetAsync<VideoClip>(LabelName);
            loadedVideo.Add(LabelName, handle);
            string name = LabelName;
            handle.Completed += (asy) =>
            {
                action(handle.Result);
            };
        }
        else
        {
            action(loadedVideo[LabelName].Result);
        }
    }


    public void GetData<T>(in string LabelName, T skillImage, out Sprite sprite) where T : struct, Enum
    {
        sprite = loadedData[LabelName].Result[Convert.ToInt32(skillImage)];
    }
    async void ChangeSprite(Image image, AsyncOperationHandle<IList<Sprite>> handle)
    {
        await handle.Task;
        Debug.Log(handle.Status);
        //image.sprite = loadedData[LabelName].Result[0];
    }
    public void UnloadData(in string LabelName)
    {
        Addressables.Release(loadedData[LabelName]);
        loadedData.Remove(LabelName);
    }
}
