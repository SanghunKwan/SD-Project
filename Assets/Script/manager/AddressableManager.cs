using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;
public class AddressableManager : MonoBehaviour
{
    Dictionary<string, (AsyncOperationHandle<IList<Sprite>> handle, Sprite[] data)> loadedData;
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
        MagicBase,
        RangeSkill,
        MeleeSkill,
        MagicSkill,
        StageSettlement,
        VilligeWindow
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
        Lock,
        Medicine,
        Healing,
        fullHp,
        halfHp,
        emptyHp
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
    void LoadData(in string LabelName, Action action)
    {
        switch (LabelName + "Image")
        {
            case nameof(MainMenuImage):
                LoadData<MainMenuImage>(LabelName, action);
                break;
            case nameof(VilligeWindowImage):
                LoadData<VilligeWindowImage>(LabelName, action);
                break;
            case nameof(StageSettlementImage):
                LoadData<StageSettlementImage>(LabelName, action);
                break;
            case nameof(BuildingImage):
                LoadData<BuildingImage>(LabelName, action);
                break;
            case string s when s.Contains("Skill"):
                LoadData<PreviewImage>(LabelName, action);
                break;

            default:
                LoadData<EquipsImage>(LabelName, action);
                break;
        }
    }
    public void LoadData<T>(in string LabelName, Action action = null) where T : struct, Enum
    {
        if (!loadedData.ContainsKey(LabelName))
        {
            AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(LabelName, (none) => { });
            int size = Enum.GetValues(typeof(T)).Length;
            Sprite[] list = new Sprite[size];
            loadedData.Add(LabelName, (handle, list));
            string name = LabelName;
            handle.Completed += (compHandle) =>
            {
                for (int i = 0; i < size; i++)
                {
                    string asdf = compHandle.Result[i].name.Split('_')[0];
                    var e = Enum.Parse<T>(asdf);
                    list[Convert.ToInt32(e)] = compHandle.Result[i];
                }
                action?.Invoke();
            };
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
        sprite = loadedData[LabelName].data[Convert.ToInt32(skillImage)];
    }
    public void GetData<T>(LabelName labelName, T skillImage, out Sprite sprite) where T : struct, Enum
    {
        GetData(labelName.ToString(), skillImage, out sprite);
    }
    public void AddEvent(LabelName labelName, Action action)
    {
        if (loadedData[labelName.ToString()].handle.IsDone)
            action();
        else
            loadedData[labelName.ToString()].handle.Completed += (asy) => { action(); };
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
        UnloadData(loadedData[key].handle);
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
