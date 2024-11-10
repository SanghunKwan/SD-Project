using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PlayBase : InitObject
{

    protected IEnumerator WaitingforSeconds(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);

        action();
    }
}
