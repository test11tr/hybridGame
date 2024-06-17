using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 10f;
    public GameObject enemyPrefab;
    public int numberOfEnemies = 5;
    public float spawnCooldown = 10f;

    private float currentCooldownTime = 0f;
    private int activeEnemies = 0;

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        if (activeEnemies <= 0)
        {
            if (currentCooldownTime > 0)
            {
                currentCooldownTime -= Time.deltaTime;
            }
            else
            {
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