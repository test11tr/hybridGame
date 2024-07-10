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
    public GameObject impactEffect2;

    private float _bulletDamage;
    private bool _isCriticalHit;
    private Vector3 _startPos;

    public void Fire(float damage, Vector3 targetPosition, float yOffset, bool isCriticalHit)
    {
        _startPos = transform.position;
        _bulletDamage = damage;
        _isCriticalHit = isCriticalHit;
        targetPosition.y += yOffset;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        rb.AddForce(directionToTarget * bulletSpeed, ForceMode.Impulse);
        transform.LookAt(targetPosition);
        Destroy(gameObject, bulletMaxLifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(isPlayerProjectile)
        {
            if (other.CompareTag("Enemy"))
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
                if(impactEffect2 != null)
                {
                    Instantiate(impactEffect2, other.transform.position, Quaternion.identity);
                }
                if(_isCriticalHit)
                    other.GetComponent<Enemy>().TakeDamage(_bulletDamage, true);
                else
                    other.GetComponent<Enemy>().TakeDamage(_bulletDamage, false);
                Destroy(gameObject);
            }
        }else
        {
            if (other.CompareTag("Player"))
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
                if(impactEffect2 != null)
                {
                    Instantiate(impactEffect2, other.transform.position, Quaternion.identity);
                }
                GameManager.Instance.player.TakeDamage(_bulletDamage);
                Destroy(gameObject);
            }
        }
    }
}
