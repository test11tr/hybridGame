using System;
using System.Collections.Generic;
using EpicToonFX;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class Enemy : MonoBehaviour
{
    private CivilianAI civilianAI;
    private MeleeAttackAI meleeAttackAI;
    private RangedAttackAI rangedAttackAI;
    private BossAI bossAI;
    private NoBehaviorAI noBehaviorAI;

    [Header("Enemy Settings")]
    public bool isCivilian;
    public bool isMeleeAttack;
    public bool isRangedAttack;
    public bool isBoss;
    public bool noBehaviour;

    [Header("Enemy Movement")]
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    public float movementSpeed;
    public float rotationSpeed;
    public float acceleration;
    public float runSpeed;
    public float runAcceleration;

    [Header("Enemy Attack")]
    public float damage;
    public float timeBetweenAttacks;
    public Projectile projectile;
    public Transform projectileSpawnPoint;
    bool alreadyAttacked;
    public int enemyDestroyTimeOnDead;

    [Header("Enemy Sight Module")]
    public float sightRange;
    public float attackRange;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public float maxHealth;
    public float healthEaseSpeed;
    public float healthRegenRate; 
    [HideInInspector] public float lastRegenTime;
    private float currentHealth;

    [Header("NoBehaviourAI Module")]
    public int requiredHitsToDie;

    [Header("Experience Module")]
    public int experienceValue;

    [Header("Loot Module")]
    public LootBag lootBag;
    public int dropCount;

    [Header("Effects Module")]
    public TrailRenderer trail;
    public ParticleSystem deathEffect;
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
    public Rigidbody rb;
    public Animator enemyAnimator;
    public floatingText floatingTextPrefab;
    public Transform lookTarget;
    private Transform player;
    public GameObject visual;

    [Header("Debug")]
    public bool drawGizmos;

    [HideInInspector] public Vector3 spawnAreaCenter;
    [HideInInspector] public float spawnAreaWidth;
    [HideInInspector] public float spawnAreaHeight;
    private bool isReturningToPatrolArea = false;
    [HideInInspector] public bool isDead;

    public Action OnDeath { get; internal set; }

    #region SettersAndGetters
    void Start()
    {
        player = GameManager.Instance.player.rb.transform;
        if(!noBehaviour)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = movementSpeed;
            agent.angularSpeed = rotationSpeed;
            agent.acceleration = acceleration;
        }
        currentHealth = maxHealth;
        GetAllRenderers();

        if(isCivilian)
        {
            civilianAI = new CivilianAI(this);
        }
        else if(isMeleeAttack)
        {
            meleeAttackAI = new MeleeAttackAI(this);
        }
        else if(isRangedAttack)
        {
            rangedAttackAI = new RangedAttackAI(this);
        }
        else if(isBoss)
        {
            bossAI = new BossAI(this);
        }
        else if(noBehaviour)
        {
            noBehaviorAI = new NoBehaviorAI(this);
        }
    }

    public void Update()
    {
        if(isCivilian)
        {
            civilianAI.Update();
        }else if(isMeleeAttack)
        {
            meleeAttackAI.Update();
        }else if(isRangedAttack)
        {
            rangedAttackAI.Update();
        }else if(isBoss)
        {
            bossAI.Update();
        }else if(noBehaviour)
        {
            noBehaviorAI.Update();
        }
    }

    public void TakeDamage(float amount)
    {
        if(isCivilian)
        {
            civilianAI.TakeDamage(amount);
        }else if(isMeleeAttack)
        {
            meleeAttackAI.TakeDamage(amount);
        }else if(isRangedAttack)
        {
            rangedAttackAI.TakeDamage(amount);
        }else if(isBoss)
        {
            bossAI.TakeDamage(amount);
        }else if(noBehaviour)
        {
            noBehaviorAI.TakeDamage(amount);
        }
    }

    public void getSpawnerInfo(Vector3 center, float width, float height)
    {
        spawnAreaCenter = center;
        spawnAreaWidth = width;
        spawnAreaHeight = height;
    }
    #endregion

    #region CivilianAI
    private class CivilianAI
    {
        #region AIBrain
        private Enemy parent;

        public CivilianAI(Enemy parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            CheckHealthForUI();
            if(parent.isDead || parent.agent.enabled == false) 
            return;
            AIStateBrain();
            RegenerateHealth();   
        }

        public void AIStateBrain()
        {
            parent.playerInSightRange = Physics.CheckSphere(parent.transform.position, parent.sightRange, parent.whatIsPlayer);
            parent.playerInAttackRange = Physics.CheckSphere(parent.transform.position, parent.attackRange, parent.whatIsPlayer);

            if (!parent.playerInSightRange && !parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                HandleTrailConditionally(false);

                if (!IsWithinSpawnArea())
                {
                    parent.isReturningToPatrolArea = true;
                    SetDestinationToSpawnAreaCenter();
                }
                else
                {
                    Patrolling();
                }
            }
            else if (parent.playerInSightRange && !parent.playerInAttackRange)
            {
                EscapeFromPlayer();
                parent.agent.speed = parent.runSpeed;
                parent.agent.acceleration = parent.runAcceleration;
                HandleTrailConditionally(true);
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                HandleTrailConditionally(false);
                if (IsWithinSpawnArea() && !parent.agent.pathPending)
                {
                    parent.isReturningToPatrolArea = false;
                    Patrolling();
                }
            }
        }

        public void getSpawnerInfo(Vector3 center, float width, float height)
        {
            parent.spawnAreaCenter = center;
            parent.spawnAreaWidth = width;
            parent.spawnAreaHeight = height;
        }

        bool IsWithinSpawnArea()
        {
            return Vector3.Distance(parent.transform.position, parent.spawnAreaCenter) <= Mathf.Max(parent.spawnAreaWidth, parent.spawnAreaHeight) / 2;
        }

        void SetDestinationToSpawnAreaCenter()
        {
            parent.agent.SetDestination(parent.spawnAreaCenter);
        }

        private void Patrolling()
        {
            if (!parent.walkPointSet)
            {
                SearchWalkPoint();
            }
            else
            {
                parent.agent.SetDestination(parent.walkPoint);
            }

            Vector3 distanceToWalkPoint = parent.transform.position - parent.walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                parent.walkPointSet = false;
            }
        }

        private void EscapeFromPlayer()
        {
            if (!parent.walkPointSet)
            {
                SearhEscapeWalkPoint();
            }
            else
            {
                parent.agent.SetDestination(parent.walkPoint);
            }

            Vector3 distanceToWalkPoint = parent.transform.position - parent.walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                parent.walkPointSet = false;
            }
        }

        private void SearhEscapeWalkPoint()
        {
            float randomZ = UnityEngine.Random.Range(-parent.walkPointRange, parent.walkPointRange);
            float randomX = UnityEngine.Random.Range(-parent.walkPointRange, parent.walkPointRange);

            parent.walkPoint = new Vector3(parent.transform.position.x + randomX, parent.transform.position.y, parent.transform.position.z + randomZ);
            parent.agent.SetDestination(parent.walkPoint);

            if(Physics.Raycast(parent.walkPoint, -parent.transform.up, 2f, parent.whatIsGround))
            {
                parent.walkPointSet = true;
            }
        }

        private void SearchWalkPoint()
        {
            float randomZ = UnityEngine.Random.Range(-parent.spawnAreaHeight / 2, parent.spawnAreaHeight / 2);
            float randomX = UnityEngine.Random.Range(-parent.spawnAreaWidth / 2, parent.spawnAreaWidth / 2);

            Vector3 randomPoint = parent.spawnAreaCenter + new Vector3(randomX, 0, randomZ);
            parent.walkPoint = new Vector3(randomPoint.x, parent.transform.position.y, randomPoint.z);

            if (Physics.Raycast(parent.walkPoint, -parent.transform.up, 2f,parent. whatIsGround))
            {
                parent.walkPointSet = true;
            }
        }
        #endregion

        #region HealthModule
        public void CheckHealthForUI()
        {
            if(parent.isDead)
            {
                parent.healthModuleCanvas.SetActive(false);
                return;
            }

            if(parent.currentHealth < parent.maxHealth)
            {
                parent.healthModuleCanvas.SetActive(true);
            }
            else
            {
                parent.healthModuleCanvas.SetActive(false);
            }

            

            if(parent.healthBar.fillAmount != parent.healthBarEase.fillAmount)
            {
                parent.healthBarEase.fillAmount = Mathf.Lerp(parent.healthBarEase.fillAmount, parent.healthBar.fillAmount, parent.healthEaseSpeed * Time.deltaTime);
            }
        }

        private void RegenerateHealth()
        {
            if (parent.isReturningToPatrolArea || !parent.playerInSightRange)
            {
                if (parent.currentHealth >= parent.maxHealth || parent.isDead) return;

                if (Time.time - parent.lastRegenTime >= 1f / parent.healthRegenRate)
                {
                    parent.currentHealth += 1;
                    parent.currentHealth = Mathf.Min(parent.currentHealth, parent.maxHealth); 
                    parent.lastRegenTime = Time.time; 

                    parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;
                }
            }else
            {
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            if (parent.currentHealth <= 0 && !parent.isDead)
            {
                Die();
            }

            if(!parent.isDead)
            {
                parent.currentHealth -= amount;

                if(parent.floatingTextPrefab)
                {
                    Vector3 spawnPosition = parent.transform.position;
                    spawnPosition.y += 1.5f;
                    floatingText _floatingText = Instantiate(parent.floatingTextPrefab, spawnPosition, Quaternion.identity);
                    _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
                }

                parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;

                PlayHitFlash();
                knockBack();

                if (parent.currentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            parent.isDead = true;
            parent.deathEffect.Play();
            parent.pufEffect.Play();
            PlayDeadFlash();
            parent.trail.emitting = false;
            parent.visual.SetActive(false);
            parent.lootBag.InstantiateLoot(parent.transform.position, parent.dropCount);
            
            GameManager.Instance.experienceModule.AddExperience(parent.experienceValue);
            parent.OnDeath?.Invoke();
            DelayHelper.DelayAction(parent.enemyDestroyTimeOnDead, () =>
            {
                Destroy(parent.gameObject);
            });
        }
        #endregion

        #region Effects Module
        void HandleTrailConditionally(bool shouldEmit)
        {
            if(parent.trail.emitting != shouldEmit)
            {
                handleTrail();
            }
        }
        public void handleTrail()
        {
            parent.trail.emitting = !parent.trail.emitting;
        }
        public void PlayHitFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.flashMaterial;
            }

            DelayHelper.DelayAction(parent.materialChangeDuration, () =>
            {
                if(parent.isDead) return;
                for (int i = 0; i < parent.includedRenderers.Length; i++)
                {
                    parent.includedRenderers[i].material = parent.originalMaterials[i];
                }
            });
        }

        public void PlayDeadFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.deadMaterial;
            }
        }
        public void knockBack()
        {
            parent.transform.DOJump(parent.transform.position, 1f, 1, 0.35f);
        }
        #endregion
    }
    #endregion

    #region MeleeAttackAI
    private class MeleeAttackAI
    {
        #region AIBrain
        private Enemy parent;

        public MeleeAttackAI(Enemy parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            CheckHealthForUI();
            if(parent.isDead || parent.agent.enabled == false) 
            return;
            AIStateBrain();
            RegenerateHealth();   
        }

        public void AIStateBrain()
        {
            parent.playerInSightRange = Physics.CheckSphere(parent.transform.position, parent.sightRange, parent.whatIsPlayer);
            parent.playerInAttackRange = Physics.CheckSphere(parent.transform.position, parent.attackRange, parent.whatIsPlayer);

            if (!parent.playerInSightRange && !parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (!IsWithinSpawnArea())
                {
                    parent.isReturningToPatrolArea = true;
                    SetDestinationToSpawnAreaCenter();
                }
                else
                {
                    Patrolling();
                }
            }
            else if (parent.playerInSightRange && !parent.playerInAttackRange)
            {
                ChasePlayer();
                parent.agent.speed = parent.runSpeed;
                parent.agent.acceleration = parent.runAcceleration;
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.playerInAttackRange && parent.playerInSightRange)
            {
                AttackPlayer();
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (IsWithinSpawnArea() && !parent.agent.pathPending)
                {
                    parent.isReturningToPatrolArea = false;
                    Patrolling();
                }
            }
        }

        public void getSpawnerInfo(Vector3 center, float width, float height)
        {
            parent.spawnAreaCenter = center;
            parent.spawnAreaWidth = width;
            parent.spawnAreaHeight = height;
        }

        bool IsWithinSpawnArea()
        {
            return Vector3.Distance(parent.transform.position, parent.spawnAreaCenter) <= Mathf.Max(parent.spawnAreaWidth, parent.spawnAreaHeight) / 2;
        }

        void SetDestinationToSpawnAreaCenter()
        {
            parent.agent.SetDestination(parent.spawnAreaCenter);
        }

        private void Patrolling()
        {
            if (!parent.walkPointSet)
            {
                SearchWalkPoint();
            }
            else
            {
                parent.agent.SetDestination(parent.walkPoint);
            }

            Vector3 distanceToWalkPoint = parent.transform.position - parent.walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                parent.walkPointSet = false;
            }
        }

        private void ChasePlayer()
        {
            parent.agent.SetDestination(parent.player.position);
        }

        private void AttackPlayer()
        {
            parent.agent.SetDestination(parent.transform.position);

            parent.transform.LookAt(parent.player);

            if (!parent.alreadyAttacked)
            {
                parent.alreadyAttacked = true;
                // Attack code here
                if(parent.isRangedAttack)
                {
                    float yOffset = parent.projectileSpawnPoint.position.y;
                    Projectile _projectile = Instantiate(parent.projectile, parent.projectileSpawnPoint.position, Quaternion.identity);
                    _projectile.Fire(parent.damage, parent.player.position, yOffset);
                }
                
                DelayHelper.DelayAction(parent.timeBetweenAttacks, () =>
                {
                    parent.alreadyAttacked = false;
                });
            }
        }

        private void SearchWalkPoint()
        {
            float randomZ = UnityEngine.Random.Range(-parent.spawnAreaHeight / 2, parent.spawnAreaHeight / 2);
            float randomX = UnityEngine.Random.Range(-parent.spawnAreaWidth / 2, parent.spawnAreaWidth / 2);

            Vector3 randomPoint = parent.spawnAreaCenter + new Vector3(randomX, 0, randomZ);
            parent.walkPoint = new Vector3(randomPoint.x, parent.transform.position.y, randomPoint.z);

            if (Physics.Raycast(parent.walkPoint, -parent.transform.up, 2f,parent. whatIsGround))
            {
                parent.walkPointSet = true;
            }
        }
        #endregion

        #region HealthModule
        public void CheckHealthForUI()
        {
            if(parent.isDead)
            {
                parent.healthModuleCanvas.SetActive(false);
                return;
            }

            if(parent.currentHealth < parent.maxHealth)
            {
                parent.healthModuleCanvas.SetActive(true);
            }
            else
            {
                parent.healthModuleCanvas.SetActive(false);
            }

            

            if(parent.healthBar.fillAmount != parent.healthBarEase.fillAmount)
            {
                parent.healthBarEase.fillAmount = Mathf.Lerp(parent.healthBarEase.fillAmount, parent.healthBar.fillAmount, parent.healthEaseSpeed * Time.deltaTime);
            }
        }

        private void RegenerateHealth()
        {
            if (parent.isReturningToPatrolArea || !parent.playerInSightRange)
            {
                if (parent.currentHealth >= parent.maxHealth || parent.isDead) return;

                if (Time.time - parent.lastRegenTime >= 1f / parent.healthRegenRate)
                {
                    parent.currentHealth += 1;
                    parent.currentHealth = Mathf.Min(parent.currentHealth, parent.maxHealth); 
                    parent.lastRegenTime = Time.time; 

                    parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;
                }
            }else
            {
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            if (parent.currentHealth <= 0 && !parent.isDead)
            {
                Die();
            }

            if(!parent.isDead)
            {
                parent.currentHealth -= amount;

                if(parent.floatingTextPrefab)
                {
                    Vector3 spawnPosition = parent.transform.position;
                    spawnPosition.y += 1.5f;
                    floatingText _floatingText = Instantiate(parent.floatingTextPrefab, spawnPosition, Quaternion.identity);
                    _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
                }

                parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;

                PlayHitFlash();
                knockBack();

                if (parent.currentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            parent.isDead = true;
            parent.deathEffect.Play();
            parent.pufEffect.Play();
            PlayDeadFlash();
            parent.trail.emitting = false;
            parent.visual.SetActive(false);
            parent.lootBag.InstantiateLoot(parent.transform.position, parent.dropCount);

            GameManager.Instance.experienceModule.AddExperience(parent.experienceValue);
            parent.OnDeath?.Invoke();
            DelayHelper.DelayAction(parent.enemyDestroyTimeOnDead, () =>
            {
                Destroy(parent.gameObject);
            });
        }
        #endregion

        #region Effects Module
        void HandleTrailConditionally(bool shouldEmit)
        {
            if(parent.trail.emitting != shouldEmit)
            {
                handleTrail();
            }
        }
        public void handleTrail()
        {
            parent.trail.emitting = !parent.trail.emitting;
        }
        public void PlayHitFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.flashMaterial;
            }

            DelayHelper.DelayAction(parent.materialChangeDuration, () =>
            {
                if(parent.isDead) return;
                for (int i = 0; i < parent.includedRenderers.Length; i++)
                {
                    parent.includedRenderers[i].material = parent.originalMaterials[i];
                }
            });
        }

        public void PlayDeadFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.deadMaterial;
            }
        }
        public void knockBack()
        {
            parent.transform.DOJump(parent.transform.position, 1f, 1, 0.35f);
        }
        #endregion
    }
    #endregion

    #region RangedAttackAI
    private class RangedAttackAI
    {
        #region AIBrain
        private Enemy parent;

        public RangedAttackAI(Enemy parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            CheckHealthForUI();
            if(parent.isDead || parent.agent.enabled == false) 
            return;
            AIStateBrain();  
            RegenerateHealth(); 
        }

        public void AIStateBrain()
        {
            parent.playerInSightRange = Physics.CheckSphere(parent.transform.position, parent.sightRange, parent.whatIsPlayer);
            parent.playerInAttackRange = Physics.CheckSphere(parent.transform.position, parent.attackRange, parent.whatIsPlayer);

            if (!parent.playerInSightRange && !parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (!IsWithinSpawnArea())
                {
                    parent.isReturningToPatrolArea = true;
                    SetDestinationToSpawnAreaCenter();
                }
                else
                {
                    Patrolling();
                }
            }
            else if (parent.playerInSightRange && !parent.playerInAttackRange)
            {
                ChasePlayer();
                parent.agent.speed = parent.runSpeed;
                parent.agent.acceleration = parent.runAcceleration;
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.playerInAttackRange && parent.playerInSightRange)
            {
                AttackPlayer();
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (IsWithinSpawnArea() && !parent.agent.pathPending)
                {
                    parent.isReturningToPatrolArea = false;
                    Patrolling();
                }
            }
        }

        public void getSpawnerInfo(Vector3 center, float width, float height)
        {
            parent.spawnAreaCenter = center;
            parent.spawnAreaWidth = width;
            parent.spawnAreaHeight = height;
        }

        bool IsWithinSpawnArea()
        {
            return Vector3.Distance(parent.transform.position, parent.spawnAreaCenter) <= Mathf.Max(parent.spawnAreaWidth, parent.spawnAreaHeight) / 2;
        }

        void SetDestinationToSpawnAreaCenter()
        {
            parent.agent.SetDestination(parent.spawnAreaCenter);
        }

        private void Patrolling()
        {
            if (!parent.walkPointSet)
            {
                SearchWalkPoint();
            }
            else
            {
                parent.agent.SetDestination(parent.walkPoint);
            }

            Vector3 distanceToWalkPoint = parent.transform.position - parent.walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                parent.walkPointSet = false;
            }
        }

        private void ChasePlayer()
        {
            parent.agent.SetDestination(parent.player.position);
        }

        private void AttackPlayer()
        {
            parent.agent.SetDestination(parent.transform.position);

            parent.transform.LookAt(parent.player);

            if (!parent.alreadyAttacked)
            {
                parent.alreadyAttacked = true;
                // Attack code here
                if(parent.isRangedAttack)
                {
                    float yOffset = parent.projectileSpawnPoint.position.y;
                    Projectile _projectile = Instantiate(parent.projectile, parent.projectileSpawnPoint.position, Quaternion.identity);
                    _projectile.Fire(parent.damage, parent.player.position, yOffset);
                }
                
                DelayHelper.DelayAction(parent.timeBetweenAttacks, () =>
                {
                    parent.alreadyAttacked = false;
                });
            }
        }

        private void SearchWalkPoint()
        {
            float randomZ = UnityEngine.Random.Range(-parent.spawnAreaHeight / 2, parent.spawnAreaHeight / 2);
            float randomX = UnityEngine.Random.Range(-parent.spawnAreaWidth / 2, parent.spawnAreaWidth / 2);

            Vector3 randomPoint = parent.spawnAreaCenter + new Vector3(randomX, 0, randomZ);
            parent.walkPoint = new Vector3(randomPoint.x, parent.transform.position.y, randomPoint.z);

            if (Physics.Raycast(parent.walkPoint, -parent.transform.up, 2f,parent. whatIsGround))
            {
                parent.walkPointSet = true;
            }
        }
        #endregion

        #region HealthModule
        public void CheckHealthForUI()
        {
            if(parent.isDead)
            {
                parent.healthModuleCanvas.SetActive(false);
                return;
            }

            if(parent.currentHealth < parent.maxHealth)
            {
                parent.healthModuleCanvas.SetActive(true);
            }
            else
            {
                parent.healthModuleCanvas.SetActive(false);
            }

            

            if(parent.healthBar.fillAmount != parent.healthBarEase.fillAmount)
            {
                parent.healthBarEase.fillAmount = Mathf.Lerp(parent.healthBarEase.fillAmount, parent.healthBar.fillAmount, parent.healthEaseSpeed * Time.deltaTime);
            }
        }

        private void RegenerateHealth()
        {
            if (parent.isReturningToPatrolArea || !parent.playerInSightRange)
            {
                if (parent.currentHealth >= parent.maxHealth || parent.isDead) return;

                if (Time.time - parent.lastRegenTime >= 1f / parent.healthRegenRate)
                {
                    parent.currentHealth += 1;
                    parent.currentHealth = Mathf.Min(parent.currentHealth, parent.maxHealth); 
                    parent.lastRegenTime = Time.time; 

                    parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;
                }
            }else
            {
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            if (parent.currentHealth <= 0 && !parent.isDead)
            {
                Die();
            }

            if(!parent.isDead)
            {
                parent.currentHealth -= amount;

                if(parent.floatingTextPrefab)
                {
                    Vector3 spawnPosition = parent.transform.position;
                    spawnPosition.y += 1.5f;
                    floatingText _floatingText = Instantiate(parent.floatingTextPrefab, spawnPosition, Quaternion.identity);
                    _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
                }

                parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;

                PlayHitFlash();
                knockBack();

                if (parent.currentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            parent.isDead = true;
            parent.deathEffect.Play();
            parent.pufEffect.Play();
            PlayDeadFlash();
            parent.trail.emitting = false;
            parent.visual.SetActive(false);
            parent.lootBag.InstantiateLoot(parent.transform.position, parent.dropCount);

            GameManager.Instance.experienceModule.AddExperience(parent.experienceValue);
            parent.OnDeath?.Invoke();
            DelayHelper.DelayAction(parent.enemyDestroyTimeOnDead, () =>
            {
                Destroy(parent.gameObject);
            });
        }
        #endregion

        #region Effects Module
        void HandleTrailConditionally(bool shouldEmit)
        {
            if(parent.trail.emitting != shouldEmit)
            {
                handleTrail();
            }
        }
        public void handleTrail()
        {
            parent.trail.emitting = !parent.trail.emitting;
        }
        public void PlayHitFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.flashMaterial;
            }

            DelayHelper.DelayAction(parent.materialChangeDuration, () =>
            {
                if(parent.isDead) return;
                for (int i = 0; i < parent.includedRenderers.Length; i++)
                {
                    parent.includedRenderers[i].material = parent.originalMaterials[i];
                }
            });
        }

        public void PlayDeadFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.deadMaterial;
            }
        }
        public void knockBack()
        {
            parent.transform.DOJump(parent.transform.position, 1f, 1, 0.35f);
        }
        #endregion
    }
    #endregion

    #region BossAI
    private class BossAI
    {
        #region AIBrain
        private Enemy parent;

        public BossAI(Enemy parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            CheckHealthForUI();
            if(parent.isDead || parent.agent.enabled == false) 
            return;
            AIStateBrain();   
            RegenerateHealth();
        }

        public void AIStateBrain()
        {
            parent.playerInSightRange = Physics.CheckSphere(parent.transform.position, parent.sightRange, parent.whatIsPlayer);
            parent.playerInAttackRange = Physics.CheckSphere(parent.transform.position, parent.attackRange, parent.whatIsPlayer);

            if (!parent.playerInSightRange && !parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (!IsWithinSpawnArea())
                {
                    parent.isReturningToPatrolArea = true;
                    SetDestinationToSpawnAreaCenter();
                }
                else
                {
                    Patrolling();
                }
            }
            else if (parent.playerInSightRange && !parent.playerInAttackRange)
            {
                ChasePlayer();
                parent.agent.speed = parent.runSpeed;
                parent.agent.acceleration = parent.runAcceleration;
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.playerInAttackRange && parent.playerInSightRange)
            {
                AttackPlayer();
                parent.isReturningToPatrolArea = false;
            }
            else if (parent.isReturningToPatrolArea)
            {
                parent.agent.speed = parent.movementSpeed;
                parent.agent.acceleration = parent.acceleration;
                if (IsWithinSpawnArea() && !parent.agent.pathPending)
                {
                    parent.isReturningToPatrolArea = false;
                    Patrolling();
                }
            }
        }

        public void getSpawnerInfo(Vector3 center, float width, float height)
        {
            parent.spawnAreaCenter = center;
            parent.spawnAreaWidth = width;
            parent.spawnAreaHeight = height;
        }

        bool IsWithinSpawnArea()
        {
            return Vector3.Distance(parent.transform.position, parent.spawnAreaCenter) <= Mathf.Max(parent.spawnAreaWidth, parent.spawnAreaHeight) / 2;
        }

        void SetDestinationToSpawnAreaCenter()
        {
            parent.agent.SetDestination(parent.spawnAreaCenter);
        }

        private void Patrolling()
        {
            if (!parent.walkPointSet)
            {
                SearchWalkPoint();
            }
            else
            {
                parent.agent.SetDestination(parent.walkPoint);
            }

            Vector3 distanceToWalkPoint = parent.transform.position - parent.walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                parent.walkPointSet = false;
            }
        }

        private void ChasePlayer()
        {
            parent.agent.SetDestination(parent.player.position);
        }

        private void AttackPlayer()
        {
            parent.agent.SetDestination(parent.transform.position);

            parent.transform.LookAt(parent.player);

            if (!parent.alreadyAttacked)
            {
                parent.alreadyAttacked = true;
                // Attack code here
                if(parent.isRangedAttack)
                {
                    float yOffset = parent.projectileSpawnPoint.position.y;
                    Projectile _projectile = Instantiate(parent.projectile, parent.projectileSpawnPoint.position, Quaternion.identity);
                    _projectile.Fire(parent.damage, parent.player.position, yOffset);
                }
                
                DelayHelper.DelayAction(parent.timeBetweenAttacks, () =>
                {
                    parent.alreadyAttacked = false;
                });
            }
        }

        private void SearchWalkPoint()
        {
            float randomZ = UnityEngine.Random.Range(-parent.spawnAreaHeight / 2, parent.spawnAreaHeight / 2);
            float randomX = UnityEngine.Random.Range(-parent.spawnAreaWidth / 2, parent.spawnAreaWidth / 2);

            Vector3 randomPoint = parent.spawnAreaCenter + new Vector3(randomX, 0, randomZ);
            parent.walkPoint = new Vector3(randomPoint.x, parent.transform.position.y, randomPoint.z);

            if (Physics.Raycast(parent.walkPoint, -parent.transform.up, 2f,parent. whatIsGround))
            {
                parent.walkPointSet = true;
            }
        }
        #endregion

        #region HealthModule
        public void CheckHealthForUI()
        {
            if(parent.isDead)
            {
                parent.healthModuleCanvas.SetActive(false);
                return;
            }

            if(parent.currentHealth < parent.maxHealth)
            {
                parent.healthModuleCanvas.SetActive(true);
            }
            else
            {
                parent.healthModuleCanvas.SetActive(false);
            }

            

            if(parent.healthBar.fillAmount != parent.healthBarEase.fillAmount)
            {
                parent.healthBarEase.fillAmount = Mathf.Lerp(parent.healthBarEase.fillAmount, parent.healthBar.fillAmount, parent.healthEaseSpeed * Time.deltaTime);
            }
        }

        private void RegenerateHealth()
        {
            if (parent.isReturningToPatrolArea || !parent.playerInSightRange)
            {
                if (parent.currentHealth >= parent.maxHealth || parent.isDead) return;

                if (Time.time - parent.lastRegenTime >= 1f / parent.healthRegenRate)
                {
                    parent.currentHealth += 1;
                    parent.currentHealth = Mathf.Min(parent.currentHealth, parent.maxHealth); 
                    parent.lastRegenTime = Time.time; 

                    parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;
                }
            }else
            {
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            if (parent.currentHealth <= 0 && !parent.isDead)
            {
                Die();
            }

            if(!parent.isDead)
            {
                parent.currentHealth -= amount;

                if(parent.floatingTextPrefab)
                {
                    Vector3 spawnPosition = parent.transform.position;
                    spawnPosition.y += 1.5f;
                    floatingText _floatingText = Instantiate(parent.floatingTextPrefab, spawnPosition, Quaternion.identity);
                    _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
                }

                parent.healthBar.fillAmount = (float)parent.currentHealth / parent.maxHealth;

                PlayHitFlash();
                knockBack();

                if (parent.currentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            parent.isDead = true;
            parent.deathEffect.Play();
            parent.pufEffect.Play();
            PlayDeadFlash();
            parent.trail.emitting = false;
            parent.visual.SetActive(false);
            parent.lootBag.InstantiateLoot(parent.transform.position, parent.dropCount);

            GameManager.Instance.experienceModule.AddExperience(parent.experienceValue);
            parent.OnDeath?.Invoke();
            DelayHelper.DelayAction(parent.enemyDestroyTimeOnDead, () =>
            {
                Destroy(parent.gameObject);
            });
        }
        #endregion

        #region Effects Module
        void HandleTrailConditionally(bool shouldEmit)
        {
            if(parent.trail.emitting != shouldEmit)
            {
                handleTrail();
            }
        }
        public void handleTrail()
        {
            parent.trail.emitting = !parent.trail.emitting;
        }
        public void PlayHitFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.flashMaterial;
            }

            DelayHelper.DelayAction(parent.materialChangeDuration, () =>
            {
                if(parent.isDead) return;
                for (int i = 0; i < parent.includedRenderers.Length; i++)
                {
                    parent.includedRenderers[i].material = parent.originalMaterials[i];
                }
            });
        }

        public void PlayDeadFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.deadMaterial;
            }
        }
        public void knockBack()
        {
            parent.transform.DOJump(parent.transform.position, 1f, 1, 0.35f);
        }
        #endregion
    }
    #endregion

    #region NoBehaviorAI
    private class NoBehaviorAI
    {
        #region AIBrain
        private Enemy parent;
        private int hitCount = 0;

        public NoBehaviorAI(Enemy parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            if(parent.isDead) 
            return;
            RegenerateHealth();
        }
        #endregion

        #region HealthModule
        private void RegenerateHealth()
        {
            parent.playerInSightRange = Physics.CheckSphere(parent.transform.position, parent.sightRange, parent.whatIsPlayer);
            if (!parent.playerInSightRange)
            {
                if (hitCount >= parent.requiredHitsToDie || parent.isDead) return;

                if (Time.time - parent.lastRegenTime >= 1f / parent.healthRegenRate)
                {
                    DelayHelper.DelayAction(1f, () =>
                    {
                        Transform target = parent.visual.transform;
                        target.DOScale(1, 0.35f);
                        hitCount = 0;
                        parent.lastRegenTime = Time.time; 
                    });
                }
            }else
            {
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            /*if(parent.floatingTextPrefab)
            {
                Vector3 spawnPosition = parent.transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(parent.floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 6f);
            }*/

            parent.lootBag.InstantiateLoot(parent.transform.position, parent.dropCount);
            GameManager.Instance.experienceModule.AddExperience(parent.experienceValue);

            hitCount++;

            if (hitCount >= parent.requiredHitsToDie && !parent.isDead)
            {
                Die();
            }

            if(!parent.isDead)
            {
                PlayHitFlash();
                knockBack();
                ScaleDown();
                
                if (hitCount >= parent.requiredHitsToDie)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            parent.isDead = true;
            parent.pufEffect.Play();
            PlayDeadFlash();
            parent.visual.SetActive(false);
            
            parent.OnDeath?.Invoke();
            DelayHelper.DelayAction(parent.enemyDestroyTimeOnDead, () =>
            {
                Destroy(parent.gameObject);
            });
        }
        #endregion

        #region Effects Module
        public void PlayHitFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.flashMaterial;
            }

            DelayHelper.DelayAction(parent.materialChangeDuration, () =>
            {
                if(parent.isDead) return;
                for (int i = 0; i < parent.includedRenderers.Length; i++)
                {
                    parent.includedRenderers[i].material = parent.originalMaterials[i];
                }
            });
        }

        public void PlayDeadFlash()
        {
            foreach (var renderer in parent.includedRenderers)
            {
                renderer.material = parent.deadMaterial;
            }
        }
        public void knockBack()
        {
            parent.transform.DOJump(parent.transform.position, 1f, 1, 0.35f);
        }
        public void ScaleDown()
        {
            Transform target = parent.visual.transform;
            target.DOScale(target.localScale * 0.75f, 0.35f);
        }
        #endregion
    }
    #endregion

    #region UtilityForFlashModule
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

    #region Debug
    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, walkPointRange);
        }
    }
    #endregion
}
