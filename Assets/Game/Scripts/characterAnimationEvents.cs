using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterAnimationEvents : MonoBehaviour
{
    public CharacterControlManager player;

    void Start()
    {
        player = GetComponentInParent<CharacterControlManager>();
    }

    void dealDamage()
    {
        player.DealMeleeDamage();
    }
}
