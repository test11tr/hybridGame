using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CollectQuest : Quest 
{
    public enum QuestItemType
    {
        Coin,
        Gem,
        Wood,
        Stone
    }

    [Foldout("Quest Details", foldEverything = true, styled = true, readOnly = false)]
    public int RequiredAmount = 5;
    public int CurrentAmount = 0;
    public QuestItemType questItemType;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;

    private void EnableWalletListener()
    {
        if(questItemType == QuestItemType.Coin)
            WalletModule.OnCoinCurrencyChanged += CheckCurrency;
        else if(questItemType == QuestItemType.Gem)
            WalletModule.OnGemCurrencyChanged += CheckCurrency;
        else if(questItemType == QuestItemType.Wood)
            WalletModule.OnWoodCurrencyChanged += CheckCurrency;
        else if(questItemType == QuestItemType.Stone)
            WalletModule.OnStoneCurrencyChanged += CheckCurrency;
    }

    private void DisableWalletListener()
    {
        if(questItemType == QuestItemType.Coin)
            WalletModule.OnCoinCurrencyChanged -= CheckCurrency;
        else if(questItemType == QuestItemType.Gem)
            WalletModule.OnGemCurrencyChanged -= CheckCurrency;
        else if(questItemType == QuestItemType.Wood)
            WalletModule.OnWoodCurrencyChanged -= CheckCurrency;
        else if(questItemType == QuestItemType.Stone)
            WalletModule.OnStoneCurrencyChanged -= CheckCurrency;
    }

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"Quest: {Description}. Collect {RequiredAmount} {questItemType}.");
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

    private void CheckCurrency(int newAmount)
    {
        CurrentAmount = CurrentAmount + newAmount;
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
