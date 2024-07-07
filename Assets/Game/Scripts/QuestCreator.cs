using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;

public class QuestCreator : MonoBehaviour
{
    public QuestManager questManager;
    
    public void CreateCollectQuest()
    {
        CollectQuest quest = new GameObject("Quest - Collect Quest").AddComponent<CollectQuest>();
        quest.transform.SetParent(questManager.questContainer.transform);
        quest.Title = "Enter Title";
        quest.Description = "Enter Description";
        quest.RequiredAmount = 1;
        quest.questItemType = CollectQuest.QuestItemType.Coin;
        quest.hasHotspot = false;
        CinemachineVirtualCamera hotspotCamera = new GameObject("Hotspot Camera").AddComponent<CinemachineVirtualCamera>();
        hotspotCamera.transform.SetParent(quest.transform);
        hotspotCamera.gameObject.SetActive(true);
        hotspotCamera.Priority = 1;
        hotspotCamera.transform.position = new Vector3(0, 20, -20);
        hotspotCamera.transform.rotation = Quaternion.Euler(40, 0, 0);
        hotspotCamera.m_Lens.FieldOfView = 50;
        quest.hotspotCamera = hotspotCamera;
        questManager.quests.Add(quest);
    }

    public void CreateCharacterLevelQuest()
    {
        CharacterLevelQuest quest = new GameObject("Quest - Level/EXP Quest").AddComponent<CharacterLevelQuest>();
        quest.transform.SetParent(questManager.questContainer.transform);
        quest.Title = "Enter Title";
        quest.Description = "Enter Description";
        quest.RequiredAmount = 1;
        quest.questType = CharacterLevelQuest.QuestType.Level;
        questManager.quests.Add(quest);
    }

    public void CreateKillQuest()
    {
        KillQuest quest = new GameObject("Quest - Kill Quest").AddComponent<KillQuest>();
        quest.transform.SetParent(questManager.questContainer.transform);
        quest.Title = "Enter Title";
        quest.Description = "Enter Description";
        quest.RequiredAmount = 1;
        quest.questEnemyType = KillQuest.QuestEnemyType.Civilian;
        CinemachineVirtualCamera hotspotCamera = new GameObject("Hotspot Camera").AddComponent<CinemachineVirtualCamera>();
        hotspotCamera.transform.SetParent(quest.transform);
        hotspotCamera.gameObject.SetActive(true);
        hotspotCamera.Priority = 1;
        hotspotCamera.transform.position = new Vector3(0, 20, -20);
        hotspotCamera.transform.rotation = Quaternion.Euler(40, 0, 0);
        hotspotCamera.m_Lens.FieldOfView = 50;
        quest.hotspotCamera = hotspotCamera;
        questManager.quests.Add(quest);
    }

    public void CreateUpgradeQuest()
    {
        UpgradeQuest quest = new GameObject("Quest - Upgrade Quest").AddComponent<UpgradeQuest>();
        quest.transform.SetParent(questManager.questContainer.transform);
        quest.Title = "Enter Title";
        quest.Description = "Enter Description";
        quest.RequiredAmount = 1;
        questManager.quests.Add(quest);
    }

    public void CreateUnlockQuest()
    {
        UnlockQuest quest = new GameObject("Quest - Unlock Area Quest").AddComponent<UnlockQuest>();
        quest.transform.SetParent(questManager.questContainer.transform);
        quest.Title = "Enter Title";
        quest.Description = "Enter Description";
        quest.RequiredAmount = 1;
        quest.questType = UnlockQuest.QuestType.CriticalDamage;
        CinemachineVirtualCamera hotspotCamera = new GameObject("Hotspot Camera").AddComponent<CinemachineVirtualCamera>();
        hotspotCamera.transform.SetParent(quest.transform);
        hotspotCamera.gameObject.SetActive(true);
        hotspotCamera.Priority = 1;
        hotspotCamera.transform.position = new Vector3(0, 20, -20);
        hotspotCamera.transform.rotation = Quaternion.Euler(40, 0, 0);
        hotspotCamera.m_Lens.FieldOfView = 50;
        quest.hotspotCamera = hotspotCamera;
        questManager.quests.Add(quest);
    }

    public void setIDs()
    {
        for (int i = 0; i < questManager.quests.Count; i++)
        {
            questManager.quests[i].ID = i;
        }
    }
}
