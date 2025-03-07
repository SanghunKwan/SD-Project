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

    private void Awake()
    {
        quotaToday = transform.GetChild(0).GetComponent<Image>();
        quotaCurrent = transform.GetChild(1).GetComponent<Image>();
        rectTransform = transform as RectTransform;
    }

    public void SetTargetBuilding(BuildingConstructDelay buildingTarget, int dayRemaining, float timeNormalized = 0)
    {
        building = buildingTarget;
        SetQuota(Mathf.Ceil(timeNormalized * (dayRemaining + 1)), timeNormalized);
        anim = building.anim;
        anim.Update(0);
        timeAccumulate = timeNormalized * anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        enabled = true;
    }
    void SetQuota(float dayQuota, float currentQuota)
    {
        quotaToday.fillAmount = dayQuota;
        quotaCurrent.fillAmount = currentQuota;
    }
    private void Update()
    {
        rectTransform.localPosition = Unit.Data.Instance.CameratoCanvas(building.transform.position) + (Vector3.down * 0.5f);

        if (anim.speed == 0)
            return;
        Debug.Log("시간 누적: " + timeAccumulate);
        timeAccumulate += Time.deltaTime;
        timeNormalized = timeAccumulate / anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        if (quotaToday.fillAmount < 1 && timeNormalized >= quotaToday.fillAmount)
            anim.speed = 0;
        else
            quotaCurrent.fillAmount = timeNormalized;

        if (timeNormalized >= 1)
            enabled = false;
    }
    private void OnDisable()
    {
        building = null;
        anim.speed = 1;
        anim = null;
        ObjectUIPool.pool.BackPooling(gameObject, ObjectUIPool.Folder.VilligeConstructingUI);
    }
}
