using System;
using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;
using UnityEngine.UI;
using FIMSpace;
using FIMSpace.FLook;
using Shapes;
using DG.Tweening;

public class CharacterControlManager : MonoBehaviour
{    
    [Header("Use Character Stats Module to values in this Class!")]
    public Rigidbody rb;
    public T11Joystick joystick;
    public CharacterStatsModule statsModule;
    public CharacterController controller;
    public Animator playerAnimator;
    public Canvas inputCanvas;
    
    [Foldout("Movement Module", foldEverything = true, styled = true, readOnly = false)]
    [DisplayWithoutEdit()] public bool isJoytick;
    [DisplayWithoutEdit()] public float movementSpeed;
    [DisplayWithoutEdit()] public float rotationSpeed;

    [Foldout("Character Rotation Module", foldEverything = true, styled = true, readOnly = false)]
    public FLookAnimator lookAnimator;

    [Foldout("Dash Module", foldEverything = true, styled = true, readOnly = false)]
    [DisplayWithoutEdit()] public float dashSpeed;
    [DisplayWithoutEdit()] public float dashDuration;
    [DisplayWithoutEdit()] public float dashCooldownTime;
    bool dashCooldownComplete = true;
    bool isDashing;

    [Foldout("Combat Module", foldEverything = true, styled = true, readOnly = false)]
    [DisplayWithoutEdit()] public bool isRangedAttack;
    [DisplayWithoutEdit()] public bool isMeleeAttack;
    [DisplayWithoutEdit()] public bool isRotatingBlades;
    [DisplayWithoutEdit()] public float rangedAttackRange;
    [DisplayWithoutEdit()] public float meleeAttackRange;
    [DisplayWithoutEdit()] public float detectorRange;
    [DisplayWithoutEdit()] public float damage;
    [DisplayWithoutEdit()] public float criticalChance;
    [DisplayWithoutEdit()] public float criticalMultiplier;
    [DisplayWithoutEdit()] public float splashDamageMultiplier;
    [DisplayWithoutEdit()] public float attackSpeedRange;
    [DisplayWithoutEdit()] public float attackSpeedMelee;
    public Projectile projectile;
    public Transform projectileSpawnPoint;
    public Collider detectorCollider;
    public Collider attackCollider;
    private List<Enemy> detectedEnemies = new List<Enemy>();
    private Enemy closestEnemy = null;
    private bool alreadyAttacked;
    private int attackCounter = 0;

    [Foldout("Health Module", foldEverything = true, styled = true, readOnly = false)]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    [DisplayWithoutEdit()] public float maxHealth;
    [DisplayWithoutEdit()] public float healthEaseSpeed;
    [DisplayWithoutEdit()] public float currentHealth;

    [Foldout("Level / Experience Module", foldEverything = true, styled = true, readOnly = false)]
    [DisplayWithoutEdit()] public int currentLevel;
    [DisplayWithoutEdit()] public int currentExperience;
    [DisplayWithoutEdit()] public int maxExperience;
    [DisplayWithoutEdit()] public int levelExperienceMultiplier;
    public static event Action<int> OnEXPChange;
    public static event Action<int> OnLevelChange;

    [Foldout("Collector Module", foldEverything = true, styled = true, readOnly = false)]
    [Header("Collector Module")]
    public GameObject currencyCollector;
    public Transform collectTarget;

    [Foldout("Attack Range Visualizer Module", foldEverything = true, styled = true, readOnly = false)]
    public GameObject attackRangeVisualizer;
    public Disc attackRangeVisualizerDisc;

    [Foldout("Floating Text Module", foldEverything = true, styled = true, readOnly = false)]
    public floatingText floatingTextPrefab;

    [Foldout("Effects Module", foldEverything = true, styled = true, readOnly = false)]
    public ParticleSystem levelUpEffect;
    public ParticleSystem onPowerUpEffect;
    public TrailRenderer speedUpTrail;
    public ParticleSystem dashEffect;
    public ParticleSystem HealEffect;
    public TrailEffect[] dashTrailEffects;
    public ParticleSystem deathEffect;
    public ParticleSystem swordSlashEffect;

