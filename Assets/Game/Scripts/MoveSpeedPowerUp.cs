using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveSpeedPowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public string powerUpName;
    public string floatText;
    public int floatFontSize;
    public Color floatColor;

    [Header("PowerUp Specialized Settings")]
    public float speedMultiplier;
    public int powerUpDuration;

    [Header("Other Settings")]
    public floatingText floatingTextPrefab;
    public float collectDuration;
    public TrailRenderer trail;
    private Vector3 playerPos;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trail.emitting = true;
            playerPos = other.transform.position;
            MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {
        transform.DOMove(playerPos * 2, collectDuration * 2).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Vector3 playerPos = GameManager.Instance.player.rb.transform.position;
            transform.DOMove(playerPos, collectDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                GameManager.Instance.player.SpeedUp(speedMultiplier, powerUpDuration);
                GameManager.Instance.player.PlayPowerUpEffect();
                Destroy(gameObject);
                ShowText();
            });
        });
    }

    private void ShowText()
    {
        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = GameManager.Instance.player.rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText(floatText, floatColor, floatFontSize);
        }
    }
}
