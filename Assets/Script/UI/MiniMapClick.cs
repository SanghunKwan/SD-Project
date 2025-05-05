using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapClick : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler
{
    RawImage rawImage;
    ClickDrag clickdrag;
    Texture2D texture2D;
    Camera miniMapCam;

    bool outMIniMap = false;
    bool isColorChanged = false;
    bool isDragStartInMinimap;

    delegate void Action<in RaycastHit>(RaycastHit hit);
    Action<RaycastHit>[] action;

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        clickdrag = transform.parent.GetComponent<ClickDrag>();
        StartCoroutine(DelayDrawPixel());
        miniMapCam = GameObject.FindGameObjectWithTag("Add_Cam").GetComponent<Camera>();

        action = new Action<RaycastHit>[]
        {
            (hit)=>{ActionLeft(hit); },
            (hit)=>{ActionRight(hit); }
        };

        if (GameManager.manager.battleClearManager == null)
            GameManager.manager.onBattleClearManagerRegistered += RegisterStageClearEvent;
        else
            RegisterStageClearEvent();
    }
    void ActionLeft(RaycastHit hit)
    {
        GameManager.manager.ScreenToPoint(hit.point);
    }
    void ActionRight(RaycastHit hit)
    {
        GameManager.manager.OrderUnit(hit);
    }
    void RegisterStageClearEvent()
    {
        GameManager.manager.battleClearManager.onStageChanged = MinimapReload;
    }
    public void MinimapReload()
    {
        StartCoroutine(DelayDrawPixel());
    }
    IEnumerator DelayDrawPixel()
    {
        var texture = rawImage.texture as RenderTexture;
        yield return null;

        RenderTexture.active = texture;

        texture2D = new Texture2D(texture.width, texture.height);
        texture2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture2D.Apply();
    }


    void GetPixel(Vector2 vector2, out float colora)
    {
        colora = texture2D.GetPixelBilinear((vector2.x / rawImage.rectTransform.sizeDelta.x),
                                             (vector2.y / rawImage.rectTransform.sizeDelta.y)).a;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetPixel(eventData.position, out float colora);
        if (colora < 0.1f)
        {
            clickdrag.OnPointerClick(eventData);
            clickdrag.MiniMapClick = false;
        }
        else
        {
            MoveScreen(eventData);
        }
    }
    void MoveScreen(PointerEventData eventData)
    {

        Vector3 onImagePosition = new Vector3(eventData.position.x / rawImage.rectTransform.sizeDelta.x,
                                                  eventData.position.y / rawImage.rectTransform.sizeDelta.y, 0);


        if (Physics.Raycast(miniMapCam.ViewportPointToRay(onImagePosition), out RaycastHit hit, float.MaxValue, 1 << 8))
        {
            action[(int)eventData.button](hit);
        }
        else
        {
            clickdrag.OnPointerClick(eventData);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (outMIniMap)
            clickdrag.OnBeginDrag(eventData);
        isDragStartInMinimap = outMIniMap;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragStartInMinimap)
        {
            clickdrag.OnDrag(eventData);
        }
        else
        {
            GetPixel(eventData.position, out float colora);
            outMIniMap = false;

            if (colora < 0.1f)
            {

            }
            else
            {
                MoveScreen(eventData);
                clickdrag.MiniMapClick = true;
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (outMIniMap)
        {
            clickdrag.OnEndDrag(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetPixel(eventData.position, out float colora);
        if (colora < 0.1f)
        {
            outMIniMap = true;
        }
        else
        {
            MoveScreen(eventData);
            outMIniMap = false;
            clickdrag.MiniMapClick = true;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        GetPixel(eventData.position, out float colorA);

        bool wasColor = isColorChanged;
        isColorChanged = colorA > 0.1f;

        if (wasColor == isColorChanged)
            return;

        if (isColorChanged)
            rawImage.color *= 0.9f;
        else
            rawImage.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outMIniMap = true;
        clickdrag.MiniMapClick = false;
    }
}
