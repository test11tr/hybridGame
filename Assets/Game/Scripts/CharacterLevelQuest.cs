using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelQuest : Quest 
{
    public enum QuestType
    {
        Level,
        EXP,
    }

    public int RequiredAmount = 5;
    public int CurrentAmount = 0;
    public QuestType questType;

    private void EnableWalletListener()
    {
        if(questType == QuestType.Level)
            CharacterControlManager.OnLevelChange += CheckGoal;
        else if(questType == QuestType.EXP)
            CharacterControlManager.OnEXPChange += CheckGoal;
    }

    private void DisableWalletListener()
    {
        if(questType == QuestType.Level)
            CharacterControlManager.OnLevelChange -= CheckGoal;
        else if(questType == QuestType.EXP)
            CharacterControlManager.OnEXPChange -= CheckGoal;
    }

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"Quest: {Description}. Collect {RequiredAmount} {questType}.");
        GameManager.Instance.questManager.magnifyingGlass.gameObject.SetActive(false);
        EnableWalletListener();
        UpdateUI();
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
}
