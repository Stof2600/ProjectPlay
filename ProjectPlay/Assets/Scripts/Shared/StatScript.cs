using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScript : MonoBehaviour
{
    public int MaxHealth, CurrentHealth;

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int Amount)
    {
        CurrentHealth -= Amount;

        if(CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
