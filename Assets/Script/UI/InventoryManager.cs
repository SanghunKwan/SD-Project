using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unit;

public class InventoryManager : JsonLoad
{
    public static InventoryManager i;

    [SerializeField] Image clickImage;

    TextMeshProUGUI tmg;
    public Image InventoryDescription { get; private set; }

    [Serializable]
    public class itemInfo
    {
        public StorageComponent.Item[] items;
    }
    public itemInfo info { get; private set; }

    List<int> roomToImage = new List<int>();
    IEnumerator descriptionCor;

    [SerializeField] PlayerNavi playerNavi;
    [SerializeField] StorageComponent inventoryStorage;

    private void Awake()
    {
        i = this;

        info = LoadData<itemInfo>("Item_Data");
    }
    // Start is called before the first frame update
    void Start()
    {
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
    #region 동작
    public void AddItem(in CUnit unit, int itemCode)
    {
        inventoryStorage.ItemCountChange(itemCode, 1);
    }
    void Erase(int slotIndex, int num)
    {
    }
    #endregion
    public void ThrowAway(in CUnit cunit, int i, Vector3 vec)
    {
    //    if (Code == 12)
    //    {
    //        int sameCode = 0;
    //        while (inventoryRoom[sameCode].itemCode != 12 || sameCode == UItoroom[i])
    //        {
    //            sameCode++;
    //        }
    //        Erase(roomToImage[sameCode]);
    //    }
    //    Vector3 offset = cunit.transform.position - vec;

    //    float angle = (Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg * (-1) + 270) % 360;

    //    int result = 0;
    //    if (angle < cunit.transform.eulerAngles.y || angle > 180 + cunit.transform.eulerAngles.y)
    //        result = 180;


    //    StartCoroutine(LayTrash(cunit, Code - 1, time, result));
    //    Erase(i);

    }
    //IEnumerator LayTrash(CUnit cunit, int itemNum, int time, int oppoAngle)
    //{
    //    float angle = oppoAngle + cunit.transform.eulerAngles.y;

    //    CallItem(cunit.transform, angle, out Transform outform);

    //    int callTime = Mathf.Min(time, 5);



    //    for (int j = 1; j < callTime; j++)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        CallItem(outform, angle - 90 + j * 180 / (callTime - 1), out _);
    //    }


    //    if (time > 5)
    //    {
    //        StartCoroutine(LayTrash(cunit, itemNum, time - 5, oppoAngle + 180));
    //    }

    //    void CallItem(Transform tran, float angle, out Transform outform)
    //    {

    //        GameObject item = DropManager.instance.pool.CallItem(itemNum);
    //        outform = item.transform;
    //        item.transform.position = tran.position + Quaternion.Euler(0, angle, 0) * Vector3.right;
    //        item.transform.Rotate(Vector3.up, angle);
    //        item.SetActive(true);

    //    }
    //}
    //#endregion


    public void ItemSwap(int i1, int i2)
    {
    //    int i = UItoroom[i1];
    //    UItoroom[i1] = UItoroom[i2];
    //    UItoroom[i2] = i;

    //    bool onoff = itemUI[i1].enabled;
    //    itemUI[i1].enabled = itemUI[i2].enabled;
    //    itemUI[i2].enabled = onoff;

    //    roomToImage[UItoroom[i1]] = i1;
    //    if (itemUI[i2].enabled.Equals(true))
    //        roomToImage[UItoroom[i2]] = i2;

    //    LoadImage(UItoroom[i1]);
    //    LoadImage(UItoroom[i2]);

    //    RenewalText(i1);
    //    RenewalText(i2);
    }
    public void Use(int i)
    {
        //ItemEffectApply(i);
        //ItemCheckRemaining(i);
    }
    public void UseOne(CUnit unit, int i)
    {
    //    if (inventoryRoom[UItoroom[i]].type != ItemType.Consumption)
    //        return;

    //    UseItem(unit, ref inventoryRoom[UItoroom[i]]);
    //    ItemCheckRemaining(i);
    //}
    //void ItemCheckRemaining(int i)
    //{
    //    RenewalText(i);
    //    if (inventoryRoom[UItoroom[i]].itemCount <= 0)
    //        Erase(i);
    }

    //private void ItemEffectApply(int i)
    //{
    //    if (inventoryRoom[UItoroom[i]].type != ItemType.Consumption)
    //        return;

    //    int listNum = 0;
    //    while (inventoryRoom[UItoroom[i]].itemCount > 0 && playerNavi.lists.Count > listNum)
    //    {
    //        UseItem(playerNavi.lists[listNum].cUnit, ref inventoryRoom[UItoroom[i]]);
    //        listNum++;
    //    }
    //}
    //void UseItem(CUnit unit, ref Item item)
    //{
    //    item.itemCount--;
    //    unit.Recovery(item.HP);
    //    //스킬, status 기능은 이후 추가
    //}

    public bool CheckSlot(int i)
    {
        return true;// itemUI[i].enabled;
    }

    public Image HideSlot(int i, Vector2 pressedPosition, out Vector2 ImageOffset, out int slot2)
    {
        //    Sprite spriteSaved = itemUI[i].sprite;
        //    itemUI[i].enabled = false;

        //    Image onClick = Instantiate(clickImage, transform.parent);
        //    onClick.sprite = spriteSaved;
        //    ImageOffset = pressedPosition - new Vector2(itemUI[i].rectTransform.position.x, itemUI[i].rectTransform.position.y);
        ImageOffset = Vector2.zero;
        slot2 = i;

        //    return onClick;
        return null;
    }
    //IEnumerator ItemInfoOnOff(bool onoff)
    //{
    //    tmg.transform.parent.gameObject.SetActive(true);
    //    Image image = tmg.transform.parent.GetComponent<Image>();

    //    Color addColor = new Color(0, 0, 0, 0.05f);
    //    if (!onoff)
    //        addColor.a -= 0.1f;

    //    do
    //    {
    //        tmg.color += addColor;
    //        image.color += addColor;

    //        yield return null;
    //    } while (tmg.color.a < 1 && tmg.color.a > 0);

    //    tmg.transform.parent.gameObject.SetActive(onoff);
    //    descriptionCor = default;
    //}

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
