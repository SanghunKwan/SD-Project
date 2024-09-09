using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildSetCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BuildingSetWindow buildingSetWindow;

    Image image;

    public bool isDrag { get => buildingSetWindow.isDrag; }
    bool isEnter;

    [SerializeField] Color effectColor;

    private void Start()
    {
        image = GetComponent<Image>();
        buildingSetWindow = transform.parent.parent.GetComponent<BuildingSetWindow>();
        buildingSetWindow.AddDragEnd(DragEnd);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDrag)
        {
            isEnter = true;
            buildingSetWindow.SetHeroInDic(transform.parent.gameObject);
            image.color = effectColor;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag)
        {
            buildingSetWindow.SetBackHeroText(transform.parent.gameObject);
        }
        isEnter = false;
        image.color = Color.clear;
    }
    void DragEnd()
    {
        image.color = Color.clear;

        if (isEnter)
            buildingSetWindow.SaveHeroData(transform.parent.gameObject);
    }
}
