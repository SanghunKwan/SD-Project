using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : JsonLoad
{
    public static InventoryManager i { get; private set; }

    [Serializable]
    public class itemInfo
    {
        public StorageComponent.Item[] items;
        public int[] prices;
    }
    public itemInfo info { get; private set; }

    private void Awake()
    {
        i = this;

        info = LoadData<itemInfo>("Item_Data");
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

    [ContextMenu("asjdflk")]
    void ContextMenu()
    {
        SDF<itemInfo>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
