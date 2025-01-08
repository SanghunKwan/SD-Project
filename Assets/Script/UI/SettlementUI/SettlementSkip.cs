using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SettlementManager))]
public class SettlementSkip : MonoBehaviour
{
    SettlementManager settlementManager;

    [SerializeField] SettleInput settleInput;

    private void Awake()
    {
        settlementManager = GetComponent<SettlementManager>();
        settleInput.onClickEvent += SetIntervalZero;
    }
    public void SetIntervalZero()
    {
        settlementManager.interval = 0;
        settleInput.onClickEvent -= SetIntervalZero;
    }
}
