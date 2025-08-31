using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentState : MonoBehaviour
{
    [SerializeField]Image _hpImage;

    [SerializeField] TextMeshProUGUI _currentHp;
    [SerializeField] TextMeshProUGUI _maxHp;

    Sprite[] _hpSprites;
    [SerializeField]Color[] _fontColors;

    public void InitState()
    {
        _hpSprites = new Sprite[3];
        AddressableManager.manager.GetData(AddressableManager.LabelName.VilligeWindowImage, AddressableManager.VilligeWindowImage.fullHp, out _hpSprites[0]);
        AddressableManager.manager.GetData(AddressableManager.LabelName.VilligeWindowImage, AddressableManager.VilligeWindowImage.halfHp, out _hpSprites[1]);
        AddressableManager.manager.GetData(AddressableManager.LabelName.VilligeWindowImage, AddressableManager.VilligeWindowImage.emptyHp, out _hpSprites[2]);
    }

    public void SetHP(int currentHp, int maxHp)
    {
        _currentHp.text = currentHp.ToString();
        _maxHp.text = maxHp.ToString();

        int spriteIndex = 1;
        if (currentHp <= maxHp * 0.3f)
            spriteIndex = 2;
        else if (currentHp == maxHp)
            spriteIndex = 0;

        _hpImage.sprite = _hpSprites[spriteIndex];
        _currentHp.color = _fontColors[spriteIndex == 2 ? 1 : 0];
    }


}
