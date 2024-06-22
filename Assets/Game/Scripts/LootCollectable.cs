using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LootCollectable : MonoBehaviour
{
    public enum CollectableType
    {
        Coin,
        Gem,
        Wood,
        Stone
    }

    private CollectableType collectableType;
    public floatingText floatingTextPrefab;
    public float jumpPower;
    public float collectDuration;
    [HideInInspector] public bool isCollectable;

    public void initCollectable(string name)
    {
        print(name);
        if(name == "Coin")
        {
            collectableType = CollectableType.Coin;
        }else if(name == "Gem")
        {
            collectableType = CollectableType.Gem;
        }else if(name == "Wood")
        {
            collectableType = CollectableType.Wood;
        }else if(name == "Stone")
        {
            collectableType = CollectableType.Stone;
        }
    }

    public void TriggerAction(GameObject other)
    {
        if(isCollectable)
        {
            isCollectable = false;
            MoveToPlayer();
        }        
    }

    private void MoveToPlayer()
    {
        Vector3 playerPos = GameManager.Instance.player.rb.transform.position;
        transform.DOJump(playerPos, jumpPower, 1, collectDuration).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            if(collectableType == CollectableType.Coin)
                GameManager.Instance.wallet.AddCoin(1);
            else if(collectableType == CollectableType.Gem)
                GameManager.Instance.wallet.AddGem(1);
            else if(collectableType == CollectableType.Wood)
                GameManager.Instance.wallet.AddWood(1);
            else if(collectableType == CollectableType.Stone)
                GameManager.Instance.wallet.AddStone(1);

            ShowText();
            Destroy(gameObject);
        });
    }

    private void ShowText()
    {
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = GameManager.Instance.player.rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText("+1", Color.white, 6f);
        }
    }
}
