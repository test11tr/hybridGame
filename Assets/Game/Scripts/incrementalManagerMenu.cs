using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class incrementalManagerMenu : MonoBehaviour
{
    public CharacterStatsModule statsModule;

    void Start()
    {
        if(statsModule == null)
        {
            statsModule = GameManager.Instance.player.statsModule;
        }
    }

    public void IncrementMovementSpeed()
    {
        statsModule.IncrementMovementSpeed();
    }

    public void IncrementDamage()
    {
        statsModule.IncrementDamage();
    }

    public void IncrementAttackRange()
    {
        statsModule.IncrementAttackRange();
    }

    public void IncrementHealth()
    {
        statsModule.IncrementHealth();
    }

    public void IncrementAttackSpeed()
    {
        statsModule.IncrementAttackSpeed();
    }

    public void IncrementCriticalChance()
    {
        statsModule.IncrementCriticalChance();
    }

    public void IncrementCriticalMultiplier()
    {
        statsModule.IncrementCriticalMultiplier();
    }
}
