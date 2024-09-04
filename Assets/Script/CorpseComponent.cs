using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class CorpseComponent : MonoBehaviour
{
    CapsuleCollider coll;
    Dictionary<Collider, CUnit> onThisUnits = new Dictionary<Collider, CUnit>();

    private void Start()
    {
        coll = GetComponent<CapsuleCollider>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CObject obj) && obj.stat.ID == 106)
            CombineCorpse();

        if (!other.TryGetComponent(out CUnit unit))
            return;

        if (!onThisUnits.ContainsKey(other))
            onThisUnits.Add(other, unit);
        onThisUnits[other].overlappedCorpse++;
    }
    private void OnTriggerExit(Collider other)
    {
        if (onThisUnits.ContainsKey(other))
            onThisUnits[other].overlappedCorpse--;
    }
    void CombineCorpse()
    {
        Destroy(gameObject, 1);
    }
}
