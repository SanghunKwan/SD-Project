using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.UI.Extensions;


public enum ItemType
{
    NONE,
    Material,
    Consumption,
    EquipsHead,
    EquipsArm,
    EquipsWeapon,
    Special,

}
public class ItemComponent : MonoBehaviour
{
    [SerializeField] int index;
    public int Index { get { return index; } }


    Transform CircleCanvas;



    UICircle uiCircle;
    [SerializeField] Material[] materials;

    [SerializeField] int CirclePad;

    [SerializeField] float getSpeed;
    CapsuleCollider coll;
    bool isCorpse;
    UnitState unitState;

    private void Awake()
    {
        CircleCanvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        coll = GetComponent<CapsuleCollider>();
        isCorpse = transform.TryGetComponent(out UnitState state);
        unitState = state;
    }
    void OnEnable()
    {
        GameObject circleObjct = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UIItemCircle);
        circleObjct.transform.SetParent(CircleCanvas, false);
        uiCircle = circleObjct.GetComponent<UICircle>();
        if (isCorpse)
        {
            unitState.Death(false);
            StartCoroutine(DelayAniEnd());
        }
        StartCoroutine(CircleRepeat());
    }

    public void Init(int indexNum, Material[] _materials, int _circlePad, float Speed)
    {
        SetIndex(indexNum);
        materials = _materials;
        CirclePad = _circlePad;
        getSpeed = Speed;
    }

    IEnumerator CircleRepeat()
    {
        yield return new WaitWhile(() => index == 0);
        uiCircle.gameObject.SetActive(true);
        uiCircle.material = materials[(int)InventoryManager.i.info.items[index].type - 1];
        uiCircle.Arc = 0.1f;
        uiCircle.Padding = CirclePad;

        while (true)
        {

            uiCircle.rectTransform.position = transform.position;
            uiCircle.ArcRotation += 2;
            uiCircle.SetAllDirty();
            yield return null;
        }
    }

    public void AnimationEnd()
    {
        coll.enabled = true;
    }
    IEnumerator DelayAniEnd()
    {
        yield return new WaitForSeconds(1);
        AnimationEnd();
    }
    private void OnMouseEnter()
    {
        uiCircle.Arc = 1f;

    }
    private void OnMouseExit()
    {
        uiCircle.Arc = 0.1f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            InventoryAddItem(other.gameObject);
            uiCircle.transform.SetParent(ObjectUIPool.pool.transform.GetChild((int)ObjectUIPool.Folder.UIItemCircle));
            uiCircle.gameObject.SetActive(false);

            StartCoroutine(ItemGetAnim(other));
            coll.enabled = false;
        }
    }
    IEnumerator ItemGetAnim(Collider other)
    {
        Vector3 vec;
        float upSpeed = getSpeed;
        float time = 0;
        while (transform.position.y < 0.9f)
        {
            time += 0.1f;
            vec.x = (other.transform.position.x - transform.position.x);
            vec.y = 0;
            vec.z = (other.transform.position.z - transform.position.z);

            vec = vec.normalized * getSpeed * time + Vector3.up * (Mathf.SmoothDamp(transform.position.y, 1f, ref upSpeed, 0.1f) - transform.position.y);
            transform.position += vec;
            yield return null;
        }
        DropManager.instance.pool.ReturnItem(index, gameObject);
    }
    public void SetIndex(int indexNum)
    {
        index = indexNum;
    }
    void InventoryAddItem(in GameObject itemFinder)
    {
        GameManager.manager.storageManager.AddItem(index, 1, itemFinder);
    }

}