    [Foldout("Flash Module", foldEverything = true, styled = true, readOnly = false)]
    public Renderer[] includedRenderers;
    private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
    private MeshRenderer[] allMeshRenderers;
    public Material flashMaterial;
    public Material deadMaterial;
    public Material[] originalMaterials;
    public float materialChangeDuration = .2f;

    [Foldout("Aim Lock Module", foldEverything = true, styled = true, readOnly = false)]
    public GameObject aimLock;

    [Foldout("Virtual Wallet Module", foldEverything = true, styled = true, readOnly = false)]
    public VirtualWalletModule virtualWallet;

    [Foldout("Weapon References", foldEverything = true, styled = true, readOnly = false)]
    public List<GameObject> MeleeWeapons;
    public List<GameObject> RangedWeapons;

    [Foldout("Debug", foldEverything = true, styled = true, readOnly = false)]
    public bool drawGizmos;

    private bool isDead;
    private Vector3 _moveVector;
    
    #region PreperationAndLoop
    private void Awake(){
        joystick = GetComponentInChildren<T11Joystick>();
        controller = GetComponentInChildren<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        inputCanvas = GetComponentInChildren<Canvas>();
        statsModule = GetComponentInChildren<CharacterStatsModule>();
    }

    private void Start()
    {
        if(isJoytick)
            EnableJoystick();

        loadHealth();
        loadLevelData();
        GetAllRenderers();
        InitializeWeaponPools();
        GameManager.Instance.experienceModule.OnExperienceChange += HandleExperienceChange;
    }

    void Update()
    {
        CheckHealthForUI();  
        UpdateClosestEnemy();
        AttackClosestEnemy();     
    }

