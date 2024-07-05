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

    public int RequiredAmount = 5;
    public int CurrentAmount = 0;
    public QuestType questType;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;

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
        Debug.Log($"{Title}: {Description}. Do {RequiredAmount} {questType}.");
        SetButtonListener();
        EnableWalletListener();
        UpdateUI();
    }

    private void SetButtonListener()
    {
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
    }

    private void CheckGoal(int newAmount)
    {
        CurrentAmount = CurrentAmount + newAmount;
        UpdateUI();
        if (CurrentAmount >= RequiredAmount && !IsCompleted)
        {
            hotspotCamera.Priority = 11;
            DelayHelper.DelayAction(3.5f, () => {
                hotspotCamera.Priority = 1;
                GameManager.Instance.questManager.questBg.color = new Color(0.62f, 0.91f, 0.33f);
                GameManager.Instance.questManager.questButton.onClick.RemoveAllListeners();
                GameManager.Instance.questManager.questButton.onClick.AddListener(CompleteQuest);
                GameManager.Instance.questManager.completeTick.SetActive(true);
            });
        }
    }

    public override void CompleteQuest() {
        base.CompleteQuest();
        DisableWalletListener();
    }

    public void MoveToHotspot()
    {
        hotspotCamera.Priority = 11;
        DelayHelper.DelayAction(3.5f, () => {
            hotspotCamera.Priority = 1;
        });
    }
}
