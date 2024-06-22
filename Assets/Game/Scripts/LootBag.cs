using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LootBag : MonoBehaviour
{
    public LootCollectable droppedItemPrefab;
    public List<Loot> lootList = new List<Loot>();
    public float maxDropDistance = 1.5f;

    Loot GetDroppedItem()
    {
        int totalDropChance = 0;
        foreach (Loot item in lootList)
        {
            totalDropChance += item.dropChance;
        }

        int randomNumber = Random.Range(1, totalDropChance + 1);
        int cumulativeChance = 0;

        foreach (Loot item in lootList)
        {
            cumulativeChance += item.dropChance;
            if (randomNumber <= cumulativeChance)
            {
                return item;
            }
        }

        Debug.LogError("Hata: Loot seçimi yapılamadı.");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition, int itemCount)
    {
        for(int i = 0; i < itemCount; i++)
        {
            Loot droppedItem = GetDroppedItem();
            if(droppedItem != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere * maxDropDistance;
                randomOffset.y = 0;
                Vector3 spawnPositionWithOffset = spawnPosition + randomOffset;

                LootCollectable lootGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
                lootGameObject.initCollectable(droppedItem.lootName);
                lootGameObject.GetComponentInChildren<SpriteRenderer>().sprite = droppedItem.lootSprite;
                lootGameObject.transform.DOJump(spawnPositionWithOffset, 2.5f, 1, .5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    lootGameObject.isCollectable = true;
                });
            }
        }
    }
}
