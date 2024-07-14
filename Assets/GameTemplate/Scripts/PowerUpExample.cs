using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PowerUpExample : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public string powerUpName;
    public string floatText;
    public int floatFontSize;
    public Color floatColor;

    [Header("PowerUp Specialized Settings")]
    //Example: public int healthAmount;

    [Header("Other Settings")]
    public floatingText floatingTextPrefab;
    public float collectDuration;
    public float jumpPower;
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
        Vector3 playerPos = GameManager.Instance.player.collectTarget.position;
        transform.DOJump(playerPos, jumpPower, 1, collectDuration).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Destroy(gameObject);
            ShowText();
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
