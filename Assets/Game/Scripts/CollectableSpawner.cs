using System;
using System.Collections;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    public GameObject collectablePrefab;
    public Shapes.Disc progressShape; 
    public float waitTime;
    public float spawnWaitTime;

    private bool isPlayerInside = false;
    private float timer = 0f;
    private float spawnTimer = 0f;

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
                    SpawnAndLaunchCollectable();
                    spawnTimer = 0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter: "+other.tag);
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

    private void SpawnAndLaunchCollectable()
    {
        GameObject collectable = Instantiate(collectablePrefab, transform.position, Quaternion.identity);
        collectable.GetComponent<Collectable>().toRandomPosition();
    }
}