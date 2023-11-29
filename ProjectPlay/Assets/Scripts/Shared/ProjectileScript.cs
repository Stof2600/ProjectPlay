using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float Speed = 10;
    public int Damage;

    public GameObject EndEffect;

    // Update is called once per frame
    void Update()
    {
        Vector3 OldPos = transform.position;
        transform.position += transform.forward * Speed * Time.deltaTime;

        if(Physics.Linecast(OldPos, transform.position, out RaycastHit Hit))
        {
            if(EndEffect)
            {
                Instantiate(EndEffect, Hit.point, transform.rotation);
                Destroy(gameObject);
                return;
            }

            if(Hit.transform.GetComponent<PlayerController>())
            {
                Hit.transform.GetComponent<PlayerController>().TakeDamage(Damage);
            }

            Destroy(gameObject);
        }
    }
}
