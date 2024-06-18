using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthPowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public string powerUpName;
    //public string floatText;
    //public int floatFontSize;
    //public Color floatColor;
    
    [Header("PowerUp Specialized Settings")]
    public int healthAmount;

    [Header("Other Settings")]
    public floatingText floatingTextPrefab;
    public float collectDuration;
    public TrailRenderer trail;
    public ParticleSystem collectEffect;

    private Vector3 playerPos;
    


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.Instance.player.currentHealth == GameManager.Instance.player.maxHealth)
            {
                ShowWarningText();
                return;
            }

            trail.emitting = true;
            playerPos = other.transform.position;
            MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {
        transform.DOMove(playerPos * 1.5f, collectDuration * 2).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            GameManager.Instance.player.playerAnimator.SetTrigger("PowerUpCollected");
            Vector3 playerPos = GameManager.Instance.player.rb.transform.position;
            transform.DOMove(playerPos, collectDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                GameManager.Instance.player.AddHealth(healthAmount);
                GameManager.Instance.player.PlayPowerUpEffect();
                GameManager.Instance.player.PlayHealEffect();
                GameManager.Instance.player.playerAnimator.ResetTrigger("PowerUpCollected");
                Destroy(gameObject);
                //ShowText(); no need for HealthPowerUp 
            });
        });
    }

    /*private void ShowText()
    {
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = GameManager.Instance.player.rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText(floatText, floatColor, floatFontSize);
        }
    }*/

    private void ShowWarningText()
    {
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = GameManager.Instance.player.rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText("Your health is Full!", Color.white, 5);
        }
    }
}
