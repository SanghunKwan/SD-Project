using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StorageComponent))]
public class InventoryComponent : InitObject, IStorageVisible
{
    protected class ItemSlot
    {
        public Image itemImage;
        public Text itemNumText;
        public InventoryStorage.Slot slotdata;

        public ItemSlot(Transform transform)
        {
            itemImage = transform.GetComponent<Image>();
            itemNumText = transform.GetChild(0).GetComponent<Text>();
        }

        public void SetSlot(in InventoryStorage.Slot slot)
        {
            slotdata = slot;
            itemNumText.gameObject.SetActive(slotdata.itemCount > 1);
            itemNumText.text = "x" + slotdata.itemCount.ToString();
            itemImage.sprite = Resources.Load<Sprite>("InventoryImage/2d" + InventoryManager.i.info.items[slotdata.itemCode].name.Replace(" ", ""));

        }
    }
    ItemSlot[] itemSlots;

    InventoryStorage inventoryStorage;
    [SerializeField] InventoryDescription inventoryDescription;
    public override void Init()
    {
        inventoryStorage = GetComponent<InventoryStorage>();
        inventoryStorage.AddListener(OriginImage);

        Transform slotParent = transform.GetChild(1);

        int slotCount = slotParent.childCount;
        for (int i = 0; i < slotCount; i++)
        {
            itemSlots[i] = new ItemSlot(slotParent.GetChild(i));
        }
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
        inventoryDescription.SetText(InventoryManager.i.info.items[itemSlots[slotIndex].slotdata.itemCode].description);
        inventoryDescription.SetPosition(vector);
    }
    #endregion

    #region 사용금지
    public void ChangeNum(int itemCode)
    {
        throw new NotImplementedException();
    }
    #endregion
}
