using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class villigeViewPort : villigeBase
{
    public List<villigeInteract> characters { get; private set; } = new List<villigeInteract>();
    public bool isMoved { get; private set; } = false;
    [SerializeField] Text teamCode;
    public string teamName { get { return teamCode.text; } }
    [SerializeField] VilligeTeamCollider teamCollider;

    protected override void VirtualAwake()
    {
        
    }
    public void ImagePaddingMove(float nDown)
    {
        transform.position += Vector3.down * (image.raycastPadding.w + nDown);

        image.raycastPadding = new Vector4(0, nDown, 0, -nDown);
        SetInteractPadding(nDown, characters.Count);
        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, characters.Count * 100 + 100);
        isMoved = nDown != 0;
    }
    void SetInteractPadding(float offset, int index, int startindex = 0)
    {

        Vector4 vec = new Vector4(0, offset, 0, -offset);


        for (int i = startindex; i < index; i++)
        {

            characters[i].transform.position = transform.position + Vector3.down * (80 + i * 100 - vec.w + image.raycastPadding.w);
            characters[i].image.raycastPadding = vec;
        }

    }
    public void InterActMove(int nDown, int index)
    {
        transform.position += Vector3.down * image.raycastPadding.w;
        image.raycastPadding = new Vector4(0, nDown, 0, 0);

        SetInteractPadding(0, index);
        SetInteractPadding(nDown, characters.Count, index);
        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, characters.Count * 100 + 200);

        isMoved = true;

    }
    public void ArrangeReset()
    {
        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, characters.Count * 100 + 100);
        image.raycastPadding = Vector4.zero;
        isMoved = false;
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].image.raycastPadding = Vector4.zero;
            characters[i].transform.localPosition = new Vector2(0, -80 - (100 * i));
        }
    }
    public void ChangeTeamCode(in string newCode)
    {
        teamCode.text = newCode;
    }
    public void DragInvisible(bool onoff)
    {
        image.color = new Color(1, 1, 1, 0.5f) * Convert.ToInt32(onoff);
        teamCode.enabled = onoff;


        foreach (villigeInteract interact in characters)
        {
            interact.DragInvisible(onoff);
        }
    }
    public void TeamCodeHighlight(FontStyle style)
    {
        teamCode.fontStyle = style;

    }
    public void TeamColliderSetInteractive(bool onoff)
    {
        teamCollider.ButtonSetInteractive(onoff);
    }
}
