using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(StorageComponent))]
public class InventoryComponent : InitObject, IStorageVisible, IPointerEnterHandler, IPointerExitHandler
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

        public void SetSlot(in InventoryStorage.Slot slot, InventoryType type, int i)
        {
            slotdata = slot;
            itemNumText.gameObject.SetActive(slotdata.itemCount > Convert.ToInt32(type != InventoryType.Store));
            itemNumText.text = "x" + slotdata.itemCount.ToString();

            if (InventoryManager.i.resourceSprite.ContainsKey(InventoryManager.i.info.items[slotdata.itemCode].name))
            {
                Sprite[] sprites = InventoryManager.i.resourceSprite[InventoryManager.i.info.items[slotdata.itemCode].name];
                itemImage.sprite = sprites[Mathf.Min(sprites.Length - 1, i)];
            }
            else
                itemImage.sprite = null;

            itemImage.color = Color.white * Convert.ToInt32(slotdata.itemCode != 0);
        }
    }

    ItemSlot[] itemSlots;
    static int nowSlotIndex;

    public InventoryStorage inventoryStorage { get; private set; }
    static InventoryComponent nowInventoryComponent;
    [SerializeField] InventoryDescription inventoryDescription;
    [SerializeField] CheckUICallChange[] uICallChange;
    public enum InventoryType
    {
        Stage,
        Villige,
        Store,
        Max
    }
    [SerializeField] InventoryType type;
    Action<int>[] useAction;
    Action<int, Vector3>[] throwAction;
    Action<int>[] swapAction;
    public override void Init()
    {
        inventoryStorage = GetComponent<InventoryStorage>();
        inventoryStorage.AddListener((slotIndex, forIndex) => OriginImage(slotIndex, forIndex));

        Transform slotParent = transform.GetChild(1);

        int slotCount = slotParent.childCount;
        itemSlots = new ItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            itemSlots[i] = new ItemSlot(slotParent.GetChild(i));
        }

        SetActions();

        inventoryStorage.SetType(type);
        inventoryStorage.Init();
    }
    void SetActions()
    {
        int withShit = ((int)InventoryType.Max) * 2;
        useAction = new Action<int>[withShit];
        useAction[(int)InventoryType.Stage] = StageUse;
        useAction[(int)InventoryType.Villige] = (slotIndex) => VilligeUse(slotIndex, InventoryType.Store, 1);
        useAction[(int)InventoryType.Store] = (slotIndex) => VilligeUse(slotIndex, InventoryType.Villige, 1);
        useAction[((int)InventoryType.Stage) + 3] = StageUse;
        useAction[((int)InventoryType.Villige) + 3] = (slotIndex) => VilligeUse(slotIndex, InventoryType.Store);
        useAction[((int)InventoryType.Store) + 3] = (slotIndex) => VilligeUse(slotIndex, InventoryType.Villige);

        throwAction = new Action<int, Vector3>[(int)InventoryType.Max];
        throwAction[(int)InventoryType.Stage] = ThrowInStage;
        throwAction[(int)InventoryType.Villige] = (slotIndex, data) => VilligeUse(slotIndex, InventoryType.Store);
        throwAction[(int)InventoryType.Store] = (slotIndex, data) => VilligeUse(slotIndex, InventoryType.Villige);

        swapAction = new Action<int>[2];
        swapAction[0] = ItemSwapWithOthers;
        swapAction[1] = ItemSwap;
    }
    #region 시각화
    void OriginImage(int slotIndex, int i = 0)
    {
        itemSlots[slotIndex].SetSlot(inventoryStorage.slots[slotIndex], type, i);
    }
    public void ActiveDescription(bool onoff)
    {
        inventoryDescription.SetActive(onoff);
    }
    public void SetDescription(int slotIndex, in Vector2 vector)
    {
        ActiveDescription(true);
        inventoryDescription.SetText(InventoryManager.i.info.items[itemSlots[slotIndex].slotdata.itemCode].description +
            inventoryStorage.ChangeingText(Slot2Code(slotIndex)));


        inventoryDescription.SetPosition(vector);
    }
    void OriginImagebySlotArray(in int[] brunchList, int offset)
    {
        int length = brunchList.Length;
        int byIndex;
        int addOffset;
        for (int i = 0; i < length; i++)
        {
            byIndex = brunchList[i];
            addOffset = byIndex + offset;
            OriginImage(addOffset, i);
        }
    }
    void OriginImagebySlotArray(in int[] brunchList)
    {
        int length = brunchList.Length;
        int byIndex;
        for (int i = 0; i < length; i++)
        {
            byIndex = brunchList[i];
            OriginImage(byIndex, i);
        }
    }
    #endregion
    #region input 요청
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
        target.transform.GetChild(0).gameObject.SetActive(false);
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
    public void ItemRemove(int slotIndex, in InventoryComponent inventoryComponent)
    {
        int[] removeSlots = inventoryComponent.inventoryStorage.slots[slotIndex].brunchIndex.ToArray();
        int length = removeSlots.Length;

        for (int i = 0; i < length; i++)
        {
            inventoryComponent.inventoryStorage.CheckCodetoSlot(removeSlots[i]);
            inventoryComponent.inventoryStorage.SetSlotEmpty(removeSlots[i]);
        }
    }
    public void DragEnd(int slotIndex, in PointerEventData eventData)
    {
        if (nowInventoryComponent is null)
            ThrowAway(slotIndex, eventData);
        else
            swapAction[Convert.ToInt32(nowSlotIndex < 0 || nowInventoryComponent == this)](slotIndex);
    }
    #region swap
    void ItemSwap(int slotIndex)
    {
        if (nowSlotIndex.Equals(slotIndex) || nowSlotIndex < 0)
        {
            ResetDrag(slotIndex);
            return;
        }

        int offset = nowSlotIndex - slotIndex;
        InventoryStorage.Slot slot = new InventoryStorage.Slot(inventoryStorage.slots[slotIndex]);
        Action callChangedSlots = () => { };

        int[] brunchIndexList = slot.brunchIndex.ToArray();
        int length = brunchIndexList.Length;

        if (inventoryStorage.IsEnoughSpace(slotIndex, offset))
        {
            ItemRemove(slotIndex, this);
            CheckBeforeItems(brunchIndexList, offset, slot.itemCode, slot.itemCount, ref callChangedSlots);
        }
        else
            offset = 0;
        callChangedSlots();
        OriginImagebySlotArray(brunchIndexList, offset);
    }
    void CheckBeforeItems(in int[] brunchIndexList, int offset, int itemCode, int itemCount, ref Action callChangedSlots)
    {
        int length = brunchIndexList.Length;
        int brunchOffset;
        int refItemCount = -itemCount;
        InventoryStorage.Slot memorySlot = new InventoryStorage.Slot();
        StorageComponent.Item item = InventoryManager.i.info.items[itemCode];
        Action callSavedItems = () => { };

        for (int i = 0; i < length; i++)
        {
            brunchOffset = brunchIndexList[i] + offset;
            if (CanBackUpItem(brunchOffset, ref memorySlot, ref callSavedItems, ref callChangedSlots))
                ItemRemove(brunchOffset, nowInventoryComponent);
        }
        nowInventoryComponent.inventoryStorage.ItemCountChangeBySlot(nowSlotIndex, itemCount, item);
        nowInventoryComponent.inventoryStorage.BaseChangeItemCount(itemCode, -itemCount);
        nowInventoryComponent.inventoryStorage.EmptySlotIndexRemove(nowSlotIndex, item.needSlots, item.figure);
        callSavedItems();
    }

    bool CanBackUpItem(int slotIndex, ref InventoryStorage.Slot slot, ref Action itemCount, ref Action imageSlot)
    {
        slot = nowInventoryComponent.inventoryStorage.slots[slotIndex];
        if (slot.itemCode == 0)
            return false;

        int code = slot.itemCode;
        int count = slot.itemCount;
        int[] brunchArray = slot.brunchIndex.ToArray();
        itemCount += () =>
        {
            nowInventoryComponent.inventoryStorage.ItemCountChange(code, count);
            nowInventoryComponent.inventoryStorage.BaseChangeItemCount(code, -count);
        };

        imageSlot += () => nowInventoryComponent.OriginImagebySlotArray(brunchArray);
        return true;
    }

    #endregion
    #region 버리기
    void ThrowAway(int slotIndex, in PointerEventData eventData)
    {
        throwAction[(int)type](slotIndex, eventData.position);
    }
    void ThrowInStage(int slotIndex, Vector3 eventPosition)
    {
        if (PlayerNavi.nav.lists.Count <= 0)
        {
            uICallChange[0].SetandCallOnce(3, true);
            ResetDrag(slotIndex);
        }
        else
        {
            InventoryStorage.Slot slot = inventoryStorage.slots[slotIndex];
            StorageComponent.Item item = InventoryManager.i.info.items[slot.itemCode];
            int addNum;
            UIEventSet(0, () =>
            {
                addNum = -slot.itemCount;
                GameManager.manager.storageManager.ThrowAwaySlotAll(eventPosition, slot);
                inventoryStorage.SlotOperateWithCodeIndex(slotIndex, ref addNum, item);
                //ItemRemove(slotIndex, this);
                uICallChange[1].CheckUICall.CallCheckUIRemove();
            });

            UIEventSet(1, () =>
            {
                addNum = -1;
                GameManager.manager.storageManager.ThrowAwaySlot(eventPosition, 1, slot.itemCode);
                inventoryStorage.SlotOperateWithCodeIndex(slotIndex, ref addNum, item);
                uICallChange[1].CheckUICall.CallCheckUIRemove();
            });

            UIEventSet(2, () =>
            {
                ResetDrag(slotIndex);
                uICallChange[1].CheckUICall.CallCheckUIRemove();
            });
        }
        uICallChange[1].SetandCallOnce(0);
    }
    void UIEventSet(int buttonIndex, in UnityAction action)
    {
        uICallChange[1].DataList[0].buttonEvents[buttonIndex].UiEvent.RemoveAllListeners();
        uICallChange[1].DataList[0].buttonEvents[buttonIndex].UiEvent.AddListener(action);
    }
    void ResetDrag(int slotIndex)
    {
        OriginImagebySlotArray(inventoryStorage.slots[slotIndex].brunchIndex.ToArray());
    }

    #endregion
    public void OnPointerEnter(int slotIndex)
    {
        nowInventoryComponent = this;
        nowSlotIndex = slotIndex;
    }
    #region click
    public void Use(int slotIndex)
    {
        int index = (Convert.ToInt32(Input.GetKey(GameManager.manager.shiftCode)) * (int)InventoryType.Max) + (int)type;
        useAction[index](slotIndex);
    }
    void VilligeUse(int slotIndex, InventoryType opersiteType, int moveCount = 0)
    {
        InventoryStorage.Slot slot = inventoryStorage.slots[slotIndex];
        int itemCode = slot.itemCode;
        if (moveCount == 0)
            moveCount = SlotMoveCount(slot, InventoryManager.i.info.items[itemCode]);

        //상속 받은 기능에 0 이하가 되면 가려지는 기능이 있음. 상점에 불필요.
        if (!inventoryStorage.ItemCountChangeByIndex(slotIndex, -moveCount, out _))
            return;

        GameManager.manager.storageManager.inventoryComponents(opersiteType).
            inventoryStorage.ItemCountChange(itemCode, moveCount);

        if (type == InventoryType.Villige)
            GameManager.manager.onItemUseOnExpedition.eventAction?.Invoke(itemCode, Vector3.zero);
        else
            GameManager.manager.onItemUseOnStore.eventAction?.Invoke(itemCode, Vector3.zero);
    }
    int SlotMoveCount(in InventoryStorage.Slot slot, in StorageComponent.Item item)
    {
        if (type != InventoryType.Store)
            return Mathf.Clamp(slot.itemCount, 0, item.MaxCount);
        else
            return item.MaxCount;
    }
    void StageUse(int slotIndex)
    {
        Character[] characters = PlayerNavi.nav.lists.ToArray();
        int heroCount = characters.Length;

        if (heroCount <= 0)
            return;

        InventoryStorage.Slot slot = inventoryStorage.slots[slotIndex];
        int itemCode = slot.itemCode;
        StorageComponent.Item item = InventoryManager.i.info.items[itemCode];

        if (item.type < ItemType.Consumption || item.type > ItemType.EquipsWeapon)
            return;

        inventoryStorage.ItemCountChangeByIndex(slotIndex, -heroCount, out float usedNum);
        //(usedNum / heroCount) 1인당 효과
        foreach (var character in characters)
        {
            inventoryStorage.AffectItem(character.cUnit, item, usedNum / heroCount);
        }
    }
    #endregion
    #region swap with class
    void ItemSwapWithOthers(int slotIndex)
    {
        InventoryStorage.Slot slot = inventoryStorage.slots[slotIndex];
        StorageComponent.Item item = InventoryManager.i.info.items[slot.itemCode];
        InventoryStorage.Slot slotTarget = nowInventoryComponent.inventoryStorage.slots[nowSlotIndex];


        int moveCount = Mathf.Clamp(slot.itemCount, 0, item.MaxCount - slotTarget.itemCount);
        if (moveCount <= 0)
        {
            moveCount = item.MaxCount - slotTarget.itemCount;
        }
        int[] brunchSlots = slot.brunchIndex.ToArray();


        if (nowInventoryComponent.type == InventoryType.Store
            || (slotTarget.itemCode != 0 && slot.itemCode != slotTarget.itemCode)
            || !inventoryStorage.IsEnoughSpaceWithOthers(inventoryStorage, slotIndex, nowInventoryComponent.inventoryStorage, nowSlotIndex))
        {
            nowInventoryComponent.inventoryStorage.ItemCountChange(slot.itemCode, moveCount);
            inventoryStorage.ItemCountChangeByIndex(slotIndex, -moveCount, out _);
        }
        else
        {
            //action 관련 삭제 가능?
            Action action = () => { };
            CheckBeforeItems(brunchSlots, nowSlotIndex - slotIndex, item.itemCode, moveCount, ref action);
            inventoryStorage.ItemCountChangeByIndex(slotIndex, -moveCount, out _);
            action();
        }
        nowInventoryComponent.OriginImagebySlotArray(slotTarget.brunchIndex.ToArray());
    }
    #endregion
    #endregion
    #region 사용금지
    public void ChangeNum(int itemCode)
    {
        throw new NotImplementedException();
    }


    #endregion
    #region pointer 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        nowInventoryComponent = this;
        nowSlotIndex = -1;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        nowInventoryComponent = null;
    }
    #endregion
}
