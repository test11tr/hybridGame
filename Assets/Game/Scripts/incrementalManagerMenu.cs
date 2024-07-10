using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements.Experimental;
using System.Runtime.InteropServices;
using System;

public class incrementalManagerMenu : MonoBehaviour
{
    public SaveModule saveModule;
    public WalletModule walletModule;
    [Foldout("Character Stats Module (Gets Automatic)", foldEverything = true, styled = true, readOnly = true)]
    public CharacterStatsModule statsModule;
    [Foldout("Text References", foldEverything = true, styled = true, readOnly = false)]
    public TMP_Text movementSpeedText;
    public TMP_Text movementSpeedCurrentLevelText;
    public TMP_Text damageText;
    public TMP_Text damageCurrentLevelText;
    public TMP_Text attackRangeText;
    public TMP_Text attackRangeCurrentLevelText;
    public TMP_Text criticalChanceText;
    public TMP_Text criticalChanceCurrentLevelText;
    public TMP_Text criticalMultiplierText;
    public TMP_Text criticalMultiplierCurrentLevelText;
    public TMP_Text attackSpeedText;
    public TMP_Text attackSpeedCurrentLevelText;
    public TMP_Text healthText;
    public TMP_Text healthCurrentLevelText;
    [Foldout("Button References", foldEverything = true, styled = true, readOnly = false)]
    public Button movementSpeedBuyButton;
    public Image movementSpeedCurrencyIcon;
    public TMP_Text movementSpeedCostText;
    public Button damageBuyButton;
    public Image damageCurrencyIcon;
    public TMP_Text damageCostText;
    public Button attackRangeBuyButton;
    public Image attackRangeCurrencyIcon;
    public TMP_Text attackRangeCostText;
    public Button criticalChanceBuyButton;
    public Image criticalChanceCurrencyIcon;
    public TMP_Text criticalChanceCostText;
    public Button criticalMultiplierBuyButton;
    public Image criticalMultiplierCurrencyIcon;
    public TMP_Text criticalMultiplierCostText;
    public Button attackSpeedBuyButton;
    public Image attackSpeedCurrencyIcon;  
    public TMP_Text attackSpeedCostText; 
    public Button healthBuyButton;
    public Image healthCurrencyIcon;
    public TMP_Text healthCostText;
    [Foldout("Currency Settings", foldEverything = true, styled = true, readOnly = false)]
    public float movementSpeedCost;
    public float movementSpeedCostMultiplier;
    public float damageCost;
    public float damageCostMultiplier;
    public float attackRangeCost;
    public float attackRangeCostMultiplier;
    public float criticalChanceCost;
    public float criticalChanceCostMultiplier;
    public float criticalMultiplierCost;
    public float criticalMultiplierCostMultiplier;
    public float attackSpeedCost;
    public float attackSpeedCostMultiplier;
    public float healthCost;
    public float healthCostMultiplier;

