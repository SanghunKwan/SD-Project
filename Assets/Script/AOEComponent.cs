using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class AOEComponent : MonoBehaviour
{
    Collider aoeCollider;
    UIPrimitiveBase uiBase;

    int numDef;

    System.Action<Collider, Vector3> action;
    // Start is called before the first frame update
    void Awake()
    {
        aoeCollider = GetComponent<Collider>();
        numDef = AOEManager.manager.numDef;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        uiBase.rectTransform.position = transform.position;
    }
    public void Init(SkillData.Skill skill, float degree, System.Action<Collider, Vector3> Hit)
    {
        aoeCollider.transform.rotation = Quaternion.Euler(0, degree, 0);
        action = Hit;

        GameObject obj = ObjectUIPool.pool.Call((ObjectUIPool.Folder)((int)skill.type + numDef), ObjectUIPool.UICanvasType.GroundCanvas);
        obj.transform.localScale = Vector3.one * skill.rangeMultiply * 2.14f;

        uiBase = obj.GetComponent<UIPrimitiveBase>();
        uiBase.gameObject.SetActive(true);
        uiBase.rectTransform.rotation = Quaternion.Euler(uiBase.rectTransform.eulerAngles.x, 0, -degree);
    }
    public void Return(SkillData.Range type)
    {
        uiBase.transform.SetParent(ObjectUIPool.pool.transform.GetChild((int)type + numDef), false);
        uiBase.gameObject.SetActive(false);
        transform.SetParent(AOEManager.manager.transform.GetChild((int)type), false);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        action(other, transform.position);
    }
}
