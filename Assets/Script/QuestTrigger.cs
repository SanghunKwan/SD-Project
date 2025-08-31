using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    int findLayer;
    int id;
    public QuestManager.QuestType type { get; private set; }
    public int questIndex { get; private set; }
    CapsuleCollider capsuleCollider;
    Transform minimapTransform;

    Action<Vector3> triggerAction;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        minimapTransform = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckObjectType(other.gameObject))
            return;

        triggerAction(other.transform.position);

        gameObject.SetActive(false);
        GameManager.manager.questManager.questPool.ReturnQuest(this);
    }
    private void OnDisable()
    {
        triggerAction = null;
    }

    #region 외부 이벤트
    public void TriggerAllSet(int layer, in Action<Vector3> action, in Vector3 vec, float radius, int nowId)
    {
        gameObject.SetActive(true);
        triggerAction = action;
        findLayer = layer;
        transform.position = vec;
        capsuleCollider.radius = radius;
        minimapTransform.localScale = Vector3.one * radius;
        id = nowId;
    }
    public void AddMakeQuestData(in Action<Vector3> addAction, QuestManager.QuestType questType, int questNum)
    {
        triggerAction += addAction;
        type = questType;
        questIndex = questNum;
    }
    #endregion
    bool CheckObjectType(GameObject other)
    {
        if (other.layer != findLayer)
            return true;

        if (id == 0)
            return false;

        ObjectManager.CObjectType type = GetObjectType(other.layer);

        if (type == ObjectManager.CObjectType.Max)
        {
            //item일 경우
            int tempType = (int)ObjectManager.AdditionalType.Item;

            if (GameManager.manager.objectManager.NoneObjectDictionary[tempType].ContainsKey(other))
            {
                return id != ((ItemComponent)GameManager.manager.objectManager.NoneObjectDictionary[tempType][other].Value).Index;
            }
            return true;
        }
        else
        {
            if (GameManager.manager.objectManager.ObjectDictionary[(int)type].ContainsKey(other))
                return id != GameManager.manager.objectManager.ObjectDictionary[(int)type][other].Value.id;
            else
                return true;
        }
    }
    ObjectManager.CObjectType GetObjectType(int layerNum)
    {
        switch (layerNum)
        {
            case 7:
                return ObjectManager.CObjectType.Hero;

            case 9:
                return ObjectManager.CObjectType.FieldObject;

            case 10:
                return ObjectManager.CObjectType.Monster;

            default:
                {
                    Debug.Log("CObjectType 아님");
                    return ObjectManager.CObjectType.Max;
                }
        }
    }

}
