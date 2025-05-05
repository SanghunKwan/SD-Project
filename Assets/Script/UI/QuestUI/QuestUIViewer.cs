using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIViewer : MonoBehaviour
{

    [SerializeField] QuestUISlot slotPrefab;
    [SerializeField] Transform ContentsTransform;
    Queue<QuestUISlot> hiddenSlots;

    private void Awake()
    {
        hiddenSlots = new Queue<QuestUISlot>(5);

        hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));
        hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));
        hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));
        hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));
        hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));
    }
    public void ShowNewQuest(out QuestUISlot newQuestUI)
    {

        if (hiddenSlots.Count <= 0)
            hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));

        newQuestUI = hiddenSlots.Dequeue();
        newQuestUI.Call();
    }
    public void ClearQuest(QuestUISlot oldUISlot)
    {
        oldUISlot.HideQuest(hiddenSlots);

    }
}
