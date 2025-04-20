using SaveData;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public abstract class UpgradePreview : MonoBehaviour
{
    public abstract void Awake();
    public abstract void ActivePreview(Hero hero);
    public abstract void ActivePreview(HeroData hero);
}
