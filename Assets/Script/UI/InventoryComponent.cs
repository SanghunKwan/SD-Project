using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Image targetImage = itemSlots[slotIndex].itemImage;
        ImageCopyNSetting(image, targetImage);
        CopyTransformChild(image.transform, targetImage.transform, 0);

        return image;
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
    public void ItemSwap(int slotIndex)
    {
        inventoryStorage.SwapSlot(slotIndex, nowSlotIndex);
    }
    public void OnPointerEnter(int slotIndex)
    {
        nowSlotIndex = slotIndex;
    }
    public void Use(int slotIndex)
    {
        //스테이지에서
        //모든 영웅들이 하나씩 사용

        //마을에서
        //창고 인벤토리로 이동
    }

    #endregion
    #region 사용금지
    public void ChangeNum(int itemCode)
    {
        throw new NotImplementedException();
    }
    #endregion
}
