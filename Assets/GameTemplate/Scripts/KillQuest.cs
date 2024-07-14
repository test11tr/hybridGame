using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class KillQuest : Quest 
{
    public enum QuestEnemyType
    {
        Civilian,
        Ranged,
        Melee,
        Boss
    }

    [Foldout("Quest Details", foldEverything = true, styled = true, readOnly = false)]
    public QuestEnemyType questEnemyType;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;

    private void EnableWalletListener()
    {
        if(questEnemyType == QuestEnemyType.Civilian)
            Enemy.OnCivilianDead += CheckGoal;
        else if(questEnemyType == QuestEnemyType.Ranged)
            Enemy.OnRangedDead += CheckGoal;
        else if(questEnemyType == QuestEnemyType.Melee)
            Enemy.OnMeleeDead += CheckGoal;
        else if(questEnemyType == QuestEnemyType.Boss)
            Enemy.OnBossDead += CheckGoal;
    }

    private void DisableWalletListener()
    {
        if(questEnemyType == QuestEnemyType.Civilian)
            Enemy.OnCivilianDead -= CheckGoal;
        else if(questEnemyType == QuestEnemyType.Ranged)
            Enemy.OnRangedDead -= CheckGoal;
        else if(questEnemyType == QuestEnemyType.Melee)
            Enemy.OnMeleeDead -= CheckGoal;
        else if(questEnemyType == QuestEnemyType.Boss)
            Enemy.OnBossDead -= CheckGoal;
    }

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"Quest: {Description}. Kill {RequiredAmount} {questEnemyType}.");
        SetButtonListener();
        EnableWalletListener();
        UpdateUI();
    }

    private void SetButtonListener()
    {
        GameManager.Instance.questManager.questButton.onClick.RemoveAllListeners();
        if(hasHotspot)
        {
            GameManager.Instance.questManager.magnifyingGlass.gameObject.SetActive(true);
            GameManager.Instance.questManager.questButton.onClick.AddListener(MoveToHotspot);
        }
    }

    public void UpdateUI() {
        GameManager.Instance.questManager.questBg.color = new Color32(34, 34, 34, 178);
        GameManager.Instance.questManager.questDescriptionText.text = Description;
        GameManager.Instance.questManager.progressBar.fillAmount = (float)CurrentAmount / RequiredAmount;
        GameManager.Instance.questManager.progressText.text = $"{CurrentAmount}/{RequiredAmount}";
        GameManager.Instance.questManager.completeTick.SetActive(false);
        GameManager.Instance.questManager.questSlot.SetActive(true);

        if(multiReward)
        {
            GameManager.Instance.questManager.SingleRewardSlot.SetActive(false);
            GameManager.Instance.questManager.MultiRewardSlot.SetActive(true);
            GameManager.Instance.questManager.multiRewardIcon1.sprite = GetRewardIcon(rewardType1);
            GameManager.Instance.questManager.multiRewardText1.text = GetRewardText(rewardType1, rewardAmount1);
            GameManager.Instance.questManager.multiRewardIcon2.sprite = GetRewardIcon(rewardType2);
            GameManager.Instance.questManager.multiRewardText2.text = GetRewardText(rewardType2, rewardAmount2);
        }
        else
        {
            GameManager.Instance.questManager.MultiRewardSlot.SetActive(false);
            GameManager.Instance.questManager.SingleRewardSlot.SetActive(true);
            GameManager.Instance.questManager.singleRewardIcon.sprite = GetRewardIcon(rewardType1);
            GameManager.Instance.questManager.singleRewardText.text = GetRewardText(rewardType1, rewardAmount1);
        }

    }

    private void CheckGoal(int newAmount)
    {
        CurrentAmount += newAmount;
        UpdateUI();
        GameManager.Instance.saveModule.updateQuestInfosFromQuests();
        if (CurrentAmount >= RequiredAmount && !IsCompleted)
        {
            GameManager.Instance.questManager.questBg.color = new Color(0.62f, 0.91f, 0.33f);
            GameManager.Instance.questManager.questButton.onClick.RemoveAllListeners();
            GameManager.Instance.questManager.questButton.onClick.AddListener(CompleteQuest);
            GameManager.Instance.questManager.completeTick.SetActive(true);
        }
    }

    public override void CompleteQuest() {
        base.CompleteQuest();
        GiveRewards(multiReward, rewardType1, rewardType2, rewardAmount1, rewardAmount2);
        DisableWalletListener();
    }

    public void MoveToHotspot()
    {
        hotspotCamera.Priority = 11;
        DelayHelper.DelayAction(2.5f, () => {
            hotspotCamera.Priority = 1;
        });
    }
}
