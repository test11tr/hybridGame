using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour {
    public List<Quest> quests = new List<Quest>();

    [Foldout("Quest UI", foldEverything = true, styled = true, readOnly = false)]
    public GameObject questSlot;
    public Button questButton;
    public Image questBg;
    public TMP_Text questDescriptionText;
    public Image progressBar;
    public TMP_Text progressText;
    public GameObject completeTick;

    void Start() {
        StartNextQuest();
    }

    public void AddQuest(Quest quest) {
        quests.Add(quest);
    }

    public void StartNextQuest() {
        foreach (var quest in quests) {
            if (!quest.IsCompleted) {
                quest.gameObject.SetActive(true);
                quest.StartQuest();
                break;
            }
        }
    }

    public void CompleteQuest(Quest quest) {
        Debug.Log($"Completed Quest: {quest.Title}");
        quest.gameObject.SetActive(false);
        StartNextQuest();
    }
}