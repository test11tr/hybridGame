using System;
using System.Collections.Generic;
using EpicToonFX;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyStatic : MonoBehaviour
{
    [Header("Enemy Settings")]
    public bool isStatic = true;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;
    private int currentHealth;
    public int enemyDestroyTimeOnDead;

    [Header("Loot Module")]
    public LootBag lootBag;
    public int dropCount;

    [Header("Effects Module")]
    public ParticleSystem pufEffect;

    [Header("FlashModule")]
    public Renderer[] includedRenderers;
    private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
    private MeshRenderer[] allMeshRenderers;
    public Material flashMaterial;
    public Material deadMaterial;
    public Material[] originalMaterials;
    public float materialChangeDuration = .2f;

    [Header("References")]
    public floatingText floatingTextPrefab;
    public Transform lookTarget;
    private Transform player;
    public GameObject visual;

    [HideInInspector] public bool isDead;

    #region PreperationAndLoop
    void Start()
    {
        player = GameManager.Instance.player.rb.transform;
        currentHealth = maxHealth;
        GetAllRenderers();
    }

    void Update()
    {
        CheckHealthForUI();  
        if(isDead) 
        return;     
    }

    public void GetAllRenderers()
    {
        List<Renderer> tempIncludedRenderers = new List<Renderer>();
        SkinnedMeshRenderer[] allSkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] allMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in allSkinnedMeshRenderers)
        {
            if (renderer.gameObject.tag != "ignoreRenderer")
            {
                tempIncludedRenderers.Add(renderer);
            }
        }
        foreach (var renderer in allMeshRenderers)
        {
            if (renderer.gameObject.tag != "ignoreRenderer")
            {
                tempIncludedRenderers.Add(renderer);
            }
        }
        includedRenderers = tempIncludedRenderers.ToArray();
        originalMaterials = new Material[includedRenderers.Length];
        for (int i = 0; i < includedRenderers.Length; i++)
        {
            originalMaterials[i] = includedRenderers[i].material;
        }
    }
    #endregion

    #region HealthModule
    private void CheckHealthForUI()
    {
        if(isDead)
        {
            healthModuleCanvas.SetActive(false);
            return;
        }

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

    public void TakeDamage(int amount)
        {
            if(floatingTextPrefab)
            {
                Vector3 spawnPosition = transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
            }

            currentHealth -= amount;
            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }

            if(!isDead)
            {
                currentHealth -= amount;
                healthBar.fillAmount = (float)currentHealth / maxHealth;

                PlayHitFlash();
                knockBack();

                if (currentHealth <= 0)
                {
                    Die();
                }
            }
        }

    public void Die()
    {
        isDead = true;
        pufEffect.Play();
        PlayDeadFlash();
        visual.SetActive(false);
        lootBag.InstantiateLoot(transform.position, dropCount);

        DelayHelper.DelayAction(enemyDestroyTimeOnDead, () =>
        {
            Destroy(gameObject);
        });
    }
    #endregion

    #region Effects Module
    public void PlayHitFlash()
    {
        foreach (var renderer in includedRenderers)
        {
            renderer.material = flashMaterial;
        }

        DelayHelper.DelayAction(materialChangeDuration, () =>
        {
            if(isDead) return;
            for (int i = 0; i < includedRenderers.Length; i++)
            {
                includedRenderers[i].material = originalMaterials[i];
            }
        });
    }

    public void PlayDeadFlash()
    {
        foreach (var renderer in includedRenderers)
        {
            renderer.material = deadMaterial;
        }
    }
    public void knockBack()
    {
        transform.DOJump(transform.position, 1f, 1, 0.35f);
    }
    #endregion
}