    void Start()
    {
        if(statsModule == null)
        {
            statsModule = GameManager.Instance.player.statsModule;
        }
        if(walletModule == null)
        {
            walletModule = GameManager.Instance.wallet;
        }
        if(saveModule == null)
        {
            saveModule = GameManager.Instance.saveModule;
        }

        CheckSave();
        SetButtons();
        CheckButtonsWithWallet();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        CheckButtonsWithWallet();
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void SetButtons()
    {
        movementSpeedBuyButton.onClick.AddListener(IncrementMovementSpeed);
        damageBuyButton.onClick.AddListener(IncrementDamage);
        attackRangeBuyButton.onClick.AddListener(IncrementAttackRange);
        criticalChanceBuyButton.onClick.AddListener(IncrementCriticalChance);
        criticalMultiplierBuyButton.onClick.AddListener(IncrementCriticalMultiplier);
        attackSpeedBuyButton.onClick.AddListener(IncrementAttackSpeed);
        healthBuyButton.onClick.AddListener(IncrementHealth);
    }

    public void CheckSave()
    {
        if (saveModule.CheckIsHaveSave())
        {
            print("Update Stats UI");
            UpdateUI();
        }
        else
        {
            SetDefaultUI();
            Debug.Log("No save data found! Setting default values for UI.");
        }
    }

    public void UpdateUI()
    {
        if (statsModule.movementSpeedCurrentValue != 0)
        {
            movementSpeedText.text = "Current: " + statsModule.movementSpeedCurrentValue.ToString("F1") + "<style=green> +%" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
            movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        }
        else
        {
            movementSpeedText.text = "Current: " + statsModule.movementSpeed.ToString("F1") + "<style=green> +%" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
            movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        }

        if (statsModule.damageCurrentValue != 0)
        {
            damageText.text = "Current: " + statsModule.damageCurrentValue.ToString("F1") + "<style=green> +%" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
            damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        }
        else
        {
            damageText.text = "Current: " + statsModule.damage.ToString("F1") + "<style=green> +%" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
            damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        }

        if (statsModule.criticalChanceCurrentValue != 0)
        {
            criticalChanceText.text = "Current: " + statsModule.criticalChanceCurrentValue.ToString("F2") + "<style=green> +%" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
            criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        }
        else
        {
            criticalChanceText.text = "Current: " + statsModule.criticalChance.ToString("F2") + "<style=green> +%" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
            criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        }

        if (statsModule.criticalMultiplierCurrentValue != 0)
        {
            criticalMultiplierText.text = "Current:  Dmg x " + statsModule.criticalMultiplierCurrentValue.ToString("F1") + "<style=green> +%" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
            criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        }
        else
        {
            criticalMultiplierText.text = "Current:  Dmg x " + statsModule.criticalMultiplier.ToString("F1") + "<style=green> +%" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
            criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        }

        if(statsModule.weaponType == CharacterStatsModule.WeaponType.MeleeAttack)
        {
            if(statsModule.AttackRangeCurrentValueMelee != 0)
            {
                attackRangeText.text = "Current: " + statsModule.AttackRangeCurrentValueMelee.ToString("F1") + "<style=green> +%" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }
            else
            {
                attackRangeText.text = "Current: " + statsModule.meleeAttackRange.ToString("F1") + "<style=green> +%" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }

            if(statsModule.attackSpeedCurrentValueMelee != 0)
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedCurrentValueMelee.ToString("F1") + "<style=green> +%" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
            else
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedMelee.ToString("F1") + "<style=green> +%" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
        }
        else if(statsModule.weaponType == CharacterStatsModule.WeaponType.RangedAttack)
        {
            if(statsModule.AttackRangeCurrentValueRange != 0)
            {
                attackRangeText.text = "Current: " + statsModule.attackSpeedCurrentValueRange.ToString("F1") + "<style=green> +%" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }
            else
            {
                attackRangeText.text = "Current: " + statsModule.rangedAttackRange.ToString("F1") + "<style=green> +%" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }

            if(statsModule.attackSpeedCurrentValueRange != 0)
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedCurrentValueRange.ToString("F1") + "<style=green> +%" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
            else
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedRange.ToString("F1") + "<style=green> +%" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
        }
        else if(statsModule.weaponType == CharacterStatsModule.WeaponType.RotatingBlades)
        {
            //
        }

        if (statsModule.healthCurrentValue != 0)
        {
            healthText.text = "Current: " + statsModule.healthCurrentValue.ToString("F1") + "<style=green> +%" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
            healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
        }
        else
        {
            healthText.text = "Current Max Health: " + statsModule.maxHealth.ToString("F1") + "<style=green> +%" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
            healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
        }
    }

    public void SetDefaultUI()
    {
        movementSpeedText.text = "Current: " + statsModule.movementSpeed.ToString("F1") + "<style=green> +%" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
        movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        damageText.text = "Current: " + statsModule.damage.ToString("F1") + "<style=green> +%" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
        damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        attackRangeText.text = "Current: " + statsModule.rangedAttackRange.ToString("F1") + "<style=green> +%" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
        attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
        criticalChanceText.text = "Current: " + statsModule.criticalChance.ToString("F2") + "<style=green> +%" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
        criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        criticalMultiplierText.text = "Current:  Dmg x " + statsModule.criticalMultiplier.ToString("F1") + "<style=green> +%" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
        criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        attackSpeedText.text = "Current: " + statsModule.attackSpeedRange.ToString("F1") + "<style=green> +%" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
        attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
        healthText.text = "Current Max Health: " + statsModule.maxHealth.ToString("F1") + "<style=green> +%" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
        healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
    }

    public void IncrementMovementSpeed()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementMovementSpeed();
        walletModule.DeductCurrency("coin", movementSpeedCost);
        movementSpeedCost *= movementSpeedCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementDamage()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementDamage();
        walletModule.DeductCurrency("coin", damageCost);
        damageCost *= damageCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementAttackRange()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementAttackRange();
        walletModule.DeductCurrency("coin", attackRangeCost);
        attackRangeCost *= attackRangeCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementHealth()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementHealth();
        walletModule.DeductCurrency("coin", healthCost);
        healthCost *= healthCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementAttackSpeed()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementAttackSpeed();
        walletModule.DeductCurrency("coin", attackSpeedCost);
        attackSpeedCost *= attackSpeedCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementCriticalChance()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementCriticalChance();
        walletModule.DeductCurrency("coin", criticalChanceCost);
        criticalChanceCost *= criticalChanceCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void IncrementCriticalMultiplier()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        statsModule.IncrementCriticalMultiplier();
        walletModule.DeductCurrency("coin", criticalMultiplierCost);
        criticalMultiplierCost *= criticalMultiplierCostMultiplier;
        UpdateUI();
        CheckButtonsWithWallet();
    }

    public void CheckButtonsWithWallet()
    {
        movementSpeedBuyButton.interactable = walletModule.coinCount >= movementSpeedCost;
        damageBuyButton.interactable = walletModule.coinCount >= damageCost;
        attackRangeBuyButton.interactable = walletModule.coinCount >= attackRangeCost;
        healthBuyButton.interactable = walletModule.coinCount >= healthCost;
        attackSpeedBuyButton.interactable = walletModule.coinCount >= attackSpeedCost;
        criticalChanceBuyButton.interactable = walletModule.coinCount >= criticalChanceCost;
        criticalMultiplierBuyButton.interactable = walletModule.coinCount >= criticalMultiplierCost;

        movementSpeedCostText.text = movementSpeedCost.ToString("F2");
        damageCostText.text = damageCost.ToString("F2");
        attackRangeCostText.text = attackRangeCost.ToString("F2");
        criticalChanceCostText.text = criticalChanceCost.ToString("F2");
        criticalMultiplierCostText.text = criticalMultiplierCost.ToString("F2");
        attackSpeedCostText.text = attackSpeedCost.ToString("F2");
        healthCostText.text = healthCost.ToString("F2");
    }
}
