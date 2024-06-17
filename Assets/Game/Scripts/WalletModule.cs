using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalletModule : MonoBehaviour
{

    public TMP_Text goldText;
    public TMP_Text gemText;

    private int goldCount;
    private int gemCount;

    private void Start()
    {
        goldCount = GameManager.Instance.saveModule.saveInfo.goldCount;
        gemCount = GameManager.Instance.saveModule.saveInfo.gemCount;
        goldText.text = goldCount.ToString();
        gemText.text = gemCount.ToString();
    }

    public void AddGold(int amount)
    {
        goldCount += amount;
        goldText.text = goldCount.ToString();
        GameManager.Instance.saveModule.saveInfo.goldCount = goldCount;
    }

    public void AddGem(int amount)
    {
        gemCount += amount;
        gemText.text = gemCount.ToString();
        GameManager.Instance.saveModule.saveInfo.gemCount = gemCount;
    }
}
