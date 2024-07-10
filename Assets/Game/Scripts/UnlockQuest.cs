using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class UnlockQuest : Quest 
{
    public enum QuestType
    {
        Coin,
        Gem,
        Wood,
        Stone,
        Civilian,
        Ranged,
        Melee,
        Boss,
        Level,
        EXP,
        MoveSpeed,
        Damage,
        AttackRange,
        CriticalChance,
        CriticalDamage,
        AttackSpeed,
        Health
    }

    [Foldout("Quest Details", foldEverything = true, styled = true, readOnly = false)]
    public QuestType questType;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;
    public UnlockableArea unlockableArea;

    private void EnableWalletListener()
    {
        if(questType == QuestType.Coin)
            WalletModule.OnCoinCurrencyChanged += CheckGoal;
        else if(questType == QuestType.Gem)
            WalletModule.OnGemCurrencyChanged += CheckGoal;
        else if(questType == QuestType.Wood)
            WalletModule.OnWoodCurrencyChanged += CheckGoal;
        else if(questType == QuestType.Stone)
            WalletModule.OnStoneCurrencyChanged += CheckGoal;
        else if(questType == QuestType.Civilian)
            Enemy.OnCivilianDead += CheckGoal;
        else if(questType == QuestType.Ranged)
            Enemy.OnRangedDead += CheckGoal;
        else if(questType == QuestType.Melee)
            Enemy.OnMeleeDead += CheckGoal;
        else if(questType == QuestType.Boss)
            Enemy.OnBossDead += CheckGoal;
        else if(questType == QuestType.Level)
            CharacterControlManager.OnLevelChange += CheckGoal;
        else if(questType == QuestType.EXP)
            CharacterControlManager.OnEXPChange += CheckGoal;
        else if(questType == QuestType.MoveSpeed)
            CharacterStatsModule.OnMoveSpeedBuy += CheckGoal;
        else if(questType == QuestType.Damage)
            CharacterStatsModule.OnDamageBuy += CheckGoal;
        else if(questType == QuestType.AttackRange)
            CharacterStatsModule.OnAttackRangeBuy += CheckGoal;
        else if(questType == QuestType.CriticalChance)
            CharacterStatsModule.OnCriticalChanceBuy += CheckGoal;
        else if(questType == QuestType.CriticalDamage)
            CharacterStatsModule.OnCriticalDamageBuy += CheckGoal;
        else if(questType == QuestType.AttackSpeed)
            CharacterStatsModule.OnAttackSpeedBuy += CheckGoal;
        else if(questType == QuestType.Health)
            CharacterStatsModule.OnHealthBuy += CheckGoal;
    }

    private void DisableWalletListener()
    {
        if(questType == QuestType.Coin)
            WalletModule.OnCoinCurrencyChanged -= CheckGoal;
        else if(questType == QuestType.Gem)
            WalletModule.OnGemCurrencyChanged -= CheckGoal;
        else if(questType == QuestType.Wood)
            WalletModule.OnWoodCurrencyChanged -= CheckGoal;
        else if(questType == QuestType.Stone)
            WalletModule.OnStoneCurrencyChanged -= CheckGoal;
        else if(questType == QuestType.Civilian)
            Enemy.OnCivilianDead -= CheckGoal;
        else if(questType == QuestType.Ranged)
            Enemy.OnRangedDead -= CheckGoal;
        else if(questType == QuestType.Melee)
            Enemy.OnMeleeDead -= CheckGoal;
        else if(questType == QuestType.Boss)
            Enemy.OnBossDead -= CheckGoal;
        else if(questType == QuestType.Level)
            CharacterControlManager.OnLevelChange -= CheckGoal;
        else if(questType == QuestType.EXP)
            CharacterControlManager.OnEXPChange -= CheckGoal;
        else if(questType == QuestType.MoveSpeed)
            CharacterStatsModule.OnMoveSpeedBuy -= CheckGoal;
        else if(questType == QuestType.Damage)
            CharacterStatsModule.OnDamageBuy -= CheckGoal;
        else if(questType == QuestType.AttackRange)
            CharacterStatsModule.OnAttackRangeBuy -= CheckGoal;
        else if(questType == QuestType.CriticalChance)
            CharacterStatsModule.OnCriticalChanceBuy -= CheckGoal;
        else if(questType == QuestType.CriticalDamage)
            CharacterStatsModule.OnCriticalDamageBuy -= CheckGoal;
        else if(questType == QuestType.AttackSpeed)
            CharacterStatsModule.OnAttackSpeedBuy -= CheckGoal;
        else if(questType == QuestType.Health)
            CharacterStatsModule.OnHealthBuy -= CheckGoal;
    }

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"Quest: {Description}. Do {RequiredAmount} {questType}.");
        SetButtonListener();
        EnableWalletListener();
        UpdateUI();

        hotspotCamera.Priority = 11;
        unlockableArea.showUI();
        DelayHelper.DelayAction(3.5f, () => {
            hotspotCamera.Priority = 1;
            unlockableArea.closeUI();
        });
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

    public void UpdateUI() 
    {
        GameManager.Instance.questManager.questBg.color = new Color32(34, 34, 34, 178);
        GameManager.Instance.questManager.questDescriptionText.text = Description;
        GameManager.Instance.questManager.progressBar.fillAmount = (float)CurrentAmount / RequiredAmount;
        GameManager.Instance.questManager.progressText.text = $"{CurrentAmount}/{RequiredAmount}";
        GameManager.Instance.questManager.completeTick.SetActive(false);
        GameManager.Instance.questManager.questSlot.SetActive(true);
        unlockableArea.questDescriptionText.text = Description;
        unlockableArea.progressBar.gameObject.SetActive(true);
        unlockableArea.progressBar.fillAmount = (float)CurrentAmount / RequiredAmount;
        unlockableArea.progressText.text = $"{CurrentAmount}/{RequiredAmount}";

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
            GameManager.Instance.questManager.completeTick.SetActive(true);
            CompleteQuest();
        }
    }

    public override void CompleteQuest() {
        hotspotCamera.Priority = 11;
        unlockableArea.showUI();
        DelayHelper.DelayAction(1.5f, () => {
            unlockableArea.unlockArea();
        });
        DelayHelper.DelayAction(3f, () => {
            unlockableArea.closeUI();
            hotspotCamera.Priority = 1;
            base.CompleteQuest();
            GiveRewards(multiReward, rewardType1, rewardType2, rewardAmount1, rewardAmount2);
            DisableWalletListener();
        });
       
    }

    public void MoveToHotspot()
    {
        hotspotCamera.Priority = 11;
        DelayHelper.DelayAction(3.5f, () => {
            hotspotCamera.Priority = 1;
        });
    }
}
