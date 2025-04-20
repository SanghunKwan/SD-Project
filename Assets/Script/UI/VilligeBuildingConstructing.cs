using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VilligeBuildingConstructing : MonoBehaviour
{
    Image quotaToday;
    Image quotaCurrent;
    RectTransform rectTransform;

    BuildingConstructDelay building;
    Animator anim;

    float timeAccumulate;
    public float timeNormalized { get; private set; }
    public static bool isReady { private get; set; }

    private void Awake()
    {
        quotaToday = transform.GetChild(0).GetComponent<Image>();
        quotaCurrent = transform.GetChild(1).GetComponent<Image>();
        rectTransform = transform as RectTransform;
    }

    public void SetTargetBuilding(BuildingConstructDelay buildingTarget, int dayRemaining, float timeNormalized)
    {
        building = buildingTarget;
        SetQuota(GetMaxQuotation(dayRemaining, timeNormalized), timeNormalized);
        anim = building.anim;
        anim.Update(0);
        timeAccumulate = timeNormalized * anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        enabled = true;
        rectTransform.localPosition = Unit.Data.Instance.CameratoCanvas(building.transform.position) + (Vector3.down * 0.5f);
        GameManager.manager.screenMove += ScreenMove;
    }
    void SetQuota(float dayQuota, float currentQuota)
    {
        quotaToday.fillAmount = dayQuota;
        quotaCurrent.fillAmount = currentQuota;
    }
    float GetMaxQuotation(int dayRemaining, float timeNormalized)
    {
        //Mathf.Floor(timeNormalized * (dayRemaining + 1))
        return Mathf.Floor(1 + timeNormalized * (dayRemaining + 1)) / (dayRemaining + 1);
    }
    private void Update()
    {
        if (!isReady || anim.speed == 0)
            return;

        timeAccumulate += Time.deltaTime;
        timeNormalized = timeAccumulate / anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        if (quotaToday.fillAmount < 1 && timeNormalized >= quotaToday.fillAmount)
            anim.speed = 0;
        else
            quotaCurrent.fillAmount = timeNormalized;

        if (timeNormalized >= 1)
            enabled = false;
    }
    void ScreenMove(Vector3 vector3)
    {
        rectTransform.localPosition -= vector3;
    }
    private void OnDisable()
    {
        building = null;
        anim.speed = 1;
        anim = null;
        ObjectUIPool.pool.BackPooling(gameObject, ObjectUIPool.Folder.VilligeConstructingUI);
        GameManager.manager.screenMove -= ScreenMove;
    }
}
