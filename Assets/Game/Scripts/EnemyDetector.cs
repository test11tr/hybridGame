using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
public CharacterControlManager characterControlManager;

    private void OnTriggerEnter(Collider other)
    {
        characterControlManager.HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        characterControlManager.HandleTriggerExit(other);
    }
}
