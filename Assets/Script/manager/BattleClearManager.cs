using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using SaveData;

[RequireComponent(typeof(BattleClearPool))]
public class BattleClearManager : MonoBehaviour
{
    [SerializeField] int[] m_linkPosition;
    public int[] linkPosition { get { return m_linkPosition; } }

    public int nowFloorIndex { get; private set; }
    int lastFloorIndex;

    [SerializeField] Vector2[] m_stagePosition;
    public Vector2[] stagePosition { get { return m_stagePosition; } }

    [SerializeField] float[] m_planeSize;
    public float[] planeSize { get { return m_planeSize; } }

    public StageFloorComponent[] stageFloorComponents { get; private set; }

    SaveDataInfo saveData;
    public SaveDataInfo SaveDataInfo { get { return saveData; } }

    [SerializeField] LoadSaveManager loadSaveManager;
    BattleClearPool battleClearPool;

    public InventoryComponent.InventoryType ManagerType;
    [HideInInspector] public UnityEngine.UI.Button StageEndButton;
    [HideInInspector] public CharacterList characterList;
    [HideInInspector] public HeroTeam heroTeam;
    [HideInInspector] public SaveStageView saveStageView;


    Action[] setStageByNextScene;
    public Action<int> SetActiveNewStageObjects { get; set; }
    public Action onStageChanged { get; set; }
    public Action<FloorUnitData, int> onStageSave { get; set; }
    public Func<StageFloorComponent> onVilligeFloorComponentSet { get; set; }

    Queue<int> stageHeroDeleting;

    public HashSet<QuestTrigger> doingQuestTrigger { get; private set; }
    public HashSet<QuestPool.QuestActionInstance> doingQuestActionInstance { get; private set; }


    public enum OBJECTNUM
    {
        BONEWALL,
    }
    private void Awake()
    {
        battleClearPool = GetComponent<BattleClearPool>();

        doingQuestTrigger = new();
        doingQuestActionInstance = new();

        SetAction();

    }
    void SetAction()
    {
        setStageByNextScene = new Action[3];
        setStageByNextScene[0] = () => { };
        setStageByNextScene[1] = () => CallStages(saveData.stageData.floors);
        setStageByNextScene[2] = () => VilligeStageFloorComponetSet();
    }
    private void Start()
    {
        loadSaveManager.LoadData(StageManager.instance.saveDataIndex, out saveData);
        GameManager.manager.SetBattleClearManager(this);
        GameManager.manager.onPlayerEnterStage.eventAction += (num, vec) =>
                                                           {
                                                               if (num == -1) nowFloorIndex++;
                                                           };
        SetStage(saveData.nextScene);
    }

    public CObject CallObject(OBJECTNUM num, Transform trans)
    {
        CObject obj = transform.GetChild((int)num).GetChild(0).GetComponent<CObject>();
        obj.gameObject.SetActive(true);
        obj.transform.position = trans.position;
        obj.transform.SetParent(transform);

        return obj;
    }
    void SetStage(int nextScene)
    {
        setStageByNextScene[nextScene]();
    }
    void CallStages(in int[] floors)
    {
        lastFloorIndex = floors.Length;
        stageFloorComponents = new StageFloorComponent[lastFloorIndex];

        stageFloorComponents[0] = battleClearPool.MakeStage(PoolStageIndex(floors[0]));
        stageFloorComponents[0].gameObject.SetActive(true);
        StageFloorComponent.Direction2Array randomDirection;
        for (int i = 1; i < lastFloorIndex; i++)
        {
            stageFloorComponents[i] = battleClearPool.MakeStage(PoolStageIndex(floors[i]));
            randomDirection = stageFloorComponents[i - 1].GetEmptyDirection();
            stageFloorComponents[i - 1].NewStage(stageFloorComponents[i], randomDirection);
            stageFloorComponents[i - 1].NewLink(battleClearPool.MakeLink(), stageFloorComponents[i], randomDirection);
        }
    }
    public int PoolStageIndex(int nowFloor)
    {
        return (nowFloor / 10) + (Convert.ToInt32(nowFloor % 10 == 0) * 10);
    }

    public void ActivateNextFloor(in QuestSpawner questSpawner, bool needSave = true)
    {
        int nextFloorIndex = nowFloorIndex + 1;
        if (nextFloorIndex < lastFloorIndex)
        {
            GetStageComponent(1).gameObject.SetActive(true);
            questSpawner.PrepareQuest(QuestManager.QuestType.FloorQuest, 1);
            onStageChanged?.Invoke();
        }
        else
        {
            Debug.Log("마지막 스테이지 클리어!");
            StageEndButton.gameObject.SetActive(true);
        }

        GameManager.manager.onStageQuestClear.eventAction?.Invoke(nextFloorIndex, Vector3.zero);

        if (needSave)
            OverrideSaveFile(true);
    }

