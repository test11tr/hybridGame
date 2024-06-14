using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : PowerUpEffect
{
    public float healthAmount;

    public override void Apply(GameObject target)
    {
        //target.GetComponent<Health>().Heal(healthAmount);
    }
}
