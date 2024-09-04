using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.UI;

public class WindowInfo : MonoBehaviour
{
    public WindowStatus wStatus { get; private set; }

    Image profileImg;
    NameChange nameChange;
    LevelChange level;
    QuirkChange quirk;
    QuirkChange disease;
    ActionAlert action;
    SkillPreview skill;
    ItemPreview item;
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

        skill = transform.GetChild(8).GetComponent<SkillPreview>();
        item = transform.GetChild(9).GetComponent<ItemPreview>();
    }
    //이름 i 버튼 누르면 수정 가능.
    public void VilligeWindowOpen(Hero hero)
    {
        wStatus.GetStatus(hero);
        wStatus.AlloStatus();

        TypeNum type = Enum.Parse<TypeNum>(hero.stat.type);
        profileImg.sprite = profileSprites[(int)type];
        profileImg.rectTransform.anchoredPosition = profileVec[(int)type];
        profileImg.rectTransform.sizeDelta = profileSizeDelta[(int)type];


        nameChange.GetName(hero);
        level.GetLevel(hero.stat.Lv);
        quirk.SetQuirk(hero.quirks);
        disease.SetQuirk(hero.disease);
        action.ChangeAction(hero.VilligeAction);
        skill.ActivateSkillPreview(hero);
        item.ActiveItemPreview(hero);
    }

    public void SetCam(bool active, Hero hero)
    {
        characterCam.gameObject.SetActive(active);
        Transform prevTransform = characterCam.transform.parent;
        characterCam.transform.SetParent(hero.transform, false);
        characterCam.transform.localPosition = new Vector3(0, 1, 2);
        characterCam.transform.localEulerAngles = new Vector3(10, 180, 0);


        if (active)
        {
            if (turning != null)
            {
                SetLayerQueue(prevTransform, 7);
                StopCoroutine(turning);
            }
            SetLayerQueue(hero.transform, 17);

            turning = SemiAnim();
            StartCoroutine(turning);
        }
    }

    IEnumerator SemiAnim()
    {
        float angle = 25f * Time.deltaTime;
        float radian = Mathf.Deg2Rad * angle;

        while (characterCam.gameObject.activeSelf)
        {

            characterCam.transform.localEulerAngles += Vector3.up * angle;
            characterCam.transform.localPosition = new Vector3
                (characterCam.transform.localPosition.x * Mathf.Cos(radian) + Mathf.Sin(radian) * characterCam.transform.localPosition.z,
                1,
                -characterCam.transform.localPosition.x * Mathf.Sin(radian) + Mathf.Cos(radian) * characterCam.transform.localPosition.z);
            yield return null;
        }
    }
    void SetLayerQueue(Transform trParent, int layerNum)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(trParent.GetChild(0));

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            current.gameObject.layer = layerNum;

            for (int i = 0; i < current.childCount; i++)
            {
                queue.Enqueue(current.GetChild(i));
            }
        }


    }
}
