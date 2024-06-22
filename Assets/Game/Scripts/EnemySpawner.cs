using UnityEngine;
using Shapes;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 10f;
    public GameObject enemyPrefab;
    public int numberOfEnemies = 5;
    public bool respawnEnemies = true;
    public float spawnCooldown = 10f;

    [Header("Spawner Visual")]
    public TextMeshPro cooldownText;
    public Rectangle bg;
    public Rectangle outline;
    public bool showSpawnArea;
    public bool showText;

    [Header("Debug")]
    [SerializeField]private float currentCooldownTime = 0f;
    [SerializeField] private int activeEnemies = 0;

    private void Start()
    {
        SpawnEnemies();
        currentCooldownTime = spawnCooldown;
        
        bg.Width = spawnAreaWidth;
        bg.Height = spawnAreaHeight;
        outline.Width = spawnAreaWidth;
        outline.Height = spawnAreaHeight;

        if(showSpawnArea)
        {
            bg.gameObject.SetActive(true);
            outline.gameObject.SetActive(true);
        }
        else
        {
            bg.gameObject.SetActive(false);
            outline.gameObject.SetActive(false);
        }

        cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (activeEnemies <= 0 && respawnEnemies)
        {
            if (currentCooldownTime > 0)
            {
                if(showText)
                {
                    cooldownText.gameObject.SetActive(true);
                    cooldownText.text = "NEXT SPAWNS IN " + currentCooldownTime.ToString("F2") + "s";
                }
                currentCooldownTime -= Time.deltaTime;
            }
            else
            {
                if(showText)
                {
                    cooldownText.gameObject.SetActive(false);
                }
                SpawnEnemies();
                currentCooldownTime = spawnCooldown;
            }
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                0,
                Random.Range(-spawnAreaHeight / 2, spawnAreaHeight / 2)
            ) + transform.position; 

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.GetComponent<Enemy>().getSpawnerInfo(transform.position, spawnAreaWidth, spawnAreaHeight);
            enemy.GetComponent<Enemy>().OnDeath += Enemy_OnDeath; 
        }
        activeEnemies = numberOfEnemies;
    }

    private void Enemy_OnDeath()
    {
        activeEnemies--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaWidth, 1, spawnAreaHeight));
    }
}