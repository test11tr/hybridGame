using System;
using System.Collections;
using System.Collections.Generic;
using EpicToonFX;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Movement")]
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    public float movementSpeed;
    public float rotationSpeed;
    public float acceleration;

    [Header("Enemy Attack")]
    public int damage;
    public float timeBetweenAttacks;
    public bool isRangedAttack;
    public Projectile projectile;
    public Transform projectileSpawnPoint;
    bool alreadyAttacked;
    public int enemyDestroyTimeOnDead;

    [Header("Enemy States")]
    public float sightRange;
    public float attackRange;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;
    private int currentHealth;

    [Header("References")]
    public Rigidbody rb;
    public Animator enemyAnimator;
    public GameObject floatingTextPrefab;
    private Transform player;

    [Header("Debug")]
    public bool drawGizmos;

    [HideInInspector] public Vector3 spawnAreaCenter;
    [HideInInspector] public float spawnAreaWidth;
    [HideInInspector] public float spawnAreaHeight;
    private bool isReturningToPatrolArea = false;
    private bool isDead;

    public Action OnDeath { get; internal set; }

    void Start()
    {
        player = GameManager.Instance.player.rb.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.acceleration = acceleration;
        currentHealth = maxHealth;
    }

    void Update()
    {
        CheckHealthForUI();     

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !isReturningToPatrolArea)
        {
            if (!IsWithinSpawnArea())
            {
                isReturningToPatrolArea = true;
                SetDestinationToSpawnAreaCenter();
            }
            else
            {
                Patrolling();
            }
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            isReturningToPatrolArea = false;
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
            isReturningToPatrolArea = false;
        }
        else if (isReturningToPatrolArea)
        {
            if (IsWithinSpawnArea() && !agent.pathPending)
            {
                isReturningToPatrolArea = false;
                Patrolling();
            }
        }
    }

    public void getSpawnerInfo(Vector3 center, float width, float height)
    {
        spawnAreaCenter = center;
        spawnAreaWidth = width;
        spawnAreaHeight = height;
    }

    bool IsWithinSpawnArea()
    {
        return Vector3.Distance(transform.position, spawnAreaCenter) <= Mathf.Max(spawnAreaWidth, spawnAreaHeight) / 2;
    }

    void SetDestinationToSpawnAreaCenter()
    {
        agent.SetDestination(spawnAreaCenter);
    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            // Attack code here
            if(isRangedAttack)
            {
                float yOffset = projectileSpawnPoint.position.y;
                Projectile _projectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                _projectile.Fire(damage, player.position, yOffset);
            }
            
            DelayHelper.DelayAction(timeBetweenAttacks, () =>
            {
                alreadyAttacked = false;
            });
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-spawnAreaHeight / 2, spawnAreaHeight / 2);
        float randomX = UnityEngine.Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);

        Vector3 randomPoint = spawnAreaCenter + new Vector3(randomX, 0, randomZ);
        walkPoint = new Vector3(randomPoint.x, transform.position.y, randomPoint.z);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
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

    void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

    public void Die()
    {
        enemyAnimator.SetBool("isDead", true);
        isDead = true;
        OnDeath?.Invoke();
        DelayHelper.DelayAction(enemyDestroyTimeOnDead, () =>
        {
            Destroy(gameObject);
        });
    }

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
}
