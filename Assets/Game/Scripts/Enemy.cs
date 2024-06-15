using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;
    private int currentHealth;

    [Header("Enemy Damage")]
    public int damage;
    public int destroyTime;

    [Header("References")]
    public Rigidbody rb;
    public Animator enemyAnimator;
    public GameObject floatingTextPrefab;
    public Rigidbody target;


    private bool isDead;

    void Start()
    {
        target = GameManager.Instance.player.rb;
        currentHealth = maxHealth;
    }

    void Update()
    {
        CheckHealthForUI();         
    }

     private void CheckHealthForUI()
    {
        if(currentHealth < maxHealth)
        {
            healthModuleCanvas.SetActive(true);
        }
        else
        {
            healthModuleCanvas.SetActive(false);
        }

        if(healthBar.fillAmount != healthBarEase.fillAmount)
        {
            healthBarEase.fillAmount = Mathf.Lerp(healthBarEase.fillAmount, healthBar.fillAmount, healthEaseSpeed * Time.deltaTime);
        }
    }

    void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

    void Die()
    {
        enemyAnimator.SetBool("isDead", true);
        isDead = true;
        DelayHelper.DelayAction(destroyTime, () =>
        {
            Destroy(gameObject);
        });

    }
}
