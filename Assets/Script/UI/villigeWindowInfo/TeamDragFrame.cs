using SDUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TeamDragFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_image;
    [SerializeField] Color effectColor;
    bool isTeamDrag { get { return VilligeTeamCollider.isDrag; } }
    SDUI_BuildSetCollider[] colliders;


    #region ����
    private void Start()
    {
        colliders = GetComponentsInChildren<SDUI_BuildSetCollider>();
    }
    #endregion
    #region �巡�� ���·� ���� ���
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isTeamDrag)
            return;

        m_image.color = effectColor;
        //�� �ؽ�Ʈ ����.
        ChangeTeamCollider();
        VilligeTeamCollider.now_Collider.dragEndEvent += DragEndAction;
    }
    void ChangeTeamCollider()
    {
        int repeatTime = Mathf.Min(colliders.Length, VilligeTeamCollider.now_Collider.viewPort.characters.Count);
        for (int i = 0; i < repeatTime; i++)
        {
            colliders[i].ChangeText(VilligeTeamCollider.now_Collider.viewPort.characters[i]);
        }
        for (int i = repeatTime; i < colliders.Length; i++)
        {
            colliders[i].NullText();
        }
    }
    #endregion
    #region �巡�׸� �ȿ��� ���� ���
    void DragEndAction()
    {
        int repeatTime = Mathf.Min(colliders.Length, VilligeTeamCollider.now_Collider.viewPort.characters.Count);

        for (int i = 0; i < repeatTime; i++)
        {
            colliders[i].DataChange(VilligeTeamCollider.now_Collider.viewPort.characters[i]);
        }
        for (int i = repeatTime; i < colliders.Length; i++)
        {
            colliders[i].DataErase();
        }

    }
    #endregion
    #region �巡�� �� ���� ���� ���
    public void OnPointerExit(PointerEventData eventData)
    {
        m_image.color = Color.clear;

        if (!isTeamDrag)
            return;

        VilligeTeamCollider.now_Collider.dragEndEvent -= DragEndAction;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].ResetText();
        }
    }
    #endregion
}
