using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalletModule : MonoBehaviour
{

    public TMP_Text coinText;
    public TMP_Text gemText;

    private int coinCount;
    private int gemCount;

    private void Start()
    {
        coinCount = GameManager.Instance.saveModule.saveInfo.coinCount;
        gemCount = GameManager.Instance.saveModule.saveInfo.gemCount;
        coinText.text = coinCount.ToString();
        gemText.text = gemCount.ToString();
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
}
