using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScript : MonoBehaviour
{
    public int MaxHealth, CurrentHealth;
    public int DeathScore = 5;


    bool SpawnedDrop;

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        SpawnedDrop = false;
    }

    public void TakeDamage(int Amount)
    {
        CurrentHealth -= Amount;

        if(CurrentHealth <= 0)
        {
            if(DeathScore > 0 && GetComponent<EnemyController>())
            {
                FindObjectOfType<ScoreManager>().AddScore(DeathScore);

                EnemyController EC = GetComponent<EnemyController>();
                int R = Random.Range(0, EC.DeathDrops.Count + 5);
                if(R < EC.DeathDrops.Count && !SpawnedDrop)
                {
                    Instantiate(EC.DeathDrops[R], transform.position, transform.rotation);
                    SpawnedDrop=true;
                }

                Destroy(gameObject);
            }
            if(GetComponent<PlayerController>())
            {
                GetComponent<PlayerController>().KillPlayer("DIED TO ENEMY");
            }
        }
        if(CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
}
