using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SettleCanSkip))]
public class SettlementSkip : MonoBehaviour
{
    SettleCanSkip settlementManager;

    [SerializeField] SettleInput settleInput;

    private void Awake()
    {
        settlementManager = GetComponent<SettleCanSkip>();
        settleInput.onClickEvent += SetIntervalZero;
    }
    public void SetIntervalZero()
    {
        settlementManager.interval = 0;
        settleInput.onClickEvent -= SetIntervalZero;
        Debug.Log("½ºÅµ");
    }
}
