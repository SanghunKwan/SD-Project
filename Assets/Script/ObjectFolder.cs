using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class ObjectFolder : MonoBehaviour
{
    [SerializeField] List<CObject> objects;

    private void OnEnable()
    {
        foreach (var cObject in objects)
            cObject.GetSelecting();

        objects.Clear();
        objects = null;

        Destroy(this);
    }
    private void Reset()
    {
        int length = transform.childCount;
        objects = new List<CObject>(length);

        for (int i = 0; i < length; i++)
            objects.Add(transform.GetChild(i).GetComponent<CObject>());
    }
}