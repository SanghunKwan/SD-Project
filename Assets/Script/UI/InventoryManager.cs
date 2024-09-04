using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using Unit;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i;

    Image[] itemUI;
    Text[] UItext;
    [SerializeField] Image clickImage;
    int[] UItoroom;

    public Item[] inventoryRoom;

    TextMeshProUGUI tmg;
    public Image InventoryDescription { get; private set; }

    [Serializable]
    public struct Item
    {
        public string name;
        public int itemCode;
        public int itemCount;
        public int MaxCount;
        public ItemType type;
        public string description;
        public int HP;
        public SkillData.ItemSkillEffect itemSkillEffect;
    }
    [Serializable]
    public class itemInfo
    {
        public Item[] items;
    }

    public itemInfo info { get; private set; }

    List<int> roomToImage = new List<int>();
    IEnumerator descriptionCor;

    [SerializeField] PlayerNavi playerNavi;
    private void Awake()
    {
        i = this;


    }
    // Start is called before the first frame update
    void Start()
    {
        GetItemUI();
        string DataAddress = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/Item_Data.json"));

        info = (itemInfo)JsonUtility.FromJson(DataAddress, typeof(itemInfo));

        StartCoroutine(DesReplace());
    }
    IEnumerator DesReplace()
    {
        yield return new WaitUntil(() => info.items[info.items.Length - 1].description != "");
        for (int i = 0; i < info.items.Length; i++)
        {
            info.items[i].description = info.items[i].description.Replace("\\n", "\n");
        }
    }

    public void GetItemUI()
    {
        itemUI = transform.GetChild(1).GetComponentsInChildren<Image>();
        inventoryRoom = new Item[itemUI.Length];
        UItoroom = new int[itemUI.Length];
        UItext = new Text[itemUI.Length];

        for (int i = 0; i < itemUI.Length; i++)
        {
            UItext[i] = itemUI[i].transform.GetChild(0).GetComponent<Text>();
        }

        InventoryDescription = transform.GetChild(2).GetComponent<Image>();
        tmg = InventoryDescription.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void AddItem(CUnit unit, int itemCode)
    {
        int i = 0;
        int size = 1;
        if (itemCode == 12)
            size = 2;
        while (inventoryRoom[i].itemCode > 0
                && (inventoryRoom[i].itemCode != itemCode || inventoryRoom[i].itemCount == inventoryRoom[i].MaxCount))
        {
            i++;
            if (i >= inventoryRoom.Length - size + 1)
            {
                StartCoroutine(LayTrash(unit, itemCode - 1, 1, (UnityEngine.Random.Range(0, 2) * 2) - 1));
                Debug.Log("인벤토리 공간이 부족합니다.");
                return;
            }
        }
        SetRoom(i, size);
        ImageRenewal(i, size);

        void SetRoom(int roomNum, int size = 1)
        {
            int max = roomNum + size;
            for (int k = roomNum; k < max; k++)
            {
                inventoryRoom[k].itemCode = itemCode;
                inventoryRoom[k].name = info.items[itemCode].name;
                inventoryRoom[k].type = info.items[itemCode].type;
                inventoryRoom[k].MaxCount = info.items[itemCode].MaxCount;
                inventoryRoom[k].itemCount++;
                inventoryRoom[k].HP = info.items[itemCode].HP;
                inventoryRoom[k].itemSkillEffect = info.items[itemCode].itemSkillEffect;
            }
        }
    }

    void ImageRenewal(int roomNum, int size)
    {
        int i = 0;
        for (int j = 0; j < size; j++)
        {
            if (roomToImage.Count == roomNum)
            {
                while (itemUI[i].enabled.Equals(true))
                {
                    i++;
                }
                roomToImage.Add(i);
                UItoroom[i] = roomNum;
                itemUI[roomToImage[roomNum]].enabled = true;
                LoadImage(roomNum);
            }
            RenewalText(roomToImage[roomNum]);
            roomNum++;
        }
    }
    void LoadImage(int roomNum)
    {
        itemUI[roomToImage[roomNum]].sprite = Resources.Load<Sprite>("InventoryImage/2d" + inventoryRoom[roomNum].name.Replace(" ", ""));
    }
    void RenewalText(int UINum)
    {
        if (inventoryRoom[UItoroom[UINum]].itemCount <= 1 || itemUI[UINum].enabled.Equals(false))
            UItext[UINum].gameObject.SetActive(false);
        else
        {
            UItext[UINum].gameObject.SetActive(true);
            UItext[UINum].text = "x" + inventoryRoom[UItoroom[UINum]].itemCount;
        }
    }

    public void ThrowAway(CUnit cunit, int i, Vector3 vec)
    {
        int Code = inventoryRoom[UItoroom[i]].itemCode;
        int time = inventoryRoom[UItoroom[i]].itemCount;
        if (Code == 12)
        {
            int sameCode = 0;
            while (inventoryRoom[sameCode].itemCode != 12 || sameCode == UItoroom[i])
            {
                sameCode++;
            }
            Erase(roomToImage[sameCode]);
        }
        Vector3 offset = cunit.transform.position - vec;

        float angle = (Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg * (-1) + 270) % 360;

        int result = 0;
        if (angle < cunit.transform.eulerAngles.y || angle > 180 + cunit.transform.eulerAngles.y)
            result = 180;


        StartCoroutine(LayTrash(cunit, Code - 1, time, result));
        Erase(i);

    }
    IEnumerator LayTrash(CUnit cunit, int itemNum, int time, int oppoAngle)
    {
        float angle = oppoAngle + cunit.transform.eulerAngles.y;

        CallItem(cunit.transform, angle, out Transform outform);

        int callTime = Mathf.Min(time, 5);



        for (int j = 1; j < callTime; j++)
        {
            yield return new WaitForSeconds(0.1f);
            CallItem(outform, angle - 90 + j * 180 / (callTime - 1), out _);
        }


        if (time > 5)
        {
            StartCoroutine(LayTrash(cunit, itemNum, time - 5, oppoAngle + 180));
        }

        void CallItem(Transform tran, float angle, out Transform outform)
        {

            GameObject item = DropManager.instance.pool.CallItem(itemNum);
            outform = item.transform;
            item.transform.position = tran.position + Quaternion.Euler(0, angle, 0) * Vector3.right;
            item.transform.Rotate(Vector3.up, angle);
            item.SetActive(true);

        }
    }
    void Erase(int i)
    {
        itemUI[i].enabled = false;
        RenewalText(i);
        int j = UItoroom[i];
        int lastNum = roomToImage.Count - 1;

        while (j < lastNum && !inventoryRoom[j].itemCode.Equals(info.items[0].itemCode))
        {
            inventoryRoom[j] = inventoryRoom[j + 1];
            roomToImage[j] = roomToImage[j + 1];
            UItoroom[roomToImage[j]]--;
            j++;
        }
        inventoryRoom[j] = info.items[0];

        roomToImage.RemoveAt(roomToImage.Count - 1);
    }

    public void ItemSwap(int i1, int i2)
    {
        int i = UItoroom[i1];
        UItoroom[i1] = UItoroom[i2];
        UItoroom[i2] = i;

        bool onoff = itemUI[i1].enabled;
        itemUI[i1].enabled = itemUI[i2].enabled;
        itemUI[i2].enabled = onoff;

        roomToImage[UItoroom[i1]] = i1;
        if (itemUI[i2].enabled.Equals(true))
            roomToImage[UItoroom[i2]] = i2;

        LoadImage(UItoroom[i1]);
        LoadImage(UItoroom[i2]);

        RenewalText(i1);
        RenewalText(i2);
    }
    public void Use(int i)
    {
        ItemEffectApply(i);
        ItemCheckRemaining(i);
    }
    public void UseOne(CUnit unit, int i)
    {
        if (inventoryRoom[UItoroom[i]].type != ItemType.Consumption)
            return;

        UseItem(unit, ref inventoryRoom[UItoroom[i]]);
        ItemCheckRemaining(i);
    }
    void ItemCheckRemaining(int i)
    {
        RenewalText(i);
        if (inventoryRoom[UItoroom[i]].itemCount <= 0)
            Erase(i);
    }

    private void ItemEffectApply(int i)
    {
        if (inventoryRoom[UItoroom[i]].type != ItemType.Consumption)
            return;

        int listNum = 0;
        while (inventoryRoom[UItoroom[i]].itemCount > 0 && playerNavi.lists.Count > listNum)
        {
            UseItem(playerNavi.lists[listNum].cUnit, ref inventoryRoom[UItoroom[i]]);
            listNum++;
        }
    }
    void UseItem(CUnit unit, ref Item item)
    {
        item.itemCount--;
        unit.Recovery(item.HP);
        //스킬, status 기능은 이후 추가
    }

    public bool CheckSlot(int i)
    {
        return itemUI[i].enabled;
    }

    public Image HideSlot(int i, Vector2 pressedPosition, out Vector2 ImageOffset, out int slot2)
    {
        Sprite spriteSaved = itemUI[i].sprite;
        itemUI[i].enabled = false;

        Image onClick = Instantiate(clickImage, transform.parent);
        onClick.sprite = spriteSaved;
        ImageOffset = pressedPosition - new Vector2(itemUI[i].rectTransform.position.x, itemUI[i].rectTransform.position.y);
        slot2 = i;

        return onClick;
    }
    public void ReturnItem(int i)
    {
        itemUI[i].enabled = true;
    }
    public void CallItemInfo(int i)
    {
        if (descriptionCor != default)
            StopCoroutine(descriptionCor);
        descriptionCor = ItemInfoOnOff(true);
        StartCoroutine(descriptionCor);

        RectTransform rec = tmg.transform.parent.GetComponent<RectTransform>();
        rec.position = itemUI[i].rectTransform.position + Vector3.left * 40 + Vector3.up * 20;

        int code = inventoryRoom[UItoroom[i]].itemCode;
        tmg.text = info.items[code].description;

    }
    IEnumerator ItemInfoOnOff(bool onoff)
    {
        tmg.transform.parent.gameObject.SetActive(true);
        Image image = tmg.transform.parent.GetComponent<Image>();

        Color addColor = new Color(0, 0, 0, 0.05f);
        if (!onoff)
            addColor.a -= 0.1f;

        do
        {
            tmg.color += addColor;
            image.color += addColor;

            yield return null;
        } while (tmg.color.a < 1 && tmg.color.a > 0);

        tmg.transform.parent.gameObject.SetActive(onoff);
        descriptionCor = default;
    }
    public void OffItemInfo()
    {
        if (!InventoryDescription.gameObject.activeSelf)
            return;
        if (descriptionCor != null)
            StopCoroutine(descriptionCor);
        descriptionCor = ItemInfoOnOff(false);
        StartCoroutine(descriptionCor);
    }
}
