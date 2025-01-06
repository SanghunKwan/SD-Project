using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettleController : MonoBehaviour
{
    public abstract void PlaySettle(Action onPlayEnd);
}
