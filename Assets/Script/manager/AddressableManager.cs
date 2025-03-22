using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;
public class AddressableManager : MonoBehaviour
{
    Dictionary<string, AsyncOperationHandle<IList<Sprite>>> loadedData;
    Dictionary<string, AsyncOperationHandle<VideoClip>> loadedVideo;

    [SerializeField] AssetLabelReference[] label;

    public static AddressableManager manager;
    int loadedDataNum;
    Action onDataLoadComplete;
    bool isCompleted;
    #region AddressableEnum
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
        Tower,
        SummonMagicCircle
    }
    public enum ItemQuality
    {
        Base = 1,
        Normal,
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
    public enum MainMenuImage
    {
        Logo,
        Title,
        TrashCan,
        TrashCanLid
    }
    public enum LabelName
    {
        MainMenu,
        Building,
        VideoView,
        VideoMouseMove,
        MeleeUpgraded,
        RangeUpgraded,
        RangeNormal,
        MeleeNormal,
        RangeBase,
        MeleeBase,
        RangeSkill,
        MeleeSkill,
        StageSettlement,
        VilligeWindowImage
    }
    public enum StageSettlementImage
    {
        ClearImage,
        TitleImage
    }
    public enum VilligeWindowImage
    {
        OK,
        Upgrade,
        Lock
    }
    #endregion

    private void Awake()
    {
        isCompleted = false;
        InheritOldManager();

        manager = this;
        OrganizeData();

    }
    void InheritOldManager()
    {
        if (manager != null)
        {
            loadedData = manager.loadedData;
            loadedVideo = manager.loadedVideo;
        }
        else
        {
            loadedData = new();
            loadedVideo = new();
        }
    }
    void OrganizeData()
    {
        foreach (var item in loadedVideo.Values)
        {
            Addressables.Release(item);
        }
        loadedVideo.Clear();

        HashSet<string> hashLabelString = new HashSet<string>(label.Select(str => str.labelString));

        string[] labelString = (from key in loadedData.Keys
                                where !hashLabelString.Contains(key)
                                select key).ToArray();

        int length = labelString.Length;

        for (int i = 0; i < length; i++)
        {
            UnloadData(labelString[i]);
        }

        length = label.Length;
        for (int i = 0; i < length; i++)
        {
            LoadData(label[i].labelString, DataCountChange);
        }
        Debug.Log("현재 data 수 : " + loadedData.Count);
    }
    public void LoadData(in string LabelName, Action action = null)
    {
        if (!loadedData.ContainsKey(LabelName))
        {
            AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(LabelName, (none) => { });
            loadedData.Add(LabelName, handle);
            handle.CompletedTypeless += (asdf) => action?.Invoke();
        }
        else
            action?.Invoke();
    }
    void DataCountChange()
    {
        int count = loadedData.Count;
        loadedDataNum++;
        Debug.Log(loadedDataNum);
        if (loadedDataNum == count)
        {
            onDataLoadComplete?.Invoke();
            onDataLoadComplete = null;
            isCompleted = true;
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
    public void GetData<T>(LabelName labelName, T skillImage, out Sprite sprite) where T : struct, Enum
    {
        GetData(labelName.ToString(), skillImage, out sprite);
    }
    public void AddEvent(LabelName labelName, Action action)
    {
        if (loadedData[labelName.ToString()].IsDone)
            action();
        else
            loadedData[labelName.ToString()].Completed += (asy) => { action(); };
    }
    async void ChangeSprite(Image image, AsyncOperationHandle<IList<Sprite>> handle)
    {

        await handle.Task;
        Debug.Log(handle.Status);
        //image.sprite = loadedData[LabelName].Result[0];
    }
    public void UnloadData(in AsyncOperationHandle<IList<Sprite>> handle)
    {
        Addressables.Release(handle);
    }
    public void UnloadData(in string key)
    {
        Addressables.Release(loadedData[key]);
        loadedData.Remove(key);
    }
    public void DelayUntilLoadingComplete(in Action action)
    {
        if (isCompleted)
            action();
        else
            onDataLoadComplete += action;
    }
}
