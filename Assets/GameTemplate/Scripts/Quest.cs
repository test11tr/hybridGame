using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

[Serializable]
public class Quest : MonoBehaviour {
    public enum RewardType
    {
        coinCurrency,
        expCurrency,
        gemCurrency,
        woodCurrency,
        stoneCurrency,
        unlockAreaCurrency
    }   

    [DisplayWithoutEdit()] public int ID;
    public string Description;
    public int RequiredAmount;
    public int CurrentAmount;
    [DisplayWithoutEdit()] public bool IsCompleted;
    [DisplayWithoutEdit()] public bool IsActive;
    public bool multiReward;
    public RewardType rewardType1;
    public RewardType rewardType2;
    public int rewardAmount1;
    public int rewardAmount2;

    public virtual void StartQuest()
    {
    }
    
    public virtual void CompleteQuest() {
        IsCompleted = true;
        GameManager.Instance.questManager.CompleteQuest(this);
        GameManager.Instance.soundModule.PlaySound("success");
    }

    public void GiveRewards(bool multiReward, RewardType rewardType1, RewardType rewardType2, int rewardAmount1, int rewardAmount2)
    {
        if(multiReward)
        {
            if(rewardType1 == RewardType.coinCurrency)
                GameManager.Instance.wallet.AddCoin(rewardAmount1);
            else if(rewardType1 == RewardType.expCurrency)
                GameManager.Instance.experienceModule.AddExperience(rewardAmount1);
            else if(rewardType1 == RewardType.gemCurrency)
                GameManager.Instance.wallet.AddGem( rewardAmount1);
            else if(rewardType1 == RewardType.woodCurrency)
                GameManager.Instance.wallet.AddWood( rewardAmount1);
            else if(rewardType1 == RewardType.stoneCurrency)
                GameManager.Instance.wallet.AddStone(rewardAmount1);

            if(rewardType2 == RewardType.coinCurrency)
                GameManager.Instance.wallet.AddCoin(rewardAmount2);
            else if(rewardType2 == RewardType.expCurrency)
                GameManager.Instance.experienceModule.AddExperience(rewardAmount2);
            else if(rewardType2 == RewardType.gemCurrency)
                GameManager.Instance.wallet.AddGem( rewardAmount2);
            else if(rewardType2 == RewardType.woodCurrency)
                GameManager.Instance.wallet.AddWood( rewardAmount2);
            else if(rewardType2 == RewardType.stoneCurrency)
                GameManager.Instance.wallet.AddStone(rewardAmount2);
        }
        else
        {
            if(rewardType1 == RewardType.coinCurrency)
                GameManager.Instance.wallet.AddCoin(rewardAmount1);
            else if(rewardType1 == RewardType.expCurrency)
                GameManager.Instance.experienceModule.AddExperience(rewardAmount1);
            else if(rewardType1 == RewardType.gemCurrency)
                GameManager.Instance.wallet.AddGem( rewardAmount1);
            else if(rewardType1 == RewardType.woodCurrency)
                GameManager.Instance.wallet.AddWood( rewardAmount1);
            else if(rewardType1 == RewardType.stoneCurrency)
                GameManager.Instance.wallet.AddStone(rewardAmount1);
        }
    }  

    public Sprite GetRewardIcon(RewardType rewardType) {
    switch (rewardType) {
        case RewardType.coinCurrency:
            return GameManager.Instance.questManager.coinCurrency.lootSprite;
        case RewardType.expCurrency:
            return GameManager.Instance.questManager.expCurrency.lootSprite;
        case RewardType.gemCurrency:
            return GameManager.Instance.questManager.gemCurrency.lootSprite;
        case RewardType.woodCurrency:
            return GameManager.Instance.questManager.woodCurrency.lootSprite;
        case RewardType.stoneCurrency:
            return GameManager.Instance.questManager.stoneCurrency.lootSprite;
        case RewardType.unlockAreaCurrency:
            return GameManager.Instance.questManager.unlockAreaCurrency.lootSprite;
        default:
            return null;
    }
    }

    public string GetRewardText(RewardType rewardType, int rewardAmount) {
        switch (rewardType) {
            case RewardType.coinCurrency:
                return "+" + rewardAmount.ToString() + " " + GameManager.Instance.questManager.coinCurrency.lootName;
            case RewardType.expCurrency:
                return "+" + rewardAmount.ToString() + " " + GameManager.Instance.questManager.expCurrency.lootName;
            case RewardType.gemCurrency:
                return "+" + rewardAmount.ToString() + " " + GameManager.Instance.questManager.gemCurrency.lootName;
            case RewardType.woodCurrency:
                return "+" + rewardAmount.ToString() + " " + GameManager.Instance.questManager.woodCurrency.lootName;
            case RewardType.stoneCurrency:
                return "+" + rewardAmount.ToString() + " " + GameManager.Instance.questManager.stoneCurrency.lootName;
            case RewardType.unlockAreaCurrency: 
                return "Unlock New Area!";
            default:
                return "";
        }
    }
}