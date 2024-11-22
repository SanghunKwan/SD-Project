using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unit;

public class InventoryManager : JsonLoad
{
    public static InventoryManager i;

    [SerializeField] Image clickImage;

    public Image InventoryDescription { get; private set; }

    [Serializable]
    public class itemInfo
    {
        public StorageComponent.Item[] items;
        public int[] prices;
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
    #region µ¿ÀÛ
    public void AddItem(in CUnit unit, int itemCode)
    {
        inventoryStorage.ItemCountChange(itemCode, 1);
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

    [ContextMenu("asjdflk")]
    void SER()
    {
        SDF<itemInfo>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
