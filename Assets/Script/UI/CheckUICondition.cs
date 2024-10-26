using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckUICondition : MonoBehaviour
{
    [SerializeField] UnityEvent conditionChanged;
    bool m_bool;
    public bool condition
    {
        get => m_bool;
        protected set
        {
            m_bool = value;
            EventActivate();
        }
    }
    protected void EventActivate()
    {
        conditionChanged.Invoke();
    }
}
