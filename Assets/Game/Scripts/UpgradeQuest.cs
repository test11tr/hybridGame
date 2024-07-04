using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeQuest : Quest 
{
    public enum QuestIncrementalType
    {
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
    public QuestIncrementalType questIncrementalType;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;

    private void EnableWalletListener()
    {
        if(questIncrementalType == QuestIncrementalType.MoveSpeed)
            CharacterStatsModule.OnMoveSpeedBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.Damage)
            CharacterStatsModule.OnDamageBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.AttackRange)
            CharacterStatsModule.OnAttackRangeBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.CriticalChance)
            CharacterStatsModule.OnCriticalChanceBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.CriticalDamage)
            CharacterStatsModule.OnCriticalDamageBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.AttackSpeed)
            CharacterStatsModule.OnAttackSpeedBuy += CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.Health)
            CharacterStatsModule.OnHealthBuy += CheckGoal;
    }

    private void DisableWalletListener()
    {
        if(questIncrementalType == QuestIncrementalType.MoveSpeed)
            CharacterStatsModule.OnMoveSpeedBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.Damage)
            CharacterStatsModule.OnDamageBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.AttackRange)
            CharacterStatsModule.OnAttackRangeBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.CriticalChance)
            CharacterStatsModule.OnCriticalChanceBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.CriticalDamage)
            CharacterStatsModule.OnCriticalDamageBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.AttackSpeed)
            CharacterStatsModule.OnAttackSpeedBuy -= CheckGoal;
        else if(questIncrementalType == QuestIncrementalType.Health)
            CharacterStatsModule.OnHealthBuy -= CheckGoal;
    }

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"{Title}: {Description}. Buy {RequiredAmount} {questIncrementalType}.");
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
        CurrentAmount += newAmount;
        UpdateUI();
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
