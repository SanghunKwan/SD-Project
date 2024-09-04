using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEManager : MonoBehaviour
{
    public static AOEManager manager;

    [SerializeField] Collider[] aoeCol;
    [SerializeField] int inItCount = 10;
    public int numDef { get; private set; }

    public Transform UICanvas { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        manager = this;

        for (int i = 0; i < aoeCol.Length; i++)
        {
            GameObject folder = new GameObject(aoeCol[i].name);
            folder.transform.SetParent(transform);

            for (int k = 0; k < inItCount; k++)
            {
                Instantiate(aoeCol[i], transform.GetChild(i));
            }
        }

        UICanvas = GameObject.FindWithTag("Canvas").transform;
        numDef = (int)ObjectUIPool.Folder.AOESwing - (int)SkillData.Range.SemiCircle;
    }

    public Collider Call(SkillData.Range type)
    {
        GameObject temp;
        if (transform.childCount > 0)
        {
            temp = transform.GetChild((int)type).GetChild(0).gameObject;
            temp.transform.SetParent(transform);
        }
        else
        {
            return Instantiate(aoeCol[(int)type], transform);
        }
        return temp.GetComponent<Collider>();
    }
}
