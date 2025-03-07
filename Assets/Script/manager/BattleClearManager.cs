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


    Action[] setStageByNextScene;
    public Action<int> SetActiveNewStageObjects { get; set; }
    public Action onStageChanged { get; set; }
    public Func<StageFloorComponent> onVilligeFloorComponentSet { get; set; }

    List<Hero> spawnedHero;
    List<Monster> spawnedMonster;
    List<ItemComponent> spawnedItem;
    List<CObject> spawnedObject;
    List<BuildingConstructDelay> spawnedBuilding;

    List<int> stageHeroDeleting;

    public enum OBJECTNUM
    {
        BONEWALL,
    }
    private void Awake()
    {
        battleClearPool = GetComponent<BattleClearPool>();

        spawnedHero = new List<Hero>();
        spawnedMonster = new List<Monster>();
        spawnedItem = new List<ItemComponent>();
        spawnedObject = new List<CObject>();
        spawnedBuilding = new();

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
        GameManager.manager.onPlayerEnterStage.eventAction += (num, vec) => { if (num == -1) nowFloorIndex++; };
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
            stageFloorComponents[i - 1].NewLink(battleClearPool.MakeLink(), randomDirection);
        }
    }
    public int PoolStageIndex(int nowFloor)
    {
        return (nowFloor / 10) + (Convert.ToInt32(nowFloor % 10 == 0) * 10);
    }

    public void ActivateNextFloor(in QuestSpawner questSpawner, bool needSave = true)
    {
        if (nowFloorIndex + 1 < lastFloorIndex)
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
        loadSaveManager.OverrideSaveFile(StageManager.instance.saveDataIndex, saveData);
    }
    void RenewalSavedataInStage(bool isClear)
    {
        StageData stageData = saveData.stageData;
        stageData.isEnter = false;
        stageData.isClear = isClear;
        stageData.nowFloorIndex = nowFloorIndex;

        //saveData.questSaveData
        saveData.questSaveData.isLoaded = true;
        //퀘스트 내용은 실시간 동기화

        //saveData.stageData
        SetSaveData(spawnedItem.Count, (num) => new DropItemData(spawnedItem[num]), ref stageData.dropItemDatas);

        SetSaveData(spawnedMonster.Count, (num) => new MonsterData(spawnedMonster[num]), ref stageData.monsterData);

        SetSaveData(spawnedObject.Count, (num) => new ObjectData(spawnedObject[num]), ref stageData.objectDatas);
    }
    void SetSaveData<T1>(int arrayLength, Func<int, T1> newData, ref T1[] stagedata)
    {
        T1[] datas = new T1[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            datas[i] = newData(i);
        }

        stagedata = datas;
    }
    void OverrideSaveDataFileHeroInventory(bool needPositionReset = false, Vector3 newPosition = new Vector3())
    {
        StageData stageData = saveData.stageData;
        HeroData hero;

        foreach (var item in spawnedHero)
        {
            hero = saveData.hero[stageData.heros[item.heroInStageIndex]];
            hero.SetHeroData(item);
            if (needPositionReset)
            {
                hero.unitData.objectData.position = newPosition;
                hero.unitData.objectData.quaternion = Quaternion.Euler(0, 180, 0);
                hero.unitData.objectData.selected = false;
            }
        }

        InventoryStorage storage = GameManager.manager.storageManager
            .inventoryComponents(InventoryComponent.InventoryType.Stage).inventoryStorage;
        stageData.inventoryData.SetData(storage.Inventory2ItemDatas(), storage.GetHeroCorpseIndexs());
    }

    public void OverrideSaveDataBeforeSettle()
    {
        OverrideSaveDataFileHeroInventory(true, new Vector3(5, 0, 2));

        DeleteSaveDataFileStageUnitDropItems();
        loadSaveManager.OverrideSaveFile(StageManager.instance.saveDataIndex, saveData);
    }

    void DeleteSaveDataFileStageUnitDropItems()
    {
        StageData stageData = saveData.stageData;
        saveData.nextScene = 2;
        saveData.day++;

        stageData.nowFloorIndex = 0;
        stageData.isClear = false;
        stageData.isEnter = true;
        stageData.unitData = null;
        stageData.monsterData = null;
        stageData.objectDatas = null;
        stageData.dropItemDatas = null;
    }
    public void OverrideSaveDataInSettle()
    {
        OverrideSaveDataVilligeHero();
        OverrideSaveDataFileBuilding();
        loadSaveManager.OverrideSaveFile(StageManager.instance.saveDataIndex, saveData);
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
        saveData.building = new BuildingData[spawnedBuilding.Count];

        for (int i = 0; i < spawnedBuilding.Count; i++)
        {
            saveData.building[i] = new BuildingData(spawnedBuilding[i]);
        }
    }
    public void ComebacktoVillige()
    {
        if (stageHeroDeleting == null)
            stageHeroDeleting = new List<int>(saveData.stageData.heros);

        stageHeroDeleting.RemoveAt(0);
        saveData.stageData.heros = stageHeroDeleting.ToArray();
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
    #region 세이브 데이터 관리
    public void NewHero(Hero newHero)
    {
        spawnedHero.Add(newHero);
    }
    public void StageOutHero(Hero fadeHero)
    {
        spawnedHero.Remove(fadeHero);
    }
    public void NewMonster(Monster newMonster)
    {
        spawnedMonster.Add(newMonster);
    }
    public void StageOutMonster(Monster fadeMonster)
    {
        spawnedMonster.Remove(fadeMonster);
    }
    public void NewItem(ItemComponent newItem)
    {
        spawnedItem.Add(newItem);
    }
    public void StageOutItem(ItemComponent usedItem)
    {
        spawnedItem.Remove(usedItem);
    }
    public void NewObject(CObject newObject)
    {
        spawnedObject.Add(newObject);
    }
    public void StageOutObject(CObject fadeObject)
    {
        spawnedObject.Remove(fadeObject);
    }
    public void NewBuilding(BuildingConstructDelay newBuilding)
    {
        spawnedBuilding.Add(newBuilding);
    }
    public void StageOutBuilding(BuildingConstructDelay fadeBuilding)
    {
        spawnedBuilding.Remove(fadeBuilding);
    }
    #endregion
}
