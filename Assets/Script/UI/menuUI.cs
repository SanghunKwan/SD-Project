using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class menuUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] menuWindow menuWindow;


    public void OnPointerClick(PointerEventData eventData)
    {
        //뒤쪽에 투명한 UI 씌우기 추가
        menuWindow.OnOffWindow();
    }

}
