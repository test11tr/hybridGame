using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Obstacle : MonoBehaviour
{
    [Foldout("Obstacle Settings", foldEverything = true, styled = true, readOnly = false)]
    public float obstacleDamage;
    public float shakeIntensity;
    public float shakeFrequency;
    public float shakeDuration;
    public float hitDelay;

    private bool isUsing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isUsing) return;
            isUsing = true;
            GameManager.Instance.player.TakeDamage(obstacleDamage);
            GameManager.Instance.player.knockBack();
            GameManager.Instance.soundModule.PlaySound("hitCharacter");
            GameManager.Instance.cameraShakeModule.ShakeCamera(shakeIntensity, shakeFrequency, shakeDuration);
            DelayHelper.DelayAction(hitDelay, () => isUsing = false);
        }
    }
}
