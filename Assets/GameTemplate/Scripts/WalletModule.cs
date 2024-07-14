using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AssetKits.ParticleImage;
using System;

public class WalletModule : MonoBehaviour
{
    [Foldout("Wallet - UI References", foldEverything = true, styled = true, readOnly = false)]
    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
 
    [Foldout("Wallet - Effects", foldEverything = true, styled = true, readOnly = false)]
    public ParticleImage coinEffect;
    public ParticleImage gemEffect;
    public ParticleImage woodEffect;
    public ParticleImage stoneEffect;

    [Foldout("Currencies (Live)", foldEverything = true, styled = true, readOnly = true)]
    public int coinCount;
    public int gemCount;
    public int woodAmount;
    public int stoneAmount;

    public static event Action<int> OnCoinCurrencyChanged;
    public static event Action<int> OnGemCurrencyChanged;
    public static event Action<int> OnWoodCurrencyChanged;
    public static event Action<int> OnStoneCurrencyChanged;

    private void Start()
    {
        coinCount = GameManager.Instance.saveModule.saveInfo.coinCount;
        gemCount = GameManager.Instance.saveModule.saveInfo.gemCount;
        woodAmount = GameManager.Instance.saveModule.saveInfo.woodAmount;
        stoneAmount = GameManager.Instance.saveModule.saveInfo.stoneAmount;
        coinText.text = coinCount.ToString();
        gemText.text = gemCount.ToString();
        woodText.text = woodAmount.ToString();
        stoneText.text = stoneAmount.ToString();
    }

    public bool CanAfford(string type, float price)
    {
        if(type == "coin")
        {
            return coinCount >= price;
        }
        else if(type == "gem")
        {
            return gemCount >= price;
        }
        else if(type == "wood")
        {
            return woodAmount >= price;
        }
        else if(type == "stone")
        {
            return stoneAmount >= price;
        }
        else
        {
            print("Invalid type, please use coin, gem, wood or stone as type or customize script.");
            return false;
        }
    }

    public void DeductCurrency(string type, float amount)
    {
        if(type == "coin")
        {
            RemoveCoin((int)amount);
        }
        else if(type == "gem")
        {
            RemoveGem((int)amount);
        }
        else if(type == "wood")
        {
            RemoveWood((int)amount);
        }
        else if(type == "stone")
        {
            RemoveStone((int)amount);
        }
        else
        {
            print("Invalid type, please use coin, gem, wood or stone as type or customize script.");
        }
    }

    public void AddCoin(int amount)
    {
        int previousAmount = coinCount;
        coinCount += amount;
        coinText.text = NumberFormatter.Convert(coinCount);
        coinEffect.Play();
        GameManager.Instance.saveModule.saveInfo.coinCount = coinCount;
        OnCoinCurrencyChanged?.Invoke(coinCount - previousAmount);
    }

    public void RemoveCoin(int amount)
    {
        int previousAmount = coinCount;
        coinCount -= amount;
        coinText.text = NumberFormatter.Convert(coinCount);
        GameManager.Instance.saveModule.saveInfo.coinCount = coinCount;
        OnCoinCurrencyChanged?.Invoke(coinCount - previousAmount);
    }

    public void AddGem(int amount)
    {
        int previousAmount = gemCount;
        gemCount += amount;
        gemText.text = NumberFormatter.Convert(gemCount);
        gemEffect.Play();
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
        OnGemCurrencyChanged?.Invoke(gemCount - previousAmount);
    }

    public void RemoveGem(int amount)
    {
        int previousAmount = gemCount;
        gemCount -= amount;
        gemText.text = NumberFormatter.Convert(gemCount);
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
        OnGemCurrencyChanged?.Invoke(gemCount - previousAmount);
    }

    public void AddWood(int amount)
    {
        int previousAmount = woodAmount;
        woodAmount += amount;
        woodText.text = NumberFormatter.Convert(woodAmount);
        woodEffect.Play();
        GameManager.Instance.saveModule.saveInfo.woodAmount = woodAmount;
        OnWoodCurrencyChanged?.Invoke(woodAmount - previousAmount);
    }

    public void RemoveWood(int amount)
    {
        int previousAmount = woodAmount;
        woodAmount -= amount;
        woodText.text = NumberFormatter.Convert(woodAmount);
        GameManager.Instance.saveModule.saveInfo.woodAmount = woodAmount;
        OnWoodCurrencyChanged?.Invoke(woodAmount - previousAmount);
    }

    public void AddStone(int amount)
    {
        int previousAmount = stoneAmount;
        stoneAmount += amount;
        stoneText.text = NumberFormatter.Convert(stoneAmount);
        stoneEffect.Play();
        GameManager.Instance.saveModule.saveInfo.stoneAmount = stoneAmount;
        OnStoneCurrencyChanged?.Invoke(stoneAmount - previousAmount);
    }

    public void RemoveStone(int amount)
    {
        int previousAmount = stoneAmount;
        stoneAmount -= amount;
        stoneText.text = NumberFormatter.Convert(stoneAmount);
        GameManager.Instance.saveModule.saveInfo.stoneAmount = stoneAmount;
        OnStoneCurrencyChanged?.Invoke(stoneAmount - previousAmount);
    }
}
