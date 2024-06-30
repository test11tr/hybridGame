using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsModule : MonoBehaviour
{
    public enum WeaponType
    {
        MeleeAttack,
        RangedAttack,
        RotatingBlades
    }

    [Header("Player")]
    public SaveModule saveModule;
    public CharacterControlManager playerManager;

    [Foldout("Default Movement Module", foldEverything = true, styled = true, readOnly = false)]
    public bool isJoytick;
    public float movementSpeed;
    public float rotationSpeed;

    [Foldout("Default Dash Module", foldEverything = true, styled = true, readOnly = false)]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldownTime;

    [Foldout("Default Combat Module", foldEverything = true, styled = true, readOnly = false)]
    public WeaponType weaponType;
    public float rangedAttackRange;
    public float meleeAttackRange;
    public float detectorRange;
    public float damage;
    public float criticalChance;
    public float criticalMultiplier;
    public float splashDamageMultiplier;
    public float attackSpeedRange;
    public float attackSpeedMelee;

    [Foldout("Default Health Module", foldEverything = true, styled = true, readOnly = false)]
    public float maxHealth;
    public float healthEaseSpeed;

    [Foldout("Default Level / Experience Module", foldEverything = true, styled = true, readOnly = false)]
    public int maxExperience;
    public int levelExperienceMultiplier;

    [Foldout("Incremental Movement Module", foldEverything = true, styled = true, readOnly = false)]
    public float movementSpeedIncreaseMultiplier;
    public int movementSpeedMaxLevel;

    [Foldout("Incremental Module", foldEverything = true, styled = true, readOnly = false)]
    public float damageIncreaseMultiplier;
    public int damageMaxLevel;
    public float criticalChanceIncreaseMultiplier;
    public float criticalChanceMaxLevel;
    public float criticalMultiplierIncreaseMultiplier;
    public float criticalMultiplierMaxLevel;
    public float AttackRangeIncreaseMultiplier;
    public int AttackRangeMaxLevel;
    public float attackSpeedIncreaseMultiplier;
    public int attackSpeedMaxLevel;
    public float healthIncreaseMultiplier;
    public float healthMaxLevel;

    [Foldout("Save Values", foldEverything = true, styled = true, readOnly = true)]
    public float movementSpeedCurrentValue;
    public int movementSpeedCurrentLevel;
    public float damageCurrentValue;
    public int damageCurrentLevel;
    public float criticalChanceCurrentValue;
    public float criticalChanceCurrentLevel;
    public float criticalMultiplierCurrentValue;
    public float criticalMultiplierCurrentLevel;
    public float AttackRangeCurrentValueMelee;
    public float AttackRangeCurrentValueRange;
    public int AttackRangeCurrentLevel;
    public float detectorRangeValue;
    public float attackSpeedCurrentValueMelee;
    public float attackSpeedCurrentValueRange;
    public int attackSpeedCurrentLevel;
    public float healthCurrentValue;
    public float healthCurrentLevel;
    
    [Foldout("Percentage Increase", foldEverything = true, styled = true, readOnly = true)]
    public float movementSpeedPercentageIncrease;
    public float damagePercentageIncrease;
    public float criticalChancePercentageIncrease;
    public float criticalMultiplierPercentageIncrease;
    public float AttackRangePercentageIncrease;
    public float attackSpeedPercentageIncrease;
    public float healthPercentageIncrease;

    #region Preperation & Setters
    void Start()
    {
        saveModule = GameManager.Instance.saveModule;
        if (saveModule.CheckIsHaveSave())
        {
            LoadCharacterStats();
            SetPlayerManagerStats();
            playerManager.PrepareWeapon();
        }
        else
        {
            SetDefaultPlayerManagerStats();
            playerManager.PrepareWeapon();
            Debug.Log("No save data found! Setting default values.");
        }
        CalculatePercentageIncrease();
    }

    void SetDefaultPlayerManagerStats()
    {
        if (playerManager != null)
        {
            print("Set Default Player Manager Stats");
            playerManager.isJoytick = this.isJoytick;
            playerManager.movementSpeed = this.movementSpeed;
            playerManager.rotationSpeed = this.rotationSpeed;
            playerManager.dashSpeed = this.dashSpeed;
            playerManager.dashDuration = this.dashDuration;
            playerManager.dashCooldownTime = this.dashCooldownTime;
            playerManager.rangedAttackRange = this.rangedAttackRange;
            playerManager.meleeAttackRange = this.meleeAttackRange;
            playerManager.detectorRange = this.detectorRange;
            playerManager.damage = this.damage;
            playerManager.criticalChance = this.criticalChance;
            playerManager.criticalMultiplier = this.criticalMultiplier;
            playerManager.splashDamageMultiplier = this.splashDamageMultiplier;
            playerManager.attackSpeedRange = this.attackSpeedRange;
            playerManager.attackSpeedMelee = this.attackSpeedMelee;
            playerManager.maxHealth = this.maxHealth;
            playerManager.healthEaseSpeed = this.healthEaseSpeed;
            playerManager.maxExperience = this.maxExperience;
            playerManager.levelExperienceMultiplier = this.levelExperienceMultiplier;

            if(weaponType.Equals(WeaponType.MeleeAttack))
            {
                playerManager.isMeleeAttack = true;
                playerManager.isRangedAttack = false;
                playerManager.isRotatingBlades = false;
            }
            else if(weaponType.Equals(WeaponType.RangedAttack))
            {
                playerManager.isMeleeAttack = false;
                playerManager.isRangedAttack = true;
                playerManager.isRotatingBlades = false;
            }
            else if(weaponType.Equals(WeaponType.RotatingBlades))
            {
                playerManager.isMeleeAttack = false;
                playerManager.isRangedAttack = false;
                playerManager.isRotatingBlades = true;
            }
        }
    }

    void SetPlayerManagerStats()
    {
        if (playerManager != null)
        {
            playerManager.isJoytick = this.isJoytick;
            playerManager.movementSpeed = movementSpeedCurrentValue;
            playerManager.rotationSpeed = this.rotationSpeed;
            playerManager.dashSpeed = this.dashSpeed;
            playerManager.dashDuration = this.dashDuration;
            playerManager.dashCooldownTime = this.dashCooldownTime;
            playerManager.rangedAttackRange = AttackRangeCurrentValueRange;
            playerManager.meleeAttackRange = AttackRangeCurrentValueMelee;
            playerManager.detectorRange = detectorRangeValue;
            playerManager.damage = damageCurrentValue;
            playerManager.criticalChance = criticalChanceCurrentValue;
            playerManager.criticalMultiplier = criticalMultiplierCurrentValue;
            playerManager.splashDamageMultiplier = this.splashDamageMultiplier;
            playerManager.attackSpeedRange = attackSpeedCurrentValueRange;
            playerManager.attackSpeedMelee = attackSpeedCurrentValueMelee;
            playerManager.maxHealth = healthCurrentValue;
            playerManager.healthEaseSpeed = this.healthEaseSpeed;
            playerManager.maxExperience = this.maxExperience;
            playerManager.levelExperienceMultiplier = this.levelExperienceMultiplier;

            if(weaponType.Equals(WeaponType.MeleeAttack))
            {
                playerManager.isMeleeAttack = true;
                playerManager.isRangedAttack = false;
                playerManager.isRotatingBlades = false;
            }
            else if(weaponType.Equals(WeaponType.RangedAttack))
            {
                playerManager.isMeleeAttack = false;
                playerManager.isRangedAttack = true;
                playerManager.isRotatingBlades = false;
            }
            else if(weaponType.Equals(WeaponType.RotatingBlades))
            {
                playerManager.isMeleeAttack = false;
                playerManager.isRangedAttack = false;
                playerManager.isRotatingBlades = true;
            }
        }
    }
    #endregion

    #region Incremental Modules
    public void IncrementMovementSpeed()
    {
        if (movementSpeedCurrentLevel < movementSpeedMaxLevel)
        {
            movementSpeedCurrentLevel++;
            movementSpeed *= movementSpeedIncreaseMultiplier;
            playerManager.movementSpeed = movementSpeed;
            movementSpeedCurrentValue = movementSpeed;
            SaveMovementSpeed();
        }
    }

    public void IncrementDamage()
    {
        if (damageCurrentLevel < damageMaxLevel)
        {
            damageCurrentLevel++;
            damage *= damageIncreaseMultiplier;
            playerManager.damage = damage;
            damageCurrentValue = damage;
            SaveDamage();
        }
    }

    public void IncrementAttackRange()
    {
        if (AttackRangeCurrentLevel < AttackRangeMaxLevel)
        {
            AttackRangeCurrentLevel++;
            rangedAttackRange *= AttackRangeIncreaseMultiplier;
            meleeAttackRange *= AttackRangeIncreaseMultiplier;
            playerManager.rangedAttackRange = rangedAttackRange;
            playerManager.meleeAttackRange = meleeAttackRange;
            if(meleeAttackRange > detectorRange || rangedAttackRange > detectorRange)
            {
                detectorRange *= AttackRangeIncreaseMultiplier;
                playerManager.detectorRange = detectorRange;
            }
            playerManager.updateAttackRangeVisualizer();
            AttackRangeCurrentValueMelee = meleeAttackRange;
            AttackRangeCurrentValueRange = rangedAttackRange;
            detectorRangeValue = detectorRange;
            SaveAttackRange();
        }
    }

    public void IncrementHealth()
    {
        if (healthCurrentLevel < healthMaxLevel)
        {
            healthCurrentLevel++;
            maxHealth *= healthIncreaseMultiplier;
            playerManager.maxHealth = maxHealth;
            playerManager.currentHealth *= healthIncreaseMultiplier;
            healthCurrentValue = maxHealth;
            SaveHealth();
        }
    }

    public void IncrementAttackSpeed()
    {
        if (attackSpeedCurrentLevel < attackSpeedMaxLevel)
        {
            attackSpeedCurrentLevel++;
            attackSpeedRange *= attackSpeedIncreaseMultiplier;
            attackSpeedMelee *= attackSpeedIncreaseMultiplier;
            playerManager.attackSpeedRange = attackSpeedRange;
            playerManager.attackSpeedMelee = attackSpeedMelee;
            attackSpeedCurrentValueMelee = attackSpeedMelee;
            attackSpeedCurrentValueRange = attackSpeedRange;
            SaveAttackSpeed();
        }
    }

    public void IncrementCriticalChance()
    {
        if (criticalChanceCurrentLevel < criticalChanceMaxLevel)
        {
            criticalChanceCurrentLevel++;
            criticalChance *= criticalChanceIncreaseMultiplier;
            playerManager.criticalChance = criticalChance;
            criticalChanceCurrentValue = criticalChance;
            SaveCriticalChance();
        }
    }

    public void IncrementCriticalMultiplier()
    {
        if (criticalMultiplierCurrentLevel < criticalMultiplierMaxLevel)
        {
            criticalMultiplierCurrentLevel++;
            criticalMultiplier *= criticalMultiplierIncreaseMultiplier;
            playerManager.criticalMultiplier = criticalMultiplier;
            criticalMultiplierCurrentValue = criticalMultiplier;
            SaveCriticalMultiplier();
        }
    }
    #endregion

    #region Percentage Calculation Methods
    float CalculatePercentageIncrease(float original, float multiplier)
    {
        float newNumber = original * multiplier;
        float increase = newNumber - original;
        float percentageIncrease = (increase / original) * 100f;

        return percentageIncrease;
    }

    private void CalculatePercentageIncrease()
    {
        movementSpeedPercentageIncrease = CalculatePercentageIncrease(movementSpeed, movementSpeedIncreaseMultiplier);
        damagePercentageIncrease = CalculatePercentageIncrease(damage, damageIncreaseMultiplier);
        criticalChancePercentageIncrease = CalculatePercentageIncrease(criticalChance, criticalChanceIncreaseMultiplier);
        criticalMultiplierPercentageIncrease = CalculatePercentageIncrease(criticalMultiplier, criticalMultiplierIncreaseMultiplier);
        healthPercentageIncrease = CalculatePercentageIncrease(maxHealth, healthIncreaseMultiplier);

        if(weaponType.Equals(WeaponType.MeleeAttack))
        {
            AttackRangePercentageIncrease = CalculatePercentageIncrease(meleeAttackRange, AttackRangeIncreaseMultiplier);
            attackSpeedPercentageIncrease = CalculatePercentageIncrease(attackSpeedMelee, attackSpeedIncreaseMultiplier);
        }
        else if(weaponType.Equals(WeaponType.RangedAttack))
        {
            AttackRangePercentageIncrease = CalculatePercentageIncrease(rangedAttackRange, AttackRangeIncreaseMultiplier);
            attackSpeedPercentageIncrease = CalculatePercentageIncrease(attackSpeedRange, attackSpeedIncreaseMultiplier);
        }
        else if(weaponType.Equals(WeaponType.RotatingBlades))
        {
            //AttackRangePercentageIncrease = CalculatePercentageIncrease(rangedAttackRange, AttackRangeIncreaseMultiplier);
            //attackSpeedPercentageIncrease = CalculatePercentageIncrease(attackSpeedRange, attackSpeedIncreaseMultiplier);
        }
    }
    #endregion

    #region SaveData
    public void SaveMovementSpeed()
    {
        saveModule.saveInfo.movementSpeedCurrentValue = this.movementSpeed;
        saveModule.saveInfo.movementSpeedCurrentLevel = this.movementSpeedCurrentLevel;
    }

    public void SaveDamage()
    {
        saveModule.saveInfo.damageCurrentValue = this.damage;
        saveModule.saveInfo.damageCurrentLevel = this.damageCurrentLevel;
    }

    public void SaveCriticalChance()
    {
        saveModule.saveInfo.criticalChanceCurrentValue = this.criticalChance;
        saveModule.saveInfo.criticalChanceCurrentLevel = this.criticalChanceCurrentLevel;
    }

    public void SaveCriticalMultiplier()
    {
        saveModule.saveInfo.criticalMultiplierCurrentValue = this.criticalMultiplier;
        saveModule.saveInfo.criticalMultiplierCurrentLevel = this.criticalMultiplierCurrentLevel;
    }

    public void SaveAttackRange()
    {
        saveModule.saveInfo.AttackRangeCurrentValueMelee = this.meleeAttackRange;
        saveModule.saveInfo.AttackRangeCurrentValueRange = this.rangedAttackRange;
        saveModule.saveInfo.AttackRangeCurrentLevel = this.AttackRangeCurrentLevel;
        saveModule.saveInfo.detectorRange = this.detectorRange;
    }

    public void SaveAttackSpeed()
    {
        saveModule.saveInfo.attackSpeedCurrentValueMelee = this.attackSpeedMelee;
        saveModule.saveInfo.attackSpeedCurrentValueRange = this.attackSpeedRange;
        saveModule.saveInfo.attackSpeedCurrentLevel = this.attackSpeedCurrentLevel;
    }

    public void SaveHealth()
    {
        saveModule.saveInfo.healthCurrentValue = this.maxHealth;
        saveModule.saveInfo.healthCurrentLevel = this.healthCurrentLevel;
        saveModule.saveInfo.characterCurrentHealth = playerManager.currentHealth;
    }

    public void LoadCharacterStats()
    {
        if (saveModule.saveInfo.movementSpeedCurrentValue != 0)
        {
            this.movementSpeedCurrentValue = saveModule.saveInfo.movementSpeedCurrentValue;
            this.movementSpeedCurrentLevel = saveModule.saveInfo.movementSpeedCurrentLevel;
            //playerManager.movementSpeed = movementSpeedCurrentValue;
        }
        else
        {
            this.movementSpeedCurrentValue = movementSpeed;
            this.movementSpeedCurrentLevel = 0;
            //playerManager.movementSpeed = movementSpeed;
        }

        if (saveModule.saveInfo.damageCurrentValue != 0)
        {
            this.damageCurrentValue = saveModule.saveInfo.damageCurrentValue;
            this.damageCurrentLevel = saveModule.saveInfo.damageCurrentLevel;
            //playerManager.damage = damageCurrentValue;
        }
        else
        {
            this.damageCurrentValue = damage;
            this.damageCurrentLevel = 0;
            //playerManager.damage = damage;
        }

        if (saveModule.saveInfo.criticalChanceCurrentValue != 0)
        {
            this.criticalChanceCurrentValue = saveModule.saveInfo.criticalChanceCurrentValue;
            this.criticalChanceCurrentLevel = saveModule.saveInfo.criticalChanceCurrentLevel;
            //playerManager.criticalChance = criticalChanceCurrentValue;
        }
        else
        {
            this.criticalChanceCurrentValue = criticalChance;
            this.criticalChanceCurrentLevel = 0;
            //playerManager.criticalChance = criticalChance;
        }

        if (saveModule.saveInfo.criticalMultiplierCurrentValue != 0)
        {
            this.criticalMultiplierCurrentValue = saveModule.saveInfo.criticalMultiplierCurrentValue;
            this.criticalMultiplierCurrentLevel = saveModule.saveInfo.criticalMultiplierCurrentLevel;
            playerManager.criticalMultiplier = criticalMultiplierCurrentValue;
        }
        else
        {
            this.criticalMultiplierCurrentValue = criticalMultiplier;
            this.criticalMultiplierCurrentLevel = 0;
            //playerManager.criticalMultiplier = criticalMultiplier;
        }

        if(saveModule.saveInfo.AttackRangeCurrentValueMelee != 0 || saveModule.saveInfo.AttackRangeCurrentValueRange != 0)
            {
                this.AttackRangeCurrentValueMelee = saveModule.saveInfo.AttackRangeCurrentValueMelee;
                this.AttackRangeCurrentValueRange = saveModule.saveInfo.AttackRangeCurrentValueRange;
                this.detectorRangeValue = saveModule.saveInfo.detectorRange;
                this.AttackRangeCurrentLevel = saveModule.saveInfo.AttackRangeCurrentLevel;
                //playerManager.meleeAttackRange = AttackRangeCurrentValueMelee;
            }
        else
        {
            this.AttackRangeCurrentValueMelee = meleeAttackRange;
            this.AttackRangeCurrentValueRange = rangedAttackRange;
            this.detectorRangeValue = detectorRange;
            this.AttackRangeCurrentLevel = 0;
            //playerManager.meleeAttackRange = meleeAttackRange;
        }

        if(saveModule.saveInfo.attackSpeedCurrentValueMelee != 0 || saveModule.saveInfo.attackSpeedCurrentValueRange != 0)
        {
            this.attackSpeedCurrentValueMelee = saveModule.saveInfo.attackSpeedCurrentValueMelee;
            this.attackSpeedCurrentValueRange = saveModule.saveInfo.attackSpeedCurrentValueRange;
            this.attackSpeedCurrentLevel = saveModule.saveInfo.attackSpeedCurrentLevel;
            //playerManager.attackSpeedMelee = attackSpeedCurrentValueMelee;
        }
        else
        {
            this.attackSpeedCurrentValueMelee = attackSpeedMelee;
            this.attackSpeedCurrentValueRange = attackSpeedRange;
            this.attackSpeedCurrentLevel = 0;
            //playerManager.attackSpeedMelee = attackSpeedMelee;
        }

        if (saveModule.saveInfo.healthCurrentValue != 0)
        {
            this.healthCurrentValue = saveModule.saveInfo.healthCurrentValue;
            this.healthCurrentLevel = saveModule.saveInfo.healthCurrentLevel;
            //playerManager.maxHealth = healthCurrentValue;
        }
        else
        {
            this.healthCurrentValue = maxHealth;
            this.healthCurrentLevel = 0;
            //playerManager.maxHealth = maxHealth;
        }
    }
    #endregion
}
