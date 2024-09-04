using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.UI;

public class ItemPreview : MonoBehaviour
{
    Image[] images = new Image[3];
    [SerializeField] AddressableManager addrMgr;
    
    float[] floats = new float[(int)AddressableManager.ItemQuality.MAX];
    MouseOnImage interact;


    private void Awake()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        floats[1] = 0.5f;
        for (int i = 2; i < floats.Length; i++)
        {
            floats[i] = 1;
        }
        interact = GetComponent<MouseOnImage>();
    }

    public void ActiveItemPreview(Hero hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 3; i++)
        {
            SetImage(hero.Getnum, (AddressableManager.ItemQuality)hero.EquipsNum[i], (AddressableManager.EquipsImage)i);
        }
    }
    public void SetImage(TypeNum type, AddressableManager.ItemQuality itemQuality, AddressableManager.EquipsImage equipsImage)
    {
        string dicKey = type.ToString() + itemQuality.ToString();
        int ImageIndex = (int)equipsImage;

        addrMgr.GetData(dicKey, equipsImage, out Sprite sprite);
        images[ImageIndex].sprite = sprite;

        images[ImageIndex].color = Color.white * floats[(int)itemQuality];
    }

}
