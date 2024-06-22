using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AssetKits.ParticleImage;

public class WalletModule : MonoBehaviour
{
    [Header("Wallet - UI References")]
    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    
    [Header("Wallet - Effects")]
    public ParticleImage coinEffect;
    public ParticleImage gemEffect;
    public ParticleImage woodEffect;
    public ParticleImage stoneEffect;

    private int coinCount;
    private int gemCount;
    private int woodAmount;
    private int stoneAmount;

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

    public void AddCoin(int amount)
    {
        coinCount += amount;
        coinText.text = coinCount.ToString();
        coinEffect.Play();
        GameManager.Instance.saveModule.saveInfo.coinCount = coinCount;
    }

    public void AddGem(int amount)
    {
        gemCount += amount;
        gemText.text = gemCount.ToString();
        gemEffect.Play();
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
    }

    public void AddWood(int amount)
    {
        woodAmount += amount;
        woodText.text = woodAmount.ToString();
        woodEffect.Play();
        GameManager.Instance.saveModule.saveInfo.woodAmount = woodAmount;
    }

    public void AddStone(int amount)
    {
        stoneAmount += amount;
        stoneText.text = stoneAmount.ToString();
        stoneEffect.Play();
        GameManager.Instance.saveModule.saveInfo.stoneAmount = stoneAmount;
    }
}
