using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNumChange : MonoBehaviour
{
    [SerializeField] StorageComponent storage;
    void Start()
    {
        StartCoroutine(asdkf());
    }
    IEnumerator asdkf()
    {
        yield return new WaitForSeconds(1);
        storage.SetItems();
    }
}
