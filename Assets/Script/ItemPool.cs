using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    Transform[] folders;
    [SerializeField] Transform itemTransform;


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

    public GameObject CallItem(int num, int stageIndex)
    {
        GameObject obj;
        if (transform.GetChild(num).childCount > 0)
        {
            obj = transform.GetChild(num).GetChild(0).gameObject;
            obj.transform.SetParent(itemTransform.GetChild(stageIndex));
        }
        else
        {
            obj = Instantiate(itemPrefabs[num], Vector3.zero, Quaternion.identity, itemTransform.GetChild(stageIndex));
        }

        return obj;
    }
    public void CheckPosition(GameObject itemObject)
    {
        Debug.DrawRay(itemObject.transform.position + Vector3.up, Vector3.down, Color.red, 2);
        if (Physics.Raycast(itemObject.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 4, 1 << 8))
            itemObject.transform.SetParent(itemTransform.GetChild(hit.collider.gameObject.transform.parent.parent.GetSiblingIndex()), true);
        else
            itemObject.gameObject.SetActive(false);
    }
    public void ReturnItem(int index, GameObject item)
    {
        item.transform.SetParent(transform.GetChild(index - 1));
        item.transform.eulerAngles = Vector3.zero;

        item.SetActive(false);
    }

    public void CallItems(SaveData.YetDroppedItem items)
    {
        if (items.type == SaveData.YetDroppedItem.DropType.Drop)
            StartCoroutine(DropRepeatAndDelay(items));
        else
            StartCoroutine(ThrowRepeatAndDelay(items));
    }
    IEnumerator DropRepeatAndDelay(SaveData.YetDroppedItem items)
    {
        int length = items.items.Count;
        for (int i = items.currentItemIndex; i < length; i++)
        {
            items.currentItemIndex = i;
            Debug.Log(items.items[items.currentItemIndex]);
            GameObject item = CallItem(items.items[items.currentItemIndex], items.stageIndex);
            item.SetActive(true);

            //회전각 계산.
            float yAngle = 360 * i / length;

            ////저장된 위치 + 회전된 방향으로 1만큼 이동.
            //item.transform.position = transform.position
            //        + Quaternion.Euler(0, 360 * i / DropInfo.MaxNum, 0) * Vector3.right * (ObjectCollider.radius + 0.5f);

            //회전
            item.transform.Rotate(0, yAngle, 0);

            //아이템 위치 선정
            item.transform.position = items.itemsPosition + Quaternion.Euler(0, yAngle, 0) * Vector3.right * items.offset;


            yield return new WaitForSeconds(0.1f);
        }

        GameManager.manager.objectManager.CompleteDropItemList(items.listIndex);
    }
    IEnumerator ThrowRepeatAndDelay(SaveData.YetDroppedItem items)
    {
        int length = items.items.Count;
        int tempLength = (length <= 1) ? length : (length - 1);
        for (int i = items.currentItemIndex; i < length; i++)
        {
            items.currentItemIndex = i;
            Debug.Log(items.items[items.currentItemIndex]);
            GameObject item = CallItem(items.items[items.currentItemIndex], items.stageIndex);
            item.SetActive(true);

            //회전각 계산.
            float yAngle = (30 * i / tempLength) - 15 + items.offset;

            //회전
            item.transform.Rotate(0, yAngle, 0);

            //아이템 위치 선정
            item.transform.position = items.itemsPosition + Quaternion.Euler(0, yAngle, 0) * Vector3.right;


            yield return new WaitForSeconds(0.1f);
        }

        GameManager.manager.objectManager.CompleteDropItemList(items.listIndex);
    }
}