using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements.Experimental;

public class incrementalManagerMenu : MonoBehaviour
{
    public SaveModule saveModule;
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
    

    void Start()
    {
        if(statsModule == null)
        {
            statsModule = GameManager.Instance.player.statsModule;
        }
        
        saveModule = GameManager.Instance.saveModule;
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
            movementSpeedText.text = "Current: " + statsModule.movementSpeedCurrentValue.ToString("F1") + "<style=green> +" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
            movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        }
        else
        {
            movementSpeedText.text = "Current: " + statsModule.movementSpeed.ToString("F1") + "<style=green> +" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
            movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        }

        if (statsModule.damageCurrentValue != 0)
        {
            damageText.text = "Current: " + statsModule.damageCurrentValue.ToString("F1") + "<style=green> +" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
            damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        }
        else
        {
            damageText.text = "Current: " + statsModule.damage.ToString("F1") + "<style=green> +" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
            damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        }

        if (statsModule.criticalChanceCurrentValue != 0)
        {
            criticalChanceText.text = "Current: " + statsModule.criticalChanceCurrentValue.ToString("F2") + "<style=green> +" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
            criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        }
        else
        {
            criticalChanceText.text = "Current: " + statsModule.criticalChance.ToString("F2") + "<style=green> +" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
            criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        }

        if (statsModule.criticalMultiplierCurrentValue != 0)
        {
            criticalMultiplierText.text = "Current:  Damage x " + statsModule.criticalMultiplierCurrentValue.ToString("F1") + "<style=green> +" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
            criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        }
        else
        {
            criticalMultiplierText.text = "Current:  Damage x " + statsModule.criticalMultiplier.ToString("F1") + "<style=green> +" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
            criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        }

        if(statsModule.weaponType == CharacterStatsModule.WeaponType.MeleeAttack)
        {
            if(statsModule.AttackRangeCurrentValueMelee != 0)
            {
                attackRangeText.text = "Current: " + statsModule.AttackRangeCurrentValueMelee.ToString("F1") + "<style=green> +" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }
            else
            {
                attackRangeText.text = "Current: " + statsModule.meleeAttackRange.ToString("F1") + "<style=green> +" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }

            if(statsModule.attackSpeedCurrentValueMelee != 0)
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedCurrentValueMelee.ToString("F1") + "<style=green> +" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
            else
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedMelee.ToString("F1") + "<style=green> +" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
        }
        else if(statsModule.weaponType == CharacterStatsModule.WeaponType.RangedAttack)
        {
            if(statsModule.AttackRangeCurrentValueRange != 0)
            {
                attackRangeText.text = "Current: " + statsModule.attackSpeedCurrentValueRange.ToString("F1") + "<style=green> +" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }
            else
            {
                attackRangeText.text = "Current: " + statsModule.rangedAttackRange.ToString("F1") + "<style=green> +" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
                attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
            }

            if(statsModule.attackSpeedCurrentValueRange != 0)
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedCurrentValueRange.ToString("F1") + "<style=green> +" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
            else
            {
                attackSpeedText.text = "Current: " + statsModule.attackSpeedRange.ToString("F1") + "<style=green> +" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
                attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
            }
        }
        else if(statsModule.weaponType == CharacterStatsModule.WeaponType.RotatingBlades)
        {
            //
        }

        if (statsModule.healthCurrentValue != 0)
        {
            healthText.text = "Current: " + statsModule.healthCurrentValue.ToString("F1") + "<style=green> +" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
            healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
        }
        else
        {
            healthText.text = "Current Max Health: " + statsModule.maxHealth.ToString("F1") + "<style=green> +" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
            healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
        }
    }

    public void SetDefaultUI()
    {
        movementSpeedText.text = "Current: " + statsModule.movementSpeed.ToString("F1") + "<style=green> +" + statsModule.movementSpeedPercentageIncrease.ToString("F1") + "</style>";
        movementSpeedCurrentLevelText.text = "Level: " + statsModule.movementSpeedCurrentLevel + " / " + statsModule.movementSpeedMaxLevel;
        damageText.text = "Current: " + statsModule.damage.ToString("F1") + "<style=green> +" + statsModule.damagePercentageIncrease.ToString("F1") + "</style>";
        damageCurrentLevelText.text = "Level: " + statsModule.damageCurrentLevel + " / " + statsModule.damageMaxLevel;
        attackRangeText.text = "Current: " + statsModule.rangedAttackRange.ToString("F1") + "<style=green> +" + statsModule.AttackRangePercentageIncrease.ToString("F1") + "</style>";   
        attackRangeCurrentLevelText.text = "Level: " + statsModule.AttackRangeCurrentLevel + " / " + statsModule.AttackRangeMaxLevel;  
        criticalChanceText.text = "Current: " + statsModule.criticalChance.ToString("F2") + "<style=green> +" + statsModule.criticalChancePercentageIncrease.ToString("F1") + "</style>";     
        criticalChanceCurrentLevelText.text = "Level: " + statsModule.criticalChanceCurrentLevel + " / " + statsModule.criticalChanceMaxLevel;
        criticalMultiplierText.text = "Current:  Damage x " + statsModule.criticalMultiplier.ToString("F1") + "<style=green> +" + statsModule.criticalMultiplierPercentageIncrease.ToString("F1") + "</style>";  
        criticalMultiplierCurrentLevelText.text = "Level: " + statsModule.criticalMultiplierCurrentLevel + " / " + statsModule.criticalMultiplierMaxLevel; 
        attackSpeedText.text = "Current: " + statsModule.attackSpeedRange.ToString("F1") + "<style=green> +" + statsModule.attackSpeedPercentageIncrease.ToString("F1") + "</style>";       
        attackSpeedCurrentLevelText.text = "Level: " + statsModule.attackSpeedCurrentLevel + " / " + statsModule.attackSpeedMaxLevel;
        healthText.text = "Current Max Health: " + statsModule.maxHealth.ToString("F1") + "<style=green> +" + statsModule.healthIncreaseMultiplier.ToString("F1") + "</style>";     
        healthCurrentLevelText.text = "Level: " + statsModule.healthCurrentLevel + " / " + statsModule.healthMaxLevel;
    }

    public void IncrementMovementSpeed()
    {
        statsModule.IncrementMovementSpeed();
        UpdateUI();
    }

    public void IncrementDamage()
    {
        statsModule.IncrementDamage();
        UpdateUI();
    }

    public void IncrementAttackRange()
    {
        statsModule.IncrementAttackRange();
        UpdateUI();
    }

    public void IncrementHealth()
    {
        statsModule.IncrementHealth();
        UpdateUI();
    }

    public void IncrementAttackSpeed()
    {
        statsModule.IncrementAttackSpeed();
        UpdateUI();
    }

    public void IncrementCriticalChance()
    {
        statsModule.IncrementCriticalChance();
        UpdateUI();
    }

    public void IncrementCriticalMultiplier()
    {
        statsModule.IncrementCriticalMultiplier();
        UpdateUI();
    }
}
