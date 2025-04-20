using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.UI;

public class WindowInfo : tempMenuWindow
{
    public WindowStatus wStatus { get; private set; }

    Image profileImg;
    NameChange nameChange;
    LevelChange level;
    QuirkChange quirk;
    QuirkChange disease;
    ActionAlert action;

    UpgradePreview[] upgradePreviews;
    int upgradePreivewCount = 2;
    IEnumerator turning;


    [SerializeField] Sprite[] profileSprites;
    [SerializeField] Vector2[] profileVec;
    [SerializeField] Vector2[] profileSizeDelta;
    [SerializeField] Camera characterCam;

    private void Awake()
    {
        profileImg = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        nameChange = transform.GetChild(1).GetComponent<NameChange>();
        level = transform.GetChild(2).GetComponent<LevelChange>();
        quirk = transform.GetChild(4).GetComponent<QuirkChange>();
        disease = transform.GetChild(5).GetComponent<QuirkChange>();
        action = transform.GetChild(6).GetComponent<ActionAlert>();

        wStatus = new WindowStatus();
        wStatus.Init(transform.GetChild(7).GetChild(2), 6, 1);

        upgradePreviews = new UpgradePreview[upgradePreivewCount];
        for (int i = 0; i < upgradePreivewCount; i++)
        {
            upgradePreviews[i] = transform.GetChild(8 + i).GetComponent<UpgradePreview>();
        }
    }
    //이름 i 버튼 누르면 수정 가능.
    public void VilligeWindowOpen(Hero hero)
    {
        wStatus.GetStatus(hero);
        wStatus.AlloStatus();

        TypeNum type = hero.stat.type;
        profileImg.sprite = profileSprites[(int)type];
        profileImg.rectTransform.anchoredPosition = profileVec[(int)type];
        profileImg.rectTransform.sizeDelta = profileSizeDelta[(int)type];


        nameChange.GetName(hero);
        level.GetLevel(hero.lv);
        quirk.SetQuirk(hero.quirks);
        disease.SetQuirk(hero.disease);
        action.ChangeAction(hero.VilligeAction, hero.BuildingAction);

        for (int i = 0; i < upgradePreivewCount; i++)
        {
            upgradePreviews[i].ActivePreview(hero);
        }
    }

    public void SetCam(bool active, Hero hero)
    {
        SetCam(active, hero.transform);
    }
    IEnumerator SemiAnim()
    {
        float angle = 25f * Time.deltaTime;
        float radian = Mathf.Deg2Rad * angle;

        while (characterCam.gameObject.activeSelf)
        {
            characterCam.transform.RotateAround(characterCam.transform.parent.position, Vector3.up, angle);
            yield return null;
        }
    }
    void SetLayerQueue(Transform trParent, int layerNum)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(trParent.GetChild(0));
        Transform current;
        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            current.gameObject.layer = layerNum;

            for (int i = 0; i < current.childCount; i++)
            {
                queue.Enqueue(current.GetChild(i));
            }
        }
    }
    public void VilligeWindowOpen(SummonHeroNameTag nameTag)
    {
        SaveData.HeroData hero = nameTag.heroData;
        wStatus.AlloStatus(hero.unitData.objectData.cur_status);

        TypeNum type = hero.unitData.objectData.cur_status.type;
        profileImg.sprite = profileSprites[(int)type];
        profileImg.rectTransform.anchoredPosition = profileVec[(int)type];
        profileImg.rectTransform.sizeDelta = profileSizeDelta[(int)type];


        nameChange.GetName(nameTag);
        level.GetLevel(nameTag.heroData.lv);
        quirk.SetQuirk(hero.quirks);
        disease.SetQuirk(hero.disease);
        action.ChangeAction((ActionAlert.ActionType)hero.villigeAction,
                            (AddressableManager.BuildingImage)hero.workBuilding);

        for (int i = 0; i < upgradePreivewCount; i++)
        {
            upgradePreviews[i].ActivePreview(hero);
        }
    }
    public void SetCam(bool active, Transform heroTransform)
    {
        characterCam.gameObject.SetActive(active);
        Transform prevTransform = characterCam.transform.parent;
        characterCam.transform.SetParent(heroTransform, false);
        characterCam.transform.localPosition = new Vector3(0, 1, 2);
        characterCam.transform.localEulerAngles = new Vector3(10, 180, 0);


        if (active)
        {
            if (turning != null)
            {
                SetLayerQueue(prevTransform, 7);
                StopCoroutine(turning);
            }
            SetLayerQueue(heroTransform, 17);

            turning = SemiAnim();
            StartCoroutine(turning);
        }
    }
}
