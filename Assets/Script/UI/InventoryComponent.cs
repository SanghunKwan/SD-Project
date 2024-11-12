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
        Debug.Log("임시 아이템 추가");
        inventoryStorage.ItemCountChange(1, 1);

    }
    #region 시각화
    void OriginImage(int slotIndex)
    {
        itemSlots[slotIndex].SetSlot(inventoryStorage.slots[slotIndex]);
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
    #region input 요청
    int Slot2Code(int slotIndex)
    {
        return itemSlots[slotIndex].slotdata.itemCode;
    }
    public bool CheckItemSlotExist(int slotIndex)
    {
        return Slot2Code(slotIndex) != 0;
    }
    public Image GetDragImage(int slotIndex)
    {
        GameObject dragImage = new GameObject("DragImage");
        dragImage.transform.SetParent(inventoryDescription.transform.parent);
        Image image = dragImage.AddComponent<Image>();
        ImageCopyNSetting(image, itemSlots[slotIndex].itemImage);

        return image;
    }
    void ImageCopy(in Image copy, in Image target)
    {
        copy.sprite = target.sprite;
        copy.rectTransform.sizeDelta = target.rectTransform.sizeDelta;
        copy.rectTransform.position = target.rectTransform.position;
    }
    void ImageCopyNSetting(in Image copy, in Image target)
    {
        ImageCopy(copy, target);
        copy.raycastTarget = false;
        target.color = Color.clear;
    }
    public void ItemSwap(int slotIndex)
    {
        inventoryStorage.SwapSlot(slotIndex, nowSlotIndex);
    }
    public void OnPointerEnter(int slotIndex)
    {
        nowSlotIndex = slotIndex;
    }
    #endregion
    #region 사용금지
    public void ChangeNum(int itemCode)
    {
        throw new NotImplementedException();
    }
    #endregion
}
