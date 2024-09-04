using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unit;

public class loadingbar : MonoBehaviour
{
    public enum BARTYPE
    {
        HP,
        MENTALITY,
        MAX
    }
    [SerializeField] BARTYPE bartype;
    private Image imageComp;
    private Image imageCase;
    public float speed = 0.0f;
    int caseSize;
    int leastSize = 100;
    unit_status full_status;
    unit_status current_status;

    System.Action[] action = new System.Action[(int)BARTYPE.MAX];
    private void Awake()
    {
        imageComp = GetComponent<Image>();
        imageCase = transform.parent.GetComponent<Image>();

        action[0] = HPUpdate;
        action[1] = MENTALITYUpdate;
    }
    private void OnEnable()
    {
        imageCase.rectTransform.sizeDelta = new Vector2(caseSize, leastSize);
        action[(int)bartype]();
    }

    public void GetStatus(unit_status stat, unit_status curstat, int size)
    {
        full_status = stat;
        current_status = curstat;
        caseSize = size + leastSize;

    }
    void HPUpdate()
    {
        imageComp.rectTransform.sizeDelta = new Vector2((float)current_status.HP / full_status.HP * (caseSize - 2), leastSize);
    }
    void MENTALITYUpdate()
    {
        imageComp.rectTransform.sizeDelta = new Vector2((float)current_status.Mentality
                                                        / full_status.Mentality * (caseSize - 2), leastSize);
    }

    public void BarUpdate()
    {
        if (transform.parent.gameObject.activeSelf)
        {
            action[(int)bartype]();
        }
    }
}