    void VilligeStageFloorComponetSet()
    {
        stageFloorComponents = new StageFloorComponent[1];
        stageFloorComponents[0] = onVilligeFloorComponentSet();
    }
    #region 데이터 갱신
    void OverrideSaveFile(bool isClear)
    {
        RenewalSavedataInStage(isClear);
        OverrideSaveDataFileHeroInventory();
        OverrideInProgressQuest();
        OverrideSaveDataPlayInfo();
        OverrideSummonHeroList();
    }
    void RenewalSavedataInStage(bool isClear)
    {
        StageData stageData = saveData.stageData;
        stageData.isEnter = false;
        stageData.isClear = isClear;
        stageData.nowFloorIndex = nowFloorIndex;
        int length = nowFloorIndex + 1;
        //saveData.questSaveData
        saveData.questSaveData.isLoaded = true;
        //퀘스트 내용은 실시간 동기화

        stageData.floorUnitDatas = new FloorUnitData[length];
        for (int i = 0; i < length; i++)
        {
            stageData.floorUnitDatas[i] = new FloorUnitData();
            onStageSave?.Invoke(stageData.floorUnitDatas[i], i);
        }
        //형변환 수정 예정.
        //삭제된 cobject Transform 이동 예정.
        stageData.yetDroppedItems = (List<YetDroppedItem>)GameManager.manager.objectManager.YetDroppedItems;
        //saveData.stageData
        //시체 등 objectManager에 없는 요소는 미리 savedata에 저장.
        //item이나 object 등 이동하지 않는 오브젝트는 stage에 종속.
        //monster는 spawn되면 시체가 되지 않고 사라지지 않음.
    }
    void OverrideSaveDataFileHeroInventory(bool needPositionReset = false, Vector3 newPosition = new Vector3())
    {
        StageData stageData = saveData.stageData;
        ObjectManager manager = GameManager.manager.objectManager;
        HeroData hero;

        foreach (Hero item in manager.ObjectList[0])
        {
            hero = saveData.hero[stageData.heros[item.stageIndex]];
            hero.SetHeroData(item);
            if (needPositionReset)
            {
                hero.unitData.objectData.position = newPosition;
                hero.unitData.destination = newPosition + Vector3.back * 2;
                hero.unitData.objectData.quaternion = Quaternion.Euler(0, 180, 0);
                hero.unitData.objectData.selected = false;
            }
        }

        SetInventoryData(stageData, InventoryComponent.InventoryType.Stage);
    }

    public void OverrideSaveDataBeforeSettle()
    {
        OverrideSaveDataFileHeroInventory(true, new Vector3(5, 0, 2));

        DeleteSaveDataFileStageUnitDropItems();
        DeletePlayInfoCamPosition();
        OverrideFloorData();
        OverrideSummonHeroList();

    }
    public void OverrideSaveDataBeforeStage()
    {
        int[] needResetIndexArray = heroTeam.GetHeroStageData();

        OverrideSaveDataVilligeHero();
        OverrideSaveDataFileBuilding();

        //스테이지 데이터 생성.
        StageData stageData = new StageData(saveStageView.GetStageFloorsData(),
                                            needResetIndexArray);

        foreach (var item in needResetIndexArray)
        {
            saveData.hero[item].unitData.objectData.position = 2 * Vector3.forward;
            saveData.hero[item].unitData.destination = Vector3.zero;
            saveData.hero[item].unitData.objectData.quaternion = Quaternion.Euler(0, 180, 0);
            saveData.hero[item].unitData.objectData.selected = false;
        }

        saveData.stageData = stageData;
        //영웅 index list 생성
        saveData.nextScene = 1;
        saveData.day++;
        saveData.playInfo = null;
        saveData.questSaveData.SetStageQuest();
        SetInventoryData(stageData, InventoryComponent.InventoryType.Villige);
        //villige 데이터 저장할 때 int[] savedata.item에 저장해야 함.
        //출정 시 storageManager(villige)를 현재 가지고 있는 item과 연산한 후 저장.
        OverrideSummonHeroList();
    }
    void SetInventoryData(StageData stageData, InventoryComponent.InventoryType inventoryType)
    {
        InventoryStorage storage = GameManager.manager.storageManager
            .inventoryComponents(inventoryType).inventoryStorage;
        stageData.inventoryData.SetData(storage.Inventory2ItemDatas(), storage.GetHeroCorpseIndexs());
    }


