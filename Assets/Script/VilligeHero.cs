using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class VilligeHero : MonoBehaviour
{
    public Hero hero { get; private set; }
    public villigeInteract villigeInteract { get; private set; }

    private void Awake()
    {
        hero = GetComponent<Hero>();
    }
    public void Init(villigeInteract interactBoard)
    {
        villigeInteract = interactBoard;
        (VilligeNavi.nav as VilligeNavi).dicVh.Add(transform, this);
    }
}
