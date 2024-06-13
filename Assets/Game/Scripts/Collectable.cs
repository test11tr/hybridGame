using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float speed;
    public TrailRenderer trail;
    private bool isMoving;

    private void Update()
    {
        if (isMoving)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(!isMoving)
            {
                isMoving = true;
                trail.emitting = true;
                //gameObject.SetActive(false);
            }
                
        }
    }
}
