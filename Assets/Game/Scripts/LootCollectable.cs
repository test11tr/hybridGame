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
        if(GameManager.Instance.player.virtualWallet.currentCargo >= GameManager.Instance.player.virtualWallet.maxCargo)
        {
            print("Wallet is Full");
            isCollectable = true;
        }else
        {
            Collect();
        }
    }

    private void Collect()
    {
        Vector3 playerPos = GameManager.Instance.player.collectTarget.position;
        transform.DOJump(playerPos, jumpPower, 1, collectDuration).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            if(collectableType == CollectableType.Coin)
                GameManager.Instance.player.virtualWallet.AddCoin(1);
            else if(collectableType == CollectableType.Gem)
                GameManager.Instance.player.virtualWallet.AddGem(1);
            else if(collectableType == CollectableType.Wood)
                GameManager.Instance.player.virtualWallet.AddWood(1);
            else if(collectableType == CollectableType.Stone)
                GameManager.Instance.player.virtualWallet.AddStone(1);

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
