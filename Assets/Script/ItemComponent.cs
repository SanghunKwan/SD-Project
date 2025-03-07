using System.Collections;
using System.Collections.Generic;
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
    Corpse,
    Max
}
public class ItemComponent : MonoBehaviour
{
    [SerializeField] int index;
    public int Index { get { return index; } }

    UICircle uiCircle;
    [SerializeField] protected int CirclePad;

    [SerializeField] protected float getSpeed;
    CapsuleCollider coll;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider>();
    }
    void OnEnable()
    {
        GameObject circleObjct = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UIItemCircle,
                                                        ObjectUIPool.UICanvasType.GroundCanvas);
        uiCircle = circleObjct.GetComponent<UICircle>();

        VirtualEnable();

        StartCoroutine(CircleRepeat());
    }
    protected virtual void VirtualEnable()
    {
        GameManager.manager.battleClearManager.NewItem(this);
    }

    IEnumerator CircleRepeat()
    {
        yield return new WaitWhile(() => index == 0);
        uiCircle.gameObject.SetActive(true);
        uiCircle.material = DropManager.instance.Materials[(int)InventoryManager.i.info.items[index].type - 1];
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
        StageOutItem();
    }
    protected virtual void StageOutItem()
    {
        GameManager.manager.battleClearManager.StageOutItem(this);
    }
    public void SetIndex(int indexNum)
    {
        index = indexNum;
    }
    protected virtual void InventoryAddItem(in GameObject itemFinder)
    {
        GameManager.manager.storageManager.AddItem(index, 1, itemFinder);
    }

}