    private void FixedUpdate()
    {
        if(!isDead)
            MoveCharacter();
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

    private void setAttackRangeVisualizer()
    {
        ((SphereCollider)detectorCollider).radius = detectorRange;
        if(isRangedAttack)
        {
            ((SphereCollider)attackCollider).radius = rangedAttackRange;
            attackRangeVisualizerDisc.Radius = rangedAttackRange;
        }
        else if(isMeleeAttack)
        {
            ((SphereCollider)attackCollider).radius = meleeAttackRange;
            attackRangeVisualizerDisc.Radius = meleeAttackRange;
        }            
    }

    public void updateAttackRangeVisualizer()
    {
        ((SphereCollider)detectorCollider).radius = detectorRange;
        if(isRangedAttack)
        {
            ((SphereCollider)attackCollider).radius = rangedAttackRange;
            attackRangeVisualizerDisc.Radius = rangedAttackRange;
        }
        else if(isMeleeAttack)
        {
            ((SphereCollider)attackCollider).radius = meleeAttackRange;
            attackRangeVisualizerDisc.Radius = meleeAttackRange;
        }    
    }

    private void InitializeWeaponPools()
    {
        foreach (var weapon in MeleeWeapons)
        {
            weapon.SetActive(false);
        }

        foreach (var weapon in RangedWeapons)
        {
            weapon.SetActive(false);
        }
    }

    public void PrepareWeapon()
    {
        setAttackRangeVisualizer();
        if (isMeleeAttack)
        {
            ActivateWeapon(MeleeWeapons, 0);
        }
        else if (isRangedAttack)
        {
            ActivateWeapon(RangedWeapons, 0);
        }
    }

    private void ActivateWeapon(List<GameObject> weaponPool, int index)
    {
        if (weaponPool.Count > index)
        {
            weaponPool[index].SetActive(true);
        }
    }
    #endregion
    
    #region MenuHandler
    public void OpenIncrementalMenu()
    {
        GameManager.Instance.openIncrementalMenu();
    }
    #endregion

    #region Movement Module
    public void EnableJoystick()
    {
        isJoytick = true;
        inputCanvas.gameObject.SetActive(true);
    }

    private void MoveCharacter()
    {
        _moveVector = Vector3.zero;
        _moveVector.x = joystick.Horizontal * movementSpeed * Time.deltaTime;
        _moveVector.z = joystick.Vertical * movementSpeed * Time.deltaTime;

        if(joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            float inputMagnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

            Vector3 direction = Vector3.RotateTowards(rb.transform.forward, _moveVector, rotationSpeed * Time.deltaTime, 0.0f);
            rb.transform.rotation = Quaternion.LookRotation(direction);

            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetFloat("movementSpeed", inputMagnitude);
        }

        else if(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetFloat("movementSpeed", 0);
        }

        rb.MovePosition(rb.position + _moveVector);
    }
    #endregion

    #region Dash Module
    public void Dash()
    {
        if(!isDead && !isDashing && dashCooldownComplete)
        {
            playerAnimator.SetBool("isDashing", true);
            dashCooldownComplete = false;
            isDashing = true;
            dashEffect.Play();
            Vector3 dashDirection = rb.transform.forward;
            rb.velocity = dashDirection * dashSpeed;

            for (int i = 0; i < dashTrailEffects.Length; i++)
            {
                dashTrailEffects[i].active = true;
            }

            DelayHelper.DelayAction(dashDuration, () => 
            { 
                playerAnimator.SetBool("isDashing", false);
                dashEffect.Stop();
                rb.velocity = Vector3.zero; 
                isDashing = false; 
                for (int i = 0; i < dashTrailEffects.Length; i++)
                {
                    dashTrailEffects[i].active = false;
                }
            });

            DelayHelper.DelayAction(dashCooldownTime + dashDuration, () => 
            { 
                dashCooldownComplete = true; 
            });
        }
    }
    #endregion

    #region Level / Experience Module
    public void loadLevelData()
    {
        if(GameManager.Instance.saveModule.saveInfo.characterCurrentLevel == 0 && GameManager.Instance.saveModule.saveInfo.characterCurrentExperience == 0)
        {
            GameManager.Instance.saveModule.saveInfo.characterCurrentLevel = 0;
            GameManager.Instance.saveModule.saveInfo.characterCurrentExperience = 0;
            GameManager.Instance.saveModule.saveInfo.characterExperienceToNextLevel = maxExperience;
            GameManager.Instance.experienceModule.UpdateExperienceUI(currentExperience, currentLevel, maxExperience);
        }
        currentLevel = GameManager.Instance.saveModule.saveInfo.characterCurrentLevel;
        currentExperience = GameManager.Instance.saveModule.saveInfo.characterCurrentExperience;
        maxExperience = GameManager.Instance.saveModule.saveInfo.characterExperienceToNextLevel;
        GameManager.Instance.experienceModule.UpdateExperienceUI(currentExperience, currentLevel, maxExperience);
    }

    public void HandleExperienceChange(int experience)
    {
        OnEXPChange?.Invoke(experience);
        currentExperience += experience;
        GameManager.Instance.saveModule.saveInfo.characterCurrentExperience = currentExperience;
        GameManager.Instance.experienceModule.UpdateExperienceUI(currentExperience, currentLevel, maxExperience);

        if(currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        OnLevelChange?.Invoke(currentLevel);
        GameManager.Instance.saveModule.saveInfo.characterCurrentLevel = currentLevel;
        currentExperience = 0;
        GameManager.Instance.saveModule.saveInfo.characterCurrentExperience = currentExperience;
        maxExperience = maxExperience * levelExperienceMultiplier;
        GameManager.Instance.saveModule.saveInfo.characterExperienceToNextLevel = maxExperience;
        GameManager.Instance.experienceModule.UpdateExperienceUI(currentExperience, currentLevel, maxExperience);

        levelUpEffect.Play();
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = rb.transform.position;
            spawnPosition.y += 2f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText("LEVEL UP!", Color.white, 6f);
        }
}
    #endregion

    #region Combat Module
    public void HandleTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (!detectedEnemies.Contains(enemy))
            {
                detectedEnemies.Add(enemy);
            }
        }
    }

    public void HandleTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (detectedEnemies.Contains(enemy))
            {
                detectedEnemies.Remove(enemy);
            }
        }
    }

    void UpdateClosestEnemy()
        {
        if (rb == null || detectedEnemies == null)
        {
            return;
        }

        if(closestEnemy != null && !closestEnemy.isDead)
        {
            aimLock.SetActive(true);
            aimLock.transform.position = closestEnemy.transform.position;
            attackRangeVisualizer.SetActive(true);
        }else
        {
            aimLock.SetActive(false);
            attackRangeVisualizer.SetActive(false);
            closestEnemy = null;
            FindClosestEnemy();
        }

        if (closestEnemy != null && isRangedAttack && Vector3.Distance(rb.position, closestEnemy.transform.position) <= detectorCollider.bounds.extents.x)
        {
            return;
        }

        if (isRangedAttack || isMeleeAttack)
        {
            FindClosestEnemy();
        }
    }

    void FindClosestEnemy()
    {
        float closestDistance = float.MaxValue;
        closestEnemy = null;

        foreach (Enemy enemy in detectedEnemies)
        {
            if (enemy == null || enemy.isDead)
            {
                continue;
            }

            float distance = Vector3.Distance(rb.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
    }

    void AttackClosestEnemy()
    {
        if (closestEnemy != null && Vector3.Distance(rb.position, closestEnemy.transform.position) <= attackCollider.bounds.extents.x)
        {
            lookAnimator.ObjectToFollow = closestEnemy.lookTarget;
            if (!alreadyAttacked) //&& joystick.Horizontal == 0 && joystick.Vertical == 0)
            {
                alreadyAttacked = true;
                
                if(isRangedAttack)
                {
                    float timeToWait = 1;
                    
                    playerAnimator.SetFloat("attackSpeed", attackSpeedRange);
                    timeToWait = GetAnimationSpeed("rangedattack1");
                    playerAnimator.SetTrigger("rangedattack1");

                    DelayHelper.DelayAction(timeToWait, () =>
                    {
                        alreadyAttacked = false;
                    });
                }

                if(isMeleeAttack)
                {
                    float timeToWait = 1;
                    if(attackCounter == 0)
                    {
                        playerAnimator.SetFloat("attackSpeed", attackSpeedMelee);
                        timeToWait = GetAnimationSpeed("meleeattack1");
                        playerAnimator.SetTrigger("meleeattack1");
                        attackCounter++;
                    }else
                    {
                        playerAnimator.SetFloat("attackSpeed", attackSpeedMelee);
                        timeToWait = GetAnimationSpeed("meleeattack2");
                        playerAnimator.SetTrigger("meleeattack2");
                        attackCounter = 0;
                    }
                    
                    DelayHelper.DelayAction(timeToWait, () =>
                    {
                        alreadyAttacked = false;
                    });       
                }
            }
        }else
        {
            lookAnimator.ObjectToFollow = null;
        }
    }

    public void FireProjectile()
    {
        bool isCriticalHit = UnityEngine.Random.value < criticalChance;
        float finalDamage;
        if (isCriticalHit)
        {
            finalDamage =criticalMultiplier * damage;
            closestEnemy.TakeDamage(finalDamage, true);
        }else
        {
            finalDamage = damage;
            closestEnemy.TakeDamage(finalDamage, false);
        }

        float yOffset = projectileSpawnPoint.position.y;
        Projectile _projectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
        _projectile.Fire(finalDamage, closestEnemy.transform.position, yOffset);
    }

    public void DealMeleeDamage()
    {
        if (closestEnemy != null)
        {
            swordSlashEffect.Play();
            bool isCriticalHit = UnityEngine.Random.value < criticalChance;
            float finalDamage;
            if (isCriticalHit)
            {
                finalDamage =criticalMultiplier * damage;
                closestEnemy.TakeDamage(finalDamage, true);
            }else
            {
                finalDamage = damage;
                closestEnemy.TakeDamage(finalDamage, false);
            }
            

            foreach (Enemy enemy in detectedEnemies)
            {
                if (enemy == null)
                    continue;

                float distance = Vector3.Distance(rb.position, enemy.transform.position);
                if (distance <= attackCollider.bounds.extents.x)
                {
                    if(enemy == closestEnemy)
                        continue;

                    if(isCriticalHit)
                        enemy.TakeDamage((damage * splashDamageMultiplier) * criticalMultiplier, true);
                    else
                        enemy.TakeDamage(damage * splashDamageMultiplier, false);
                }
            }
        }
    }

    float GetAnimationSpeed(string clipName)
    {
        RuntimeAnimatorController controller = playerAnimator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length / attackSpeedMelee;
            }
        }

        Debug.LogWarning($"Animation clip '{clipName}' not found.");
        return -1f;
    }
    #endregion

    #region Health Module
    private void loadHealth()
    {
        currentHealth = GameManager.Instance.saveModule.saveInfo.characterCurrentHealth;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthBarEase.fillAmount = (float)currentHealth / maxHealth;

        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            GameManager.Instance.saveModule.saveInfo.characterCurrentHealth = currentHealth;
            healthBar.fillAmount = maxHealth;
            healthBarEase.fillAmount = maxHealth;
        }
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

    public void TakeDamage(float amount)
    {
        if(!isDead)
        {
            playerAnimator.SetTrigger("isHit");
            
            currentHealth -= amount;
            GameManager.Instance.saveModule.saveInfo.characterCurrentHealth = currentHealth;
            GameManager.Instance.cameraShakeModule.ShakeCamera(2f, 1f, 0.25f);
            healthBar.fillAmount = currentHealth / maxHealth;

            PlayHitFlash();

            if(floatingTextPrefab)
            {
                Vector3 spawnPosition = rb.transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 3f);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int amount)
    {
        if(!isDead)
        {
            currentHealth += amount;
            GameManager.Instance.saveModule.saveInfo.characterCurrentHealth = currentHealth;
            healthBar.fillAmount = (float)currentHealth / maxHealth;
            
            if(floatingTextPrefab)
            {
                Vector3 spawnPosition = rb.transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("+" + amount.ToString() + "hp", Color.green, 6f);
            }

            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

    void Die()
    {
        PlayDeadFlash();
        currencyCollector.SetActive(false);
        playerAnimator.SetBool("isDead", true);
        deathEffect.Play();
        isDead = true;
    } 
    #endregion

    #region Effect Actions
        public void PlayPowerUpEffect()
        {
            onPowerUpEffect.Play();
        }
        public void PlayHealEffect()
        {
            HealEffect.Play();
        }
        public void PlayHitFlash()
        {
            foreach (var renderer in includedRenderers)
            {
                renderer.material = flashMaterial;
            }

            DelayHelper.DelayAction(materialChangeDuration, () =>
            {
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
            transform.DOJump(transform.position, 2f, 1, 0.35f);
        }
    #endregion

    #region PowerUp Actions

        public void AddHealth(int amount)
        {
            Heal(amount);
        }

        public void SpeedUp(float speedAmount, float duration)
        {
            movementSpeed = movementSpeed * speedAmount;
            speedUpTrail.emitting = true;
            DelayHelper.DelayAction(duration, () => 
            {
                movementSpeed /= speedAmount; 
                speedUpTrail.emitting = false;
            });
            //StartCoroutine(SpeedUpCoroutine(duration));
        }
    #endregion

    #region Debug
    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position, attackCollider.bounds.extents.x);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rb.position, detectorCollider.bounds.extents.x);
        }
    }
    #endregion
}