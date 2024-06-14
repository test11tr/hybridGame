using UnityEngine;
using DG.Tweening;
using System;

public class DamageArea : MonoBehaviour
{
    public Shapes.Disc progressShape; 
    public float damageWaitTime;
    public float repetativeDamageCooldown;

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
        print("OnTriggerEnter:   " + other.tag);
        if (other.CompareTag("Player"))
        {
            print("Compared:   " + other.tag);
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
        CharacterControlManager.Instance.TakeDamage(5);
    }
}