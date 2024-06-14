using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : MonoBehaviour
{
    public enum CollectableType
    {
        Gold,
        Gem
    }

    public CollectableType collectableType;
    public floatingText floatingTextPrefab;
    public float randomSpreadDistance;
    public float collectDuration;
    public TrailRenderer trail;
    private bool isMoving;
    private Vector3 playerPos;
    private bool isCollectable;

    public void toRandomPosition()
    {
        trail.emitting = true;
        Vector3 launchDirection = new Vector3(Random.Range(-randomSpreadDistance, randomSpreadDistance), 1, Random.Range(-randomSpreadDistance, randomSpreadDistance));
        transform.DOMove(launchDirection, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            isCollectable = true;
            trail.emitting = false;
            
        });

        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }

    public void TriggerAction(GameObject other)
    {
        if(!isMoving && isCollectable)
        {
            trail.emitting = true;
            playerPos = other.transform.position;
            isMoving = true;
            MoveToPlayer();
        }        
    }

    private void MoveToPlayer()
    {
        transform.DOMove(playerPos * 2, collectDuration * 2).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Vector3 playerPos = CharacterControlManager.Instance.rb.transform.position;
            transform.DOMove(playerPos, collectDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                isMoving = false;

                if(collectableType == CollectableType.Gold)
                    WalletModule.Instance.AddGold(1);
                else if(collectableType == CollectableType.Gem)
                    WalletModule.Instance.AddGem(1);

                ShowText();
                Destroy(gameObject);
            });
        });
    }

    private void ShowText()
    {
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = CharacterControlManager.Instance.rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText("+1", Color.white, 4f);
        }
    }
}
