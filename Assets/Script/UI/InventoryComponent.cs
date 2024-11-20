using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StorageComponent))]
public class InventoryComponent : InitObject, IStorageVisible
{
    public class ItemSlot
    {
        public Image itemImage { get; private set; }
        public Text itemNumText { get; private set; }
        public InventoryStorage.Slot slotdata;

        public ItemSlot(Transform transform)
        {
            itemImage = transform.GetComponent<Image>();
            itemNumText = transform.GetChild(0).GetComponent<Text>();
            slotdata = new InventoryStorage.Slot();
        }

        public void SetSlot(in InventoryStorage.Slot slot)
        {
            slotdata = slot;
            itemNumText.gameObject.SetActive(slotdata.itemCount > 1);
            itemNumText.text = "x" + slotdata.itemCount.ToString();
            itemImage.sprite = Resources.Load<Sprite>("InventoryImage/2d" + InventoryManager.i.info.items[slotdata.itemCode].name.Replace(" ", ""));
            itemImage.color = Color.white * Convert.ToInt32(slotdata.itemCode != 0);
        }
    }

    ItemSlot[] itemSlots;
    int nowSlotIndex;

    InventoryStorage inventoryStorage;
    [SerializeField] InventoryDescription inventoryDescription;

    enum InventoryType
    {
        Stage,
        Villige,
        Store,
        Max
    }
    [SerializeField] InventoryType type;
    Action<int>[] useAction;
    public override void Init()
    {
        inventoryStorage = GetComponent<InventoryStorage>();
        inventoryStorage.Init();
        inventoryStorage.AddListener(OriginImage);

        Transform slotParent = transform.GetChild(1);

        int slotCount = slotParent.childCount;
        itemSlots = new ItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            itemSlots[i] = new ItemSlot(slotParent.GetChild(i));
        }
        Debug.Log("�ӽ� ������ �߰�");
        inventoryStorage.ItemCountChange(1, 1);

