using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorModule : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            Collectable collectable = other.GetComponent<Collectable>();
            collectable.TriggerAction(gameObject);
        }
    }
}
