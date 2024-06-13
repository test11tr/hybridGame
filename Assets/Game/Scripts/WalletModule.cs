using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalletModule : MonoBehaviour
{
    public static WalletModule Instance;

    public TMP_Text goldText;
    public TMP_Text gemText;

    private int goldCount;
    private int gemCount;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); 
        }
    }

    private void Start()
    {
        goldCount = SaveModule.Instance.saveInfo.goldCount;
        gemCount = SaveModule.Instance.saveInfo.gemCount;
        goldText.text = goldCount.ToString();
        gemText.text = gemCount.ToString();
    }

    public void AddGold(int amount)
    {
        goldCount += amount;
        goldText.text = goldCount.ToString();
        SaveModule.Instance.saveInfo.goldCount = goldCount;
    }

    public void AddGem(int amount)
    {
        gemCount += amount;
        gemText.text = gemCount.ToString();
        SaveModule.Instance.saveInfo.gemCount = gemCount;
    }
}
