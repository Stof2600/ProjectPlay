using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float Radius = 4;
    public int Damage = 20;

    // Start is called before the first frame update
    void Start()
    {
        Collider[] InRange = Physics.OverlapSphere(transform.position, Radius);

        foreach(Collider Col in InRange)
        {
            if(Col.GetComponent<PlayerController>())
            {
                Col.GetComponent<PlayerController>().TakeDamage(Damage);
                Col.GetComponent<PlayerController>().StunTimer = 0.2f;
            }
            if(Col.GetComponent<Rigidbody>())
            {
                Rigidbody RB = Col.GetComponent<Rigidbody>();

                Vector3 LaunchDir = (transform.position - RB.position).normalized;
                LaunchDir *= -1;
                RB.AddForce(LaunchDir * 20, ForceMode.Impulse);
            }
            if (Col.GetComponent<EnemyController>())
            {
                Col.GetComponent<EnemyController>().TakeDamage(Damage);
            }
        }
    }
}
