using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandTeam : MonoBehaviour
{
    [SerializeField] HandImage handImage;
    HandImage[] handImages;
    [SerializeField] Image comp_image;
    public Image image { get { return comp_image; } }
    [SerializeField] Text teamCode;


    public void Init(villigeViewPort viewport)
    {
        int teamMemberCount = viewport.transform.childCount - 1;
        handImages = new HandImage[teamMemberCount];
        SetImageSize(teamMemberCount);

        for (int i = 0; i < handImages.Length; i++)
        {
            handImages[i] = Instantiate(handImage, transform);
            SetPosition(i);
            handImages[i].SetText(viewport.characters[i].HeroInfoText());
        }
        SetTeamCode(viewport.characters[0].hero.keycode);
    }
    void SetImageSize(int teamMemberCount)
    {
        Vector2 sizeDelta = image.rectTransform.sizeDelta;
        sizeDelta.y = 100 + 100 * teamMemberCount;
        image.rectTransform.sizeDelta = sizeDelta;
    }
    void SetPosition(int index)
    {
        handImages[index].image.rectTransform.anchoredPosition = new Vector2(0, -80 - index * 100);
    }
    void SetTeamCode(in string str)
    {
        teamCode.text = str;
    }

}
