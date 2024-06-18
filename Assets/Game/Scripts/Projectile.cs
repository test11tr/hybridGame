using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public bool isPlayerProjectile;
    public float bulletSpeed;
    public float bulletMaxLifeTime;
    public Rigidbody rb;
    public GameObject impactEffect;

    private int _bulletDamage;
    private Vector3 _startPos;

    public void Fire(int damage, Vector3 targetPosition, float yOffset)
    {
        _startPos = transform.position;
        _bulletDamage = damage;
        targetPosition.y += yOffset;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        rb.AddForce(directionToTarget * bulletSpeed, ForceMode.Impulse);
        Destroy(gameObject, bulletMaxLifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(isPlayerProjectile)
        {
            if (other.CompareTag("Enemy"))
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
                other.GetComponent<Enemy>().TakeDamage(_bulletDamage);
                Destroy(gameObject);
            }
        }else
        {
            if (other.CompareTag("Player"))
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
                GameManager.Instance.player.TakeDamage(_bulletDamage);
                Destroy(gameObject);
            }
        }
    }
}
