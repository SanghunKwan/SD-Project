using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettleController : MonoBehaviour
{
    protected int m_interval;
    public abstract void PlaySettle(Action onPlayEnd, int interval);
}
