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
    private int currentCargo = 0;

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

    void Start()
    {
        currentCoin = GameManager.Instance.saveModule.saveInfo.virtualCoin;
        currentGem = GameManager.Instance.saveModule.saveInfo.virtualGem;
        currentWood = GameManager.Instance.saveModule.saveInfo.virtualWood;
        currentStone = GameManager.Instance.saveModule.saveInfo.virtualStone;
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
        if(currentCargo + amount > maxCargo)
        {
            Debug.Log("Inventory is full!");
            return;
        }else
        {
            currentCoin += amount;
            coinText.text = currentCoin.ToString();
            GameManager.Instance.saveModule.saveInfo.virtualCoin = currentCoin;
        }
    }

    public void AddGem(int amount)
    {
        if(currentCargo + amount > maxCargo)
        {
            Debug.Log("Inventory is full!");
            return;
        }else
        {
            currentGem += amount;
            gemText.text = currentGem.ToString();
            GameManager.Instance.saveModule.saveInfo.virtualGem = currentGem;
        }
    }

    public void AddWood(int amount)
    {
        if(currentCargo + amount > maxCargo)
        {
            Debug.Log("Inventory is full!");
            return;
        }else
        {
            currentWood += amount;
            woodText.text = currentWood.ToString();
            GameManager.Instance.saveModule.saveInfo.virtualWood = currentWood;
        }
    }

    public void AddStone(int amount)
    {
        if(currentCargo + amount > maxCargo)
        {
            Debug.Log("Inventory is full!");
            return;
        }else
        {
            currentStone += amount;
            stoneText.text = currentStone.ToString();
            GameManager.Instance.saveModule.saveInfo.virtualStone = currentStone;
        }
    }

    // Base'e varınca çağrılacak fonksiyon
    public void TransferToWallet()
    {
        // Burada Wallet Module'e transfer işlemleri yapılacak
        // Örneğin: walletModule.AddMoney(currentMoney);
        // currentMoney ve currentMaterials sıfırlanır, UI güncellenir
    }
}
