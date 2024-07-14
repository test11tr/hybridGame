using UnityEngine;
using DG.Tweening;
using System;

public class DamageArea : MonoBehaviour
{
    [Foldout("Damage Area Visual", foldEverything = true, styled = true, readOnly = false)]
    public Shapes.Disc progressShape; 
    
    [Foldout("Damage Area Settings", foldEverything = true, styled = true, readOnly = false)]
    public float damageWaitTime;
    public float repetativeDamageCooldown;
    public int damageAmount;

    private bool isPlayerInside = false;
    private float damageTimer = 0f;
    private float repetativeDamagedamageTimer = 0f;

    private void Update()
    {
        if (isPlayerInside)
        {
            damageTimer += Time.deltaTime;
            if(progressShape.AngRadiansEnd < 360)
            {
                float totalIncrease = 2 * Mathf.PI;
                float increasePerSecond = totalIncrease / damageWaitTime;
                progressShape.AngRadiansEnd += increasePerSecond * Time.deltaTime;
            }
                
            if (damageTimer >= damageWaitTime)
            {
                repetativeDamagedamageTimer += Time.deltaTime;
                if (repetativeDamagedamageTimer >= repetativeDamageCooldown)
                {
                    doDamage();
                    repetativeDamagedamageTimer = 0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            damageTimer = 0f; 
            repetativeDamagedamageTimer = 0f; 
            progressShape.AngRadiansEnd = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            damageTimer = 0f; 
            repetativeDamagedamageTimer = 0f; 
            progressShape.AngRadiansEnd = 0;
        }
    }

    private void doDamage()
    {
        GameManager.Instance.player.TakeDamage(damageAmount);
    }
}