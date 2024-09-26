using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrintFloor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    Animator animator;
    Text text;
    FloorManager floorManager;

    int m_nowFloor = 1;
    int[] hash = new int[2];

    IEnumerator tickCor;
    bool isTimeRemain;
    bool isOnMouse;


    private void Awake()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        text = transform.GetChild(0).GetComponent<Text>();

        floorManager = GetComponent<FloorManager>();

        HashSave();
    }
    void HashSave()
    {
        hash[0] = Animator.StringToHash("fadeOut");
        hash[1] = Animator.StringToHash("fadeIn");
    }

    private void Start()
    {
        if (floorManager.GetData(out FloorManager.FloorData data))
        {
            m_nowFloor = data.nowFloor;
        }
        PrintText();
    }
    private void OnEnable()
    {
        CheckTime();
    }
    void PrintText()
    {
        text.text = m_nowFloor.ToString() + " Ãþ";
        CheckTime();
    }
    void CheckTime()
    {
        isTimeRemain = true;
        if (tickCor != null)
            StopCoroutine(tickCor);
        tickCor = TextTick();
        StartCoroutine(tickCor);
    }
    IEnumerator TextTick()
    {
        float timeSave = 0;

        while (timeSave < 3)
        {
            timeSave += Time.deltaTime;
            yield return null;
        }

        if (!isOnMouse)
            animator.SetTrigger(hash[0]);

        isTimeRemain = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnMouse = false;

        if (!isTimeRemain)
            animator.SetTrigger(hash[0]);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnMouse = true;

        if (!isTimeRemain)
        {
            animator.SetTrigger(hash[1]);
            CheckTime();

        }
    }
}
