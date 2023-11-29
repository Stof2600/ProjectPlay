using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : StatScript
{

    Transform Target;
    NavMeshAgent Agent;

    public GameObject ProjectilePrefab;
    public float FireRate = 1;
    float FireTime;

    // Start is called before the first frame update
    void Start()
    {
        Target = FindObjectOfType<PlayerController>().transform;
        Agent = GetComponent<NavMeshAgent>();

        ResetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        Agent.SetDestination(Target.position);

        FireTime += Time.deltaTime;

        if(FireTime >= FireRate)
        {
            Instantiate(ProjectilePrefab, transform.position, transform.rotation);
            FireTime = 0;
        }
    }
}
