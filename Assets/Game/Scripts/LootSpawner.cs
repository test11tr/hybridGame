using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LootSpawner : MonoBehaviour
{
    [Header("Loot Settings")]
    public LootCollectable droppedItemPrefab;
    public Loot loot;
    
    [Header("Spawner Settings")]
    public Shapes.Disc progressShape; 
    public float maxDropDistance = 3f;    
    public float waitTime;
    public float spawnWaitTime;
    public SpriteRenderer lootIcon;

    private bool isPlayerInside = false;
    private float timer = 0f;
    private float spawnTimer = 0f;

    void Start()
    {
        lootIcon.sprite = loot.lootSprite;
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            timer += Time.deltaTime;
            if(progressShape.AngRadiansEnd < 360)
            {
                float totalIncrease = 2 * Mathf.PI;
                float increasePerSecond = totalIncrease / waitTime;
                progressShape.AngRadiansEnd += increasePerSecond * Time.deltaTime;
            }
                
            if (timer >= waitTime)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnWaitTime)
                {
                    SpawnAndLaunchLoot();
                    spawnTimer = 0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            timer = 0f; 
            spawnTimer = 0f; 
            progressShape.AngRadiansEnd = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            timer = 0f; 
            spawnTimer = 0f; 
            progressShape.AngRadiansEnd = 0;
        }
    }

    private void SpawnAndLaunchLoot()
    {
        if(loot != null)
        {
            Vector3 spawnPosition = transform.position;
            Vector3 randomOffset = Random.insideUnitSphere * maxDropDistance;
            randomOffset.y = 0;
            Vector3 spawnPositionWithOffset = spawnPosition + randomOffset;

            LootCollectable lootGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
            lootGameObject.initCollectable(loot.lootName);
            lootGameObject.GetComponentInChildren<SpriteRenderer>().sprite = loot.lootSprite;
            lootGameObject.transform.DOJump(spawnPositionWithOffset, 2.5f, 1, .5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                lootGameObject.isCollectable = true;
            });
        }
    }
}