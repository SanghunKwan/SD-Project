using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class menuUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] menuWindow menuWindow;


    public void OnPointerClick(PointerEventData eventData)
    {
        //���ʿ� ������ UI ����� �߰�
        menuWindow.OnOffWindow();
    }

}