    void DeleteSaveDataFileStageUnitDropItems()
    {
        StageData stageData = saveData.stageData;
        saveData.nextScene = 2;
        saveData.day++;

        stageData.nowFloorIndex = 0;
        stageData.isClear = false;
        stageData.isEnter = true;
        stageData.floorUnitDatas = null;
    }
    void DeletePlayInfoCamPosition()
    {
        saveData.playInfo.ResetPosition();
    }
    void OverrideFloorData()
    {
        int expeditionMaxFloor = saveData.floorData.topFloor;
        int length = saveData.stageData.floors.Length;
        for (int i = 0; i < length; i++)
        {
            if (expeditionMaxFloor < saveData.stageData.floors[i] + 1)
            {
                expeditionMaxFloor = saveData.stageData.floors[i] + 1;
            }
        }

        saveData.floorData.topFloor = expeditionMaxFloor;
    }
    void OverrideSaveDataVilligeHero()
    {
        //CharacterList 위에서 아래로 순회하며 저장.
        int viewportLength = characterList.trViewPort.Count;
        int heroLenth;
        List<villigeInteract> tempInteract = new List<villigeInteract>();
        for (int i = 0; i < viewportLength; i++)
        {
            tempInteract.AddRange(characterList.GetViewPortByIndex(i).characters);
        }
        heroLenth = tempInteract.Count;
        saveData.hero = new HeroData[heroLenth];

        for (int i = 0; i < heroLenth; i++)
        {
            saveData.hero[i] = new HeroData(tempInteract[i].hero);
        }
    }
    void OverrideSaveDataFileBuilding()
    {
        ObjectManager manager = GameManager.manager.objectManager;

        int length = manager.NoneObjectList[(int)ObjectManager.AdditionalType.Building].Count;
        saveData.building = new BuildingData[length];
        LinkedListNode<MonoBehaviour> node = manager.NoneObjectList[(int)ObjectManager.AdditionalType.Building].Last;

        for (int i = 0; i < length; i++)
        {
            saveData.building[i] = new BuildingData((BuildingConstructDelay)node.Value);
            node = node.Next;
        }
    }
    void OverrideInProgressQuest()
    {
        //씬 전환 시 이전에 가지고 있던 퀘스트 목록도 사라지는지 체크
        saveData.questSaveData.floorQuestData.nowQuestList.Clear();
        saveData.questSaveData.villigeQuestData.nowQuestList.Clear();
        saveData.questSaveData.stagePerformOneData.nowQuestList.Clear();
        saveData.questSaveData.villigePerformOneData.nowQuestList.Clear();

        foreach (var item in doingQuestActionInstance)
        {
            saveData.questSaveData[item.type].nowQuestList.Add(new QuestSaveData.BitSaveData.QuestProgress(item));
        }

        foreach (var item in doingQuestTrigger)
        {
            saveData.questSaveData[item.type].nowQuestList.Add(new QuestSaveData.BitSaveData.QuestProgress(item));
        }
    }
    ///<summary>
    ///세이브 데이터 저장.
    ///</summary>
    public void OverrideSummonHeroList()
    {
        loadSaveManager.OverrideSaveFile(StageManager.instance.saveDataIndex, saveData);
    }
    public void ComebacktoVillige()
    {
        if (stageHeroDeleting == null)
            stageHeroDeleting = new Queue<int>(saveData.stageData.heros);

        stageHeroDeleting.Dequeue();
        saveData.stageData.heros = stageHeroDeleting.ToArray();
    }
    public void OverrideSaveDataInVillige()
    {
        OverrideSaveDataVilligeHero();
        OverrideSaveDataFileBuilding();
        OverrideInProgressQuest();
        OverrideSaveDataPlayInfo();
        OverrideSummonHeroList();
    }
    void OverrideSaveDataPlayInfo()
    {
        saveData.playInfo.SaveData();
    }
    #endregion
    public void ActiveStageObject()
    {
        SetActiveNewStageObjects(nowFloorIndex);
    }
    public StageFloorComponent GetStageComponent(int offsetIndex)
    {
        return stageFloorComponents[nowFloorIndex + offsetIndex];
    }
    public void IsCleared(QuestSpawner questSpawner)
    {
        if (saveData.stageData.isClear)
            ActivateNextFloor(questSpawner, false);
    }
}