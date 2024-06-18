using System;
using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;
using UnityEngine.UI;
using FIMSpace;
using FIMSpace.FLook;

public class CharacterControlManager : MonoBehaviour
{
    public Rigidbody rb;
    [HideInInspector] public T11Joystick joystick;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public Canvas inputCanvas;

    [Header("Movement Module")]
    public bool isJoytick;
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Character Rotation Module")]
    public FLookAnimator lookAnimator;

    [Header("Dash Module")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldownTime;
    bool dashCooldownComplete = true;
    bool isDashing;

    [Header("Combat Module")]
    public bool isRangedAttack;
    public int rangedAttackRange;
    public int meleeAttackRange;
    public int detectorRange;
    public int damage;
    public float attackSpeed;
    public float timeBetweenAttacks;
    public Projectile projectile;
    public Transform projectileSpawnPoint;
    public Collider detectorCollider;
    public Collider attackCollider;
    private List<Enemy> detectedEnemies = new List<Enemy>();
    private Enemy closestEnemy = null;
    private bool alreadyAttacked;
    private int attackCounter = 0;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;
    [HideInInspector] public int currentHealth;

    [Header("Collector Module")]
    public GameObject currencyCollector;

    [Header("Floating Text Module")]
    public floatingText floatingTextPrefab;

    [Header("Effects Module")]
    public ParticleSystem onPowerUpEffect;
    public TrailRenderer speedUpTrail;
    public ParticleSystem dashEffect;
    public ParticleSystem HealEffect;
    public TrailEffect[] dashTrailEffects;
    public ParticleSystem deathEffect;

    [Header("FlashModule")]
    public Renderer[] includedRenderers;
    private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
    private MeshRenderer[] allMeshRenderers;
    public Material flashMaterial;
    public Material deadMaterial;
    public Material[] originalMaterials;
    public float materialChangeDuration = .2f;

    [Header("Aim Lock Module")]
    public GameObject aimLock;
    
    [Header("Debug")]
    public bool drawGizmos;

    private bool isDead;
    private Vector3 _moveVector;
    
    #region PreperationAndLoop
    private void Awake(){
        joystick = GetComponentInChildren<T11Joystick>();
        controller = GetComponentInChildren<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        inputCanvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        if(isJoytick)
            EnableJoystick();

        loadHealth();
        setCombatColliders();
        GetAllRenderers();
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

    #region Combat Module
    private void setCombatColliders()
    {
        ((SphereCollider)detectorCollider).radius = detectorRange;
        if(isRangedAttack)
            ((SphereCollider)attackCollider).radius = rangedAttackRange;
        else
            ((SphereCollider)attackCollider).radius = meleeAttackRange;
    }

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

        if(closestEnemy != null)
        {
            aimLock.SetActive(true);
            aimLock.transform.position = closestEnemy.transform.position;
        }else
        {
            aimLock.SetActive(false);
        }

        if(isRangedAttack)
        {
            if(closestEnemy != null && !closestEnemy.isDead && Vector3.Distance(rb.position, closestEnemy.transform.position) <= detectorCollider.bounds.extents.x)
            return;
        }

        float closestDistance = float.MaxValue;
        closestEnemy = null;

        foreach (Enemy enemy in detectedEnemies)
        {
            if (enemy == null)
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

        

        if (detectedEnemies == null)
        {
            closestEnemy = null;
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
                    float yOffset = projectileSpawnPoint.position.y;
                    Projectile _projectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                    _projectile.Fire(damage, closestEnemy.transform.position, yOffset);
                }

                if(!isRangedAttack)
                {
                    attackCounter++;
                    switch (attackCounter)
                    {
                        case 1:
                            playerAnimator.SetTrigger("Attack1");
                            break;
                        case 2:
                            playerAnimator.SetTrigger("Attack2");
                            break;
                        default:   
                            attackCounter = 0;
                            break;
                    }
                }

                DelayHelper.DelayAction(timeBetweenAttacks, () =>
                {
                    alreadyAttacked = false;
                });
            }
        }else
        {
            lookAnimator.ObjectToFollow = null;
        }
    }

    // This method will be called by the animation event
    public void DealMeleeDamage()
    {
        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(damage);
        }
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

    public void TakeDamage(int amount)
    {
        if(!isDead)
        {
            playerAnimator.SetTrigger("isHit");
            
            currentHealth -= amount;
            GameManager.Instance.saveModule.saveInfo.characterCurrentHealth = currentHealth;
            healthBar.fillAmount = (float)currentHealth / maxHealth;

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
                _floatingText.SetText("+" + amount.ToString() + "hp", Color.green, 3f);
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