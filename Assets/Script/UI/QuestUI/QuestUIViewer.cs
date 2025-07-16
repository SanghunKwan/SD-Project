using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIViewer : MonoBehaviour
{

    [SerializeField] QuestUISlot slotPrefab;
    [SerializeField] Transform ContentsTransform;
    [SerializeField] Color[] gradeColors;
    [SerializeField] Color[] gradientColors;
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
    public void ShowNewQuest(QuestManager.QuestGrade grade, out QuestUISlot newQuestUI)
    {

        if (hiddenSlots.Count <= 0)
            hiddenSlots.Enqueue(Instantiate(slotPrefab, ContentsTransform));

        newQuestUI = hiddenSlots.Dequeue();
        int gradeIndex = (int)grade;
        newQuestUI.Call(gradeColors[gradeIndex], gradientColors[gradeIndex * 2], gradientColors[gradeIndex * 2 + 1]);
    }
    public void ClearQuest(QuestUISlot oldUISlot)
    {
        oldUISlot.HideQuest(hiddenSlots);

    }
}
