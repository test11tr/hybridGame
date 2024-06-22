using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalletModule : MonoBehaviour
{

    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text woodText;
    public TMP_Text stoneText;

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
        GameManager.Instance.saveModule.saveInfo.coinCount = coinCount;
    }

    public void AddGem(int amount)
    {
        gemCount += amount;
        gemText.text = gemCount.ToString();
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
    }

    public void AddWood(int amount)
    {
        woodAmount += amount;
        woodText.text = woodAmount.ToString();
        GameManager.Instance.saveModule.saveInfo.woodAmount = woodAmount;
    }

    public void AddStone(int amount)
    {
        stoneAmount += amount;
        stoneText.text = stoneAmount.ToString();
        GameManager.Instance.saveModule.saveInfo.stoneAmount = stoneAmount;
    }
}
