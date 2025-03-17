using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    Transform[] folders;


    // Start is called before the first frame update
    void Start()
    {
        int length = itemPrefabs.Length;
        folders = new Transform[length];
        for (int i = 0; i < length; i++)
        {
            if (i >= 11)
            {
                folders[i] = new GameObject("Item_Corpse").transform;
                folders[i].SetParent(transform);
                continue;
            }
            else if (i == 12)
            {
                folders[i] = new GameObject("Coin").transform;
                folders[i].SetParent(transform);
                continue;
            }

            folders[i] = new GameObject(itemPrefabs[i].name).transform;
            folders[i].SetParent(transform);
            for (int j = 0; j < 10; j++)
            {
                Instantiate(itemPrefabs[i], Vector3.zero, Quaternion.identity, folders[i]);
            }
        }
    }

    public GameObject CallItem(int num)
    {
        GameObject obj;
        if (transform.GetChild(num).childCount > 0)
        {
            obj = transform.GetChild(num).GetChild(0).gameObject;
            obj.transform.SetParent(transform);
        }
        else
        {
            obj = Instantiate(itemPrefabs[num], Vector3.zero, Quaternion.identity, transform);
        }
        return obj;
    }

    public void ReturnItem(int index, GameObject item)
    {
        item.transform.SetParent(transform.GetChild(index - 1));
        item.transform.eulerAngles = Vector3.zero;

        item.SetActive(false);
    }
}