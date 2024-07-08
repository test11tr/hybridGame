using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class QuestManager : MonoBehaviour {

    [Foldout("Quests", foldEverything = true, styled = true, readOnly = false)]
    public GameObject questContainer;
    public List<Quest> quests = new List<Quest>();

    [Foldout("Quest UI", foldEverything = true, styled = true, readOnly = false)]
    public GameObject questSlot;
    public Button questButton;
    public Image questBg;
    public TMP_Text questDescriptionText;
    public Image progressBar;
    public TMP_Text progressText;
    public GameObject completeTick;
    public Image magnifyingGlass;

    void Start() {
        GetQuests();
        LoadQuests();
    }
    
    public void LoadQuests() {
        foreach (var quest in quests) {
            if (!quest.IsCompleted) {
                quest.gameObject.SetActive(true);
                quest.StartQuest();
                break;
            }else
            {
                quest.gameObject.SetActive(false);
            }
        }
    }

    public void CompleteQuest(Quest quest) {
        Debug.Log($"Completed Quest: {quest.Description}");
        quest.gameObject.SetActive(false);
        LoadQuests();
        UpdateQuestInfoList();
    }

    public void CreateQuestsList() {
        GameManager.Instance.saveModule.saveInfo.quests = new List<Quest>(quests);
    }

    public void GetQuests() {
        var savedQuests = GameManager.Instance.saveModule.saveInfo.quests;
        if (savedQuests != null && savedQuests.Count > 0) {
            if (quests.Count > savedQuests.Count) {
                var loadedQuestIDs = new List<int>();
                foreach (var quest in savedQuests) {
                    loadedQuestIDs.Add(quest.ID);
                }
                foreach (var quest in quests) {
                    if (!loadedQuestIDs.Contains(quest.ID)) {
                        savedQuests.Add(quest);
                    }
                }
                quests = new List<Quest>(savedQuests);
            }
        }

        var savedQuestInfos = GameManager.Instance.saveModule.saveInfo.questInfos;
        if (savedQuestInfos != null && savedQuestInfos.Count > 0)
        {
            foreach (var quest in quests)
            {
                var questInfo = savedQuestInfos.FirstOrDefault(qi => qi.questID == quest.ID);
                if (questInfo != null)
                {
                    quest.IsCompleted = questInfo.isComplete;
                    quest.gameObject.SetActive(!quest.IsCompleted);
                    if (!quest.IsCompleted)
                    {
                        quest.StartQuest();
                    }
                }
            }
        }

        CreateQuestsList();
        UpdateQuestInfoList();
    }

    public void UpdateQuestInfoList() {
        GameManager.Instance.saveModule.updateQuestInfosFromQuests();
    }
}