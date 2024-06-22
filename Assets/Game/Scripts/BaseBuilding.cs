using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilding : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("here");
            CharacterControlManager player = GameManager.Instance.player;
            if(player.virtualWallet != null)
            {
                player.virtualWallet.TransferToWallet();
            }
        }
    }
}
