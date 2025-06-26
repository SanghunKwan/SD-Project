using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public enum CObjectType
    {
        Hero,
        Monster,
        FieldObject,
        Max
    }
    public enum AdditionalType
    {
        Item,
        Building,
        Max
    }
    [SerializeField] CObjectType[] needList;
    [SerializeField] AdditionalType[] additionalList;

    LinkedList<CObject>[] objectList;
    LinkedList<MonoBehaviour>[] noneObjectList;
    public IReadOnlyList<LinkedList<CObject>> ObjectList => objectList;
    public IReadOnlyList<LinkedList<MonoBehaviour>> NoneObjectList => noneObjectList;

    Dictionary<GameObject, LinkedListNode<CObject>>[] objectDictionary;
    Dictionary<GameObject, LinkedListNode<MonoBehaviour>>[] noneObjectDictionary;
    public IReadOnlyDictionary<GameObject, LinkedListNode<CObject>>[] ObjectDictionary => objectDictionary;
    public IReadOnlyDictionary<GameObject, LinkedListNode<MonoBehaviour>>[] NoneObjectDictionary => noneObjectDictionary;

    Dictionary<GameObject, Monster> corpseDictinoary;
    public IReadOnlyDictionary<GameObject, Monster> CorpseDictionary => corpseDictinoary;

    List<SaveData.YetDroppedItem> yetDroppedItems;
    public IReadOnlyCollection<SaveData.YetDroppedItem> YetDroppedItems => yetDroppedItems;



    private void Awake()
    {
        int objectLength = (int)CObjectType.Max;
        int noneObjectLength = (int)AdditionalType.Max;
        objectList = new LinkedList<CObject>[objectLength];
        noneObjectList = new LinkedList<MonoBehaviour>[noneObjectLength];
        objectDictionary = new Dictionary<GameObject, LinkedListNode<CObject>>[objectLength];
        noneObjectDictionary = new Dictionary<GameObject, LinkedListNode<MonoBehaviour>>[noneObjectLength];
        corpseDictinoary = new Dictionary<GameObject, Monster>();
        yetDroppedItems = new List<SaveData.YetDroppedItem>();

        foreach (var item in needList)
        {
            objectList[(int)item] = new LinkedList<CObject>();
            objectDictionary[(int)item] = new Dictionary<GameObject, LinkedListNode<CObject>>();
        }

        foreach (var item in additionalList)
        {
            noneObjectList[(int)item] = new LinkedList<MonoBehaviour>();
            noneObjectDictionary[(int)item] = new Dictionary<GameObject, LinkedListNode<MonoBehaviour>>();
        }
    }
    private void Start()
    {
        GameManager.manager.objectManager = this;
    }


    #region cobject
    public void NewObject(CObjectType type, CObject target)
    {
        int index = (int)type;
        objectDictionary[index].Add(target.gameObject, objectList[index].AddLast(target));
    }
    public void OutObject(CObjectType type, GameObject targetGameObject)
    {
        int index = (int)type;
        Dictionary<GameObject, LinkedListNode<CObject>> dictionary = objectDictionary[index];

        objectList[index].Remove(dictionary[targetGameObject]);
        dictionary.Remove(targetGameObject);
    }
    public void NewCorpse(Monster targetObject)
    {
        corpseDictinoary.Add(targetObject.gameObject, targetObject);
    }

    public CObjectType GetCObjectType(int layerNum)
    {
        switch (layerNum)
        {
            case 7:
            case 17:
            case 18:
                return CObjectType.Hero;
            case 9:
                return CObjectType.FieldObject;
            case 10:
                return CObjectType.Monster;
            default:
                return CObjectType.Max;
        }
    }
    #endregion cobject

    #region Additional
    public void NewNoneObject(AdditionalType type, MonoBehaviour target)
    {
        int index = (int)type;
        noneObjectDictionary[index].Add(target.gameObject, noneObjectList[index].AddLast(target));
    }
    public void OutNoneObject(AdditionalType type, GameObject targetGameObject)
    {
        int index = (int)type;
        Dictionary<GameObject, LinkedListNode<MonoBehaviour>> dictionary = noneObjectDictionary[index];

        noneObjectList[index].Remove(dictionary[targetGameObject]);
        dictionary.Remove(targetGameObject);

        if (type == AdditionalType.Building)
            OutObject(CObjectType.FieldObject, targetGameObject);
    }
    #endregion Additional

    #region GetData
    public LinkedListNode<CObject> GetNode(CObject findObject)
    {
        return GetNode(findObject.gameObject);
    }
    public LinkedListNode<CObject> GetNode(GameObject findObject)
    {
        CObjectType dicArrayIndex = GetCObjectType(findObject.layer);
        if (dicArrayIndex == CObjectType.Max) return null;

        return objectDictionary[(int)dicArrayIndex][findObject];
    }
    #endregion GetData

    public SaveData.YetDroppedItem CreateItemList(DropInfo dropInfo, in Vector3 position, float colliderRadius, int stageIndex)
    {
        //dropInfo를 통해 생성할 아이템 개수 생성.
        SaveData.YetDroppedItem droppingItems = new SaveData.YetDroppedItem(dropInfo.MaxNum, stageIndex, position, colliderRadius);

        //아이템 좌표는 중심점에 저장.
        for (int i = 0; i < dropInfo.MaxNum; i++)
        {
            if (Random.Range(0, 100) > dropInfo.Percentage) continue;

            droppingItems.items.Add((int)dropInfo.material);
        }

        if (Random.Range(0f, 100f) < dropInfo.PercentageEquips)
        {
            //무기 드랍 코드
            droppingItems.items.Add((int)Materials.WeaponLevel2);
        }
        else
        {
            //소모품 드랍 코드
            droppingItems.items.Add(Random.Range((int)Materials.Berry, (int)Materials.GreenFruit + 1));
        }
        return droppingItems;
    }
    public void AddYetDroppedItem(SaveData.YetDroppedItem item)
    {
        item.listIndex = yetDroppedItems.Count;
        yetDroppedItems.Add(item);
    }

    public void CompleteDropItemList(int listIndex)
    {
        yetDroppedItems.RemoveAt(listIndex);
        int length = yetDroppedItems.Count;
        for (int i = listIndex; i < length; i++)
        {
            yetDroppedItems[i].listIndex = i;
        }
    }
}

