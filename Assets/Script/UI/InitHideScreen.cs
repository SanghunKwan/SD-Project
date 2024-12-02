using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitHideScreen : InitObject
{
    public override void Init()
    {
        gameObject.SetActive(true);
    }

}
