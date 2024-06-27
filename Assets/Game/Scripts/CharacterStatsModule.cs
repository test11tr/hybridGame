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
    public float timeBetweenAttacksRanged;

    [Foldout("Default Health Module", foldEverything = true, styled = true, readOnly = false)]
    public float maxHealth;
    public float healthEaseSpeed;

    [Foldout("Default Level / Experience Module", foldEverything = true, styled = true, readOnly = false)]
    public int maxExperience;
    public int levelExperienceMultiplier;

    [Foldout("Incremental Movement Module", foldEverything = true, styled = true, readOnly = false)]
    public float movementSpeedIncreaseMultiplier;
    public int movementSpeedMaxLevel;
    public int movementSpeedCurrentLevel;

    [Foldout("Incremental Combat Module", foldEverything = true, styled = true, readOnly = false)]
    public int damageIncreaseMultiplier;
    public int damageMaxLevel;
    public int damageCurrentLevel;
    public float criticalChanceIncreaseMultiplier;
    public float criticalChanceMaxLevel;
    public float criticalChanceCurrentLevel;
    public float criticalMultiplierIncreaseMultiplier;
    public float criticalMultiplierMaxLevel;
    public float criticalMultiplierCurrentLevel;
    public float AttackRangeIncreaseMultiplier;
    public int AttackRangeMaxLevel;
    public int AttackRangeCurrentLevel;
    public float attackSpeedIncreaseMultiplier;
    public int attackSpeedMaxLevel;
    public int attackSpeedCurrentLevel;

    [Foldout("Default Health Module", foldEverything = true, styled = true, readOnly = false)]
    public float healthIncreaseMultiplier;
    public float healthMaxLevel;
    public float healthCurrentLevel;

    void Start()
    {
        SetPlayerManagerStats();
    }

    void SetPlayerManagerStats()
    {
        if (playerManager != null)
        {
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
            playerManager.timeBetweenAttacksRanged = this.timeBetweenAttacksRanged;
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

    public void IncrementMovementSpeed()
    {
        if (movementSpeedCurrentLevel < movementSpeedMaxLevel)
        {
            movementSpeedCurrentLevel++;
            movementSpeed *= movementSpeedIncreaseMultiplier;
            playerManager.movementSpeed = movementSpeed;
        }
    }

    public void IncrementDamage()
    {
        if (damageCurrentLevel < damageMaxLevel)
        {
            damageCurrentLevel++;
            damage *= damageIncreaseMultiplier;
            playerManager.damage = damage;
        }
    }

    public void IncrementAttackRange()
    {
        if (AttackRangeCurrentLevel < AttackRangeMaxLevel)
        {
            AttackRangeCurrentLevel++;
            rangedAttackRange *= AttackRangeIncreaseMultiplier;
            playerManager.rangedAttackRange = rangedAttackRange;
        }
    }

    public void IncrementHealth()
    {
        if (healthCurrentLevel < healthMaxLevel)
        {
            healthCurrentLevel++;
            maxHealth *= healthIncreaseMultiplier;
            playerManager.maxHealth = maxHealth;
        }
    }

    public void IncrementAttackSpeed()
    {
        if (attackSpeedCurrentLevel < attackSpeedMaxLevel)
        {
            attackSpeedCurrentLevel++;
            attackSpeedRange *= attackSpeedIncreaseMultiplier;
            playerManager.attackSpeedRange = attackSpeedRange;
        }
    }

    public void IncrementCriticalChance()
    {
        if (criticalChanceCurrentLevel < criticalChanceMaxLevel)
        {
            criticalChanceCurrentLevel++;
            criticalChance *= criticalChanceIncreaseMultiplier;
            playerManager.criticalChance = criticalChance;
        }
    }

    public void IncrementCriticalMultiplier()
    {
        if (criticalMultiplierCurrentLevel < criticalMultiplierMaxLevel)
        {
            criticalMultiplierCurrentLevel++;
            criticalMultiplier *= criticalMultiplierIncreaseMultiplier;
            playerManager.criticalMultiplier = criticalMultiplier;
        }
    }
}
