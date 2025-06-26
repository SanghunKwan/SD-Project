using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unit;
using System;

public class InputEffect : MonoBehaviour
{

    [SerializeField] GameObject[] effects;
    [SerializeField] ProjectileComponent[] projectileEffects;
    Queue<ProjectileComponent>[] projectileQueue;
    [SerializeField] int inItCount = 5;
    [SerializeField] int offset1 = 120;

    Transform canvas;



    public static InputEffect e;

    public enum WARNINGANIMTYPE
    {
        NONE,
        CANCEL,
        DIE,
        MAX
    }


    // Start is called before the first frame update
    void Start()
    {
        e = this;
        projectileQueue = new Queue<ProjectileComponent>[projectileEffects.Length];
        canvas = GameObject.FindWithTag("CanvasWorld").transform;
        GameObject effectFolder = new GameObject("effectFolder");
        GameObject projectileFolder = new GameObject("projectileFolder");
        effectFolder.transform.SetParent(transform);
        projectileFolder.transform.SetParent(transform);

        GameObject folder;
        for (int i = 0; i < effects.Length; i++)
        {
            folder = new GameObject(effects[i].name);
            folder.transform.SetParent(effectFolder.transform);

            for (int k = 0; k < inItCount; k++)
            {
                var effect = Instantiate(effects[i], folder.transform);
            }
        }
        for (int i = 0; i < projectileEffects.Length; i++)
        {
            folder = new GameObject(projectileEffects[i].name);
            folder.transform.SetParent(projectileFolder.transform);
            projectileQueue[i] = new Queue<ProjectileComponent>();
            for (int k = 0; k < inItCount; k++)
            {
                var effect = Instantiate(projectileEffects[i], folder.transform);
                projectileQueue[i].Enqueue(effect);
            }
        }
    }
    public void Callback(GameObject usedEffect, int _type)
    {

        if (_type.Equals(1) || _type.Equals(4))
        {
            StartCoroutine(Fade(usedEffect, _type));
        }
        else
        {
            usedEffect.transform.SetParent(transform.GetChild(0).GetChild(_type));
            usedEffect.SetActive(false);
        }

    }
    public void PrintEffect(Vector3 vec, int effectNum)
    {
        Transform folder = transform.GetChild(0).GetChild(effectNum);
        GameObject effect;

        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).gameObject;
            effect.transform.SetParent(folder.parent);
        }
        else
        {
            effect = Instantiate(effects[effectNum], folder.parent);
            effect.GetComponent<EffectReturn>().type = effectNum;
        }
        ReObject(effect, vec);
        if (effectNum.Equals(0))
        {
            effect.GetComponent<ParticleSystem>().Play();
        }
    }
    void ReObject(GameObject effect, Vector3 vec)
    {
        effect.SetActive(true);
        effect.transform.position = vec;
    }

    public Text PrintEffect2(Vector3 vec, string qltskrka = "ºø³ª°¨")
    {
        Transform folder = transform.GetChild(0).GetChild(1);
        Text effect;

        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).gameObject.GetComponent<Text>();
            effect.text = qltskrka;
            effect.transform.SetParent(canvas);
        }
        else
        {
            effect = Instantiate(effects[1], canvas).GetComponent<Text>();
            effect.GetComponent<EffectReturn>().type = 1;
        }
        ReUI(effect, vec);

        return effect;
    }
    IEnumerator Fade(GameObject gameObject, int type)
    {
        float end = Time.unscaledDeltaTime * 2;
        Color calColor = new Color(0, 0, 0, end);
        Color color;
        MaskableGraphic com;


        if (type.Equals(1))
        {
            Text text = gameObject.GetComponent<Text>();
            com = text;
            color = text.color;
        }
        else
        {
            TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
            com = text;
            color = text.color;
        }

        while (color.a > end)
        {
            color -= calColor;
            com.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
        gameObject.transform.SetParent(transform.GetChild(0).GetChild(type));

    }
    public GameObject PrintEffect3(int effectNum, Transform parent)
    {
        Transform folder = transform.GetChild(0).GetChild(effectNum);
        GameObject effect;

        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).gameObject;
            effect.transform.SetParent(parent);
        }
        else
        {
            effect = Instantiate(effects[effectNum], parent);
            effect.GetComponent<EffectReturn>().type = effectNum;
        }
        ReObject(effect);
        return effect;
    }
    void ReObject(GameObject effect)
    {
        effect.SetActive(true);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localEulerAngles = Vector3.zero;
    }
    public GameObject PrintEffect4(Vector3 start, Vector3 Destination, int effectNum)
    {
        Transform folder = transform.GetChild(0).GetChild(effectNum);
        GameObject effect;

        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).gameObject;
            effect.transform.SetParent(folder.parent);
        }
        else
        {
            effect = Instantiate(effects[effectNum], transform);
            effect.GetComponent<EffectReturn>().type = effectNum;
        }
        ReObject(effect, start, Destination);
        return effect;
    }
    void ReObject(GameObject effect, Vector3 vec, Vector3 des)
    {
        ReObject(effect, vec);
        effect.transform.LookAt(des);
        effect.transform.GetChild(0).transform.localPosition = Vector3.zero;
    }
    public TextMeshProUGUI PrintTextMesh(Vector3 vec, string qltskrka = "ºø³ª°¨")
    {
        Transform folder = transform.GetChild(0).GetChild(4);
        TextMeshProUGUI effect;
        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            effect.text = qltskrka;
            effect.transform.SetParent(canvas);
        }
        else
        {
            effect = Instantiate(effects[4], canvas).GetComponent<TextMeshProUGUI>();
            effect.GetComponent<EffectReturn>().type = 4;
        }
        ReUI(effect, vec);
        return effect;
    }
    private void ReUI(MaskableGraphic ui, Vector3 vec)
    {
        ui.color += new Color(0, 0, 0, 1);
        ReUI(ui.rectTransform, vec);
    }
    void ReUI(RectTransform rect, Vector3 vec)
    {
        rect.localPosition = Data.Instance.CameratoCanvas(vec) + Vector3.up * offset1;
        rect.gameObject.SetActive(true);
        rect.transform.localScale = new Vector3(1, 1, 1);
        rect.transform.localRotation = Quaternion.identity;
    }

    public GameObject PrintAnimation(Vector3 vec)
    {
        Transform folder = transform.GetChild(0).GetChild(5);
        RectTransform effect;
        if (folder.childCount > 1)
        {
            effect = folder.GetChild(0).GetComponent<RectTransform>();
            effect.transform.SetParent(canvas);
        }
        else
        {
            effect = Instantiate(effects[5], canvas).GetComponent<RectTransform>();
            effect.GetComponent<EffectReturn>().type = 5;
        }
        ReUI(effect, vec);
        return effect.gameObject;
    }
    public void EffectPositionFollow(GameObject effect, CObject unit)
    {
        RectTransform rec = effect.GetComponent<RectTransform>();

        StartCoroutine(EffectFollow());


        IEnumerator EffectFollow()
        {
            if (unit.TryGetComponent(out CObject unitComponent))
                while (unitComponent && rec.gameObject.activeSelf)
                {
                    rec.localPosition = Data.Instance.CameratoCanvas(unit.transform.position) + Vector3.up * offset1;
                    yield return null;
                }
        }
    }
    public ProjectileComponent CallProjectileComponent(int effectIndex)
    {
        Transform folder = transform.GetChild(1).GetChild(effectIndex);
        ProjectileComponent projectileComponent;
        if (projectileQueue[effectIndex].Count > 0)
        {
            projectileComponent = projectileQueue[effectIndex].Dequeue();
            projectileComponent.transform.SetParent(folder.parent);
        }
        else
            projectileComponent = Instantiate(projectileEffects[effectIndex], folder.parent);

        return projectileComponent;
    }
    public void ReturnProjectileComponent(int effectIndex, ProjectileComponent usedComponent)
    {
        usedComponent.transform.SetParent(transform.GetChild(1).GetChild(effectIndex));
        projectileQueue[effectIndex].Enqueue(usedComponent);
    }
}
