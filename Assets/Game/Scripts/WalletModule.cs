using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AssetKits.ParticleImage;

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
        coinText.text = NumberFormatter.Convert(coinCount);
        coinEffect.Play();
        GameManager.Instance.saveModule.saveInfo.coinCount = coinCount;
    }

    public void AddGem(int amount)
    {
        gemCount += amount;
        gemText.text = NumberFormatter.Convert(gemCount);
        gemEffect.Play();
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
    }

    public void AddWood(int amount)
    {
        woodAmount += amount;
        woodText.text = NumberFormatter.Convert(woodAmount);
        woodEffect.Play();
        GameManager.Instance.saveModule.saveInfo.woodAmount = woodAmount;
    }

    public void AddStone(int amount)
    {
        stoneAmount += amount;
        stoneText.text = NumberFormatter.Convert(stoneAmount);
        stoneEffect.Play();
        GameManager.Instance.saveModule.saveInfo.stoneAmount = stoneAmount;
    }
}
