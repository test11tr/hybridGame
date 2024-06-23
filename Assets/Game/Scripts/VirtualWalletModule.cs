using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VirtualWalletModule : MonoBehaviour
{
    private Dictionary<GameObject, int> uiItemsToValues = new Dictionary<GameObject, int>();
    [Header("Wallet - Limits")]
    public int maxCargo = 100;
    [HideInInspector] public int currentCargo = 0;

    [Header("Wallet - UI References")]
    public TMP_Text currentCargoText;
    public GameObject coinUIItem;
    public GameObject gemUIItem;
    public GameObject woodUIItem;
    public GameObject stoneUIItem;
    public TMP_Text coinText;
    public TMP_Text gemText;
    public TMP_Text woodText;
    public TMP_Text stoneText;

    [Header("Wallet - UI Visuals")]
    public Image progressBar;

    [Header("Wallet - DebugPurposes")]
    public int currentCoin;
    public int currentGem;
    public int currentWood;
    public int currentStone;

    [Header("Wallet - References")]
    private WalletModule walletModule;
    private SaveModule saveModule;

    void Start()
    {
        walletModule = GameManager.Instance.wallet;
        saveModule = GameManager.Instance.saveModule;
        
        currentCoin = saveModule.saveInfo.virtualCoin;
        currentGem = saveModule.saveInfo.virtualGem;
        currentWood = saveModule.saveInfo.virtualWood;
        currentStone = saveModule.saveInfo.virtualStone;
        currentCargo = currentCoin + currentGem + currentWood + currentStone;

        uiItemsToValues.Add(coinUIItem, currentCoin);
        uiItemsToValues.Add(gemUIItem, currentGem);
        uiItemsToValues.Add(woodUIItem, currentWood);
        uiItemsToValues.Add(stoneUIItem, currentStone);

        UpdateUI();
    }

    void UpdateUI()
    {
        coinText.text = currentCoin.ToString();
        gemText.text = currentGem.ToString();
        woodText.text = currentWood.ToString();
        stoneText.text = currentStone.ToString();
    }

    public void Update()
    {
        currentCargoText.text = "CARGO: " + currentCargo + "/" + maxCargo;
        currentCargo = currentCoin + currentGem + currentWood + currentStone;
        progressBar.fillAmount = (float)currentCargo / maxCargo;

        uiItemsToValues[coinUIItem] = currentCoin;
        uiItemsToValues[gemUIItem] = currentGem;
        uiItemsToValues[woodUIItem] = currentWood;
        uiItemsToValues[stoneUIItem] = currentStone;

        if(currentCargo >= maxCargo)
        {
            progressBar.color = Color.red;
        }
        else
        {
            progressBar.color = Color.white;
        }

        foreach (var item in uiItemsToValues)
        {
            item.Key.SetActive(item.Value > 0);
        }
    }

    public void FixedUpdate()
    {
        transform.position = GameManager.Instance.player.rb.transform.position + new Vector3(0, 1.5f, -2.5f);
    }

    public void AddCoin(int amount)
    {
        currentCoin += amount;
        coinText.text = currentCoin.ToString();
        saveModule.saveInfo.virtualCoin = currentCoin;
    }

    public void AddGem(int amount)
    {
        currentGem += amount;
        gemText.text = currentGem.ToString();
        saveModule.saveInfo.virtualGem = currentGem;
    }

    public void AddWood(int amount)
    {
        currentWood += amount;
        woodText.text = currentWood.ToString();
        saveModule.saveInfo.virtualWood = currentWood;
    }

    public void AddStone(int amount)
    {
        currentStone += amount;
        stoneText.text = currentStone.ToString();
        saveModule.saveInfo.virtualStone = currentStone;
    }

    public void TransferToWallet()
    {
        if (walletModule != null)
        {
            if(currentCoin > 0)
                walletModule.AddCoin(currentCoin);
            if(currentGem > 0)
                walletModule.AddGem(currentGem);
            if(currentWood > 0)
                walletModule.AddWood(currentWood);
            if(currentStone > 0)
                walletModule.AddStone(currentStone);

            currentCoin = 0;
            currentGem = 0;
            currentWood = 0;
            currentStone = 0;

            UpdateUI();
        }
    }
}