        useAction = new Action<int>[(int)InventoryType.Max];
        useAction[(int)InventoryType.Stage] = StageUse;
        useAction[(int)InventoryType.Villige] = VilligeUse;
        useAction[(int)InventoryType.Store] = StoreUse;
    }
    #region �ð�ȭ
    void OriginImage(int slotIndex)
    {
        foreach (var item in inventoryStorage.slots[slotIndex].brunchIndex)
            itemSlots[item].SetSlot(inventoryStorage.slots[item]);
    }
    public void ActiveDescription(bool onoff)
    {
        inventoryDescription.SetActive(onoff);
    }
    public void SetDescription(int slotIndex, in Vector2 vector)
    {
        ActiveDescription(true);
        inventoryDescription.SetText(InventoryManager.i.info.items[itemSlots[slotIndex].slotdata.itemCode].description);
        inventoryDescription.SetPosition(vector);
    }
    #endregion
    #region input ��û
    int Slot2Code(int slotIndex)
    {
        return itemSlots[slotIndex].slotdata.itemCode;
    }
    public bool CheckItemSlotExist(int slotIndex)
    {
        return Slot2Code(slotIndex) != 0;
    }
    public RectTransform GetDragImage(int slotIndex)
    {
        GameObject dragImage = new GameObject("DragObject", typeof(RectTransform));
        dragImage.transform.SetParent(inventoryDescription.transform.parent);
        RectTransform returnTransform = (RectTransform)dragImage.transform;
        Image targetImage = itemSlots[slotIndex].itemImage;
        Image image = BrunchImagesSetting(slotIndex, dragImage.transform);

        CopyTransformChild(image.transform, targetImage.transform, 0);

        return returnTransform;
    }
    Image BrunchImagesSetting(int slotIndex, in Transform dragTransform)
    {
        List<int> brunchList = itemSlots[slotIndex].slotdata.brunchIndex;
        int brunchNum = brunchList.Count;
        Image[] images = new Image[brunchNum];
        for (int i = 0; i < brunchNum; i++)
        {
            GameObject tempObject = new GameObject();
            tempObject.transform.SetParent(dragTransform);
            images[i] = tempObject.AddComponent<Image>();
            ImageCopyNSetting(images[i], itemSlots[brunchList[i]].itemImage);
        }
        return images[brunchList.IndexOf(slotIndex)];
    }
    void ImageCopyNSetting(in Image copy, in Image target)
    {
        ImageCopy(copy, target);
        copy.raycastTarget = false;
        target.color = Color.clear;
    }
    void ImageCopy(in Image copy, in Image target)
    {
        copy.sprite = target.sprite;
        copy.rectTransform.sizeDelta = target.rectTransform.sizeDelta;
        copy.rectTransform.position = target.rectTransform.position;
    }
    void CopyTransformChild(in Transform objectTransform, in Transform targetTransform, int childNum)
    {
        Transform targetChildTransform = targetTransform.GetChild(childNum);
        GameObject newGameObject = Instantiate(targetChildTransform.gameObject);

        newGameObject.transform.SetParent(objectTransform);
        newGameObject.transform.localPosition = targetChildTransform.localPosition;
        targetChildTransform.gameObject.SetActive(false);
    }
    public void ItemRemove(int slotIndex)
    {
        int[] removeSlots = itemSlots[slotIndex].slotdata.brunchIndex.ToArray();
        int length = removeSlots.Length;
        inventoryStorage.CheckCodetoSlot(slotIndex);
        for (int i = 0; i < length; i++)
        {
            inventoryStorage.SetSlotEmpty(removeSlots[i]);
        }
    }
    public void ItemSwap(int slotIndex)
    {
        int offset = nowSlotIndex - slotIndex;
        InventoryStorage.Slot slot = new InventoryStorage.Slot(itemSlots[slotIndex].slotdata);
        InventoryStorage.Slot memorySlot = new InventoryStorage.Slot();
        int[] brunchIndexList = slot.brunchIndex.ToArray();
        int length = brunchIndexList.Length;
        int brunchOffset;
        Action callSavedItems = () => { };
        Action callChangedSlots = () => { };

        if (inventoryStorage.IsEnoughSpace(slotIndex, offset))
        {
            ItemRemove(slotIndex);

            for (int i = 0; i < length; i++)
            {
                brunchOffset = brunchIndexList[i] + offset;
                if (CanBackUpItem(brunchOffset, ref memorySlot, ref callSavedItems, ref callChangedSlots))
                    ItemRemove(brunchOffset);
            }
            inventoryStorage.SetSlot(slot, offset);
            callSavedItems();
        }
        else
            offset = 0;
        OriginImagebySlot(brunchIndexList, offset);
        callChangedSlots();
    }
    bool CanBackUpItem(int slotIndex, ref InventoryStorage.Slot slot, ref Action itemCount, ref Action imageSlot)
    {
        slot = inventoryStorage.slots[slotIndex];
        if (slot.itemCode == 0)
            return false;

        int code = slot.itemCode;
        int count = slot.itemCount;
        int[] brunchArray = slot.brunchIndex.ToArray();
        itemCount += () => inventoryStorage.ItemCountChange(code, count);
        imageSlot += () => OriginImagebySlot(brunchArray);
        return true;
    }
    void OriginImagebySlot(in int[] brunchList, int offset)
    {
        int length = brunchList.Length;
        int byIndex;
        int addOffset;
        for (int i = 0; i < length; i++)
        {
            byIndex = brunchList[i];
            addOffset = byIndex + offset;
            itemSlots[byIndex].SetSlot(inventoryStorage.slots[byIndex]);
            itemSlots[addOffset].SetSlot(inventoryStorage.slots[addOffset]);
        }
    }
    void OriginImagebySlot(in int[] brunchList)
    {
        int length = brunchList.Length;
        int byIndex;
        for (int i = 0; i < length; i++)
        {
            byIndex = brunchList[i];
            itemSlots[byIndex].SetSlot(inventoryStorage.slots[byIndex]);
        }
    }
    public void OnPointerEnter(int slotIndex)
    {
        nowSlotIndex = slotIndex;
    }
    public void Use(int slotIndex)
    {
        useAction[(int)type](slotIndex);
        //������������
        //��� �������� �ϳ��� ���

        //��������
        //â�� �κ��丮�� �̵�


    }
    void VilligeUse(int slotIndex)
    {
        //������ ����
        ItemRemove(slotIndex);
        //storage ���� ����

        //store�� ���� �߰�
        //�̹���ȭ
    }
    void StoreUse(int slotIndex)
    {
        //���� ����
        //������ -�� �Ǹ� �� ����.
        //villige�� ���� �߰�
        //�̹���ȭ
    }
    void StageUse(int slotIndex)
    {
        InventoryStorage.Slot slot = inventoryStorage.slots[slotIndex];
        //nav���� �������� ���� ����ŭ ���� ����
        int heroCount = PlayerNavi.nav.lists.Count;
        inventoryStorage.ItemCountChangeByIndex(slotIndex, -heroCount, out int usedNum);
        //������ �� / ���� �� * ȿ���� ����. ���� �����ϸ� ��ü���� ȿ���� �������� ����.
        Debug.Log("������ ���" + usedNum);
        //�̹���ȭ
        OriginImagebySlot(slot.brunchIndex.ToArray());
        Debug.Log("���� ������ ��" + slot.itemCount);
    }

    #endregion
    #region ������
    public void ChangeNum(int itemCode)
    {
        throw new NotImplementedException();
    }
    #endregion
}
