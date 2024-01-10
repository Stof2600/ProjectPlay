using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : StatScript
{
    Transform Target;
    NavMeshAgent Agent;

    public int Damage = 1;
    public GameObject ProjectilePrefab;
    public float MeleeRange = 3;
    public float FireRateMin = 1, FireRateMax = 4;
    float FireTime;

    public GameObject[] Sprites; //0=front 1=side 2=back

    AIState CurrentState;
    public AIType Type;

    NavMeshPath Path;

    public float DetectionRadius = 40;
    float TargetDistance;
    bool TargetInRange;

    Vector3 RandomPoint;
    float RunTime = 15;
    float StateTimer;

    float StopDis;

    public float DamageTick;

    public List<GameObject> DeathDrops = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Target = FindObjectOfType<PlayerController>().transform;
        Agent = GetComponent<NavMeshAgent>();

        ResetHealth();

        StopDis = Agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        SpriteChange();

        if (Type == AIType.Dummy)
        {
            return;
        }

        TargetDetection();

        if(CurrentState == AIState.Running)
        {
            RunFromTarget();
            Agent.stoppingDistance = 0;
        }
        else if(CurrentState == AIState.Chasing)
        {
            ChaseTarget();
            Agent.stoppingDistance = StopDis;
        }
        else if(CurrentState == AIState.Idle)
        {
            Agent.SetDestination(transform.position);
            StateTimer -= Time.deltaTime;

            if(StateTimer <= 0)
            {
                CurrentState = AIState.Roaming;
            }
        }
        else if(CurrentState == AIState.Roaming && Type != AIType.NoMove)
        {
            Roam();
            Agent.stoppingDistance = 0;
        }
    }

    void TargetDetection()
    {
        TargetDistance = Vector3.Distance(Target.position, transform.position);

        if (TargetDistance <= DetectionRadius)
        {
            TargetInRange = true;

            if(Type == AIType.Agressive || Type == AIType.NoMove)
            {
                CurrentState = AIState.Chasing;
            }
            else if(Type == AIType.Passive)
            {
                CurrentState = AIState.Running;
            }
        }
        else
        {
            TargetInRange = false;

            if(CurrentState == AIState.Chasing && Type != AIType.NoMove)
            {
                CurrentState = AIState.Roaming;
            }
        }
    }

    void Roam()
    {
        if(RandomPoint == Vector3.zero)
        {
            RandomPoint = transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));

            Path = new NavMeshPath();
            if(Agent.CalculatePath(RandomPoint, Path) && Path.status == NavMeshPathStatus.PathComplete)
            {
                print("PathComplete");
            }
            else
            {
                RandomPoint = Vector3.zero;
            }
        }
        else
        {
            Agent.SetDestination(RandomPoint);

            if(Vector3.Distance(transform.position, RandomPoint) < 1 || (transform.position.x == RandomPoint.x && transform.position.z == RandomPoint.z) || Agent.isPathStale)
            {
                StateTimer = Random.Range(3, 8);
                CurrentState = AIState.Idle;
                RandomPoint = Vector3.zero;
            }
        }
    }
    void ChaseTarget()
    {
        Vector3 DirToTarget = (Target.position - transform.position).normalized;

        if (Type != AIType.NoMove)
        {
            Agent.SetDestination(Target.position);
        }
        else
        {
            Quaternion NewRot = Quaternion.LookRotation(DirToTarget, Vector3.up);
            NewRot.x = 0;
            NewRot.z = 0;

            transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, Time.deltaTime * 2);
        }

        FireTime -= Time.deltaTime;

        if(FireTime <= 0)
        {
            bool TargetHit = Physics.Raycast(transform.position, transform.forward, out RaycastHit Hitinfo);

            if(TargetHit && Hitinfo.transform == Target)
            {
                if (ProjectilePrefab)
                {
                    ProjectileScript PS = Instantiate(ProjectilePrefab, transform.position, transform.rotation).GetComponent<ProjectileScript>();
                    PS.Damage = Damage;
                }
                else if(TargetDistance <= MeleeRange)
                {
                    Target.GetComponent<PlayerController>().TakeDamage(Damage);
                }
            }

            FireTime = Random.Range(FireRateMin, FireRateMax);
        }
    }
    void RunFromTarget()
    {
        Vector3 AwayDirection = (transform.position - Target.position).normalized;
        Debug.DrawRay(transform.position, AwayDirection, Color.green);
        Agent.SetDestination(transform.position + AwayDirection * 4);

        StateTimer += Time.deltaTime;
        if(StateTimer >= RunTime)
        {
            CurrentState = AIState.Roaming;
        }
    }

    void SpriteChange()
    {
        Vector3 DirToTarget = (transform.position - Target.position).normalized;
        DirToTarget *= -1;

        float Angle = Vector3.SignedAngle(transform.forward, DirToTarget, transform.up);
        Angle = Mathf.Abs(Angle);

        int Active = -1;

        if(Angle <= 45)
        {
            Active = 0;
        }
        else if(Angle <= 135)
        {
            Active = 1;
        }
        else
        {
            Active = 2;
        }

        for (int i = 0; i < Sprites.Length; i++)
        {
            if(i == Active)
            {
                Sprites[i].SetActive(true);
            }
            else
            {
                Sprites[i].SetActive(false);
            }

            if(DamageTick > 0)
            {
                Sprites[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                Sprites[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if(DamageTick > 0)
        {
            DamageTick -= Time.deltaTime;
        }
    }

    public enum AIState
    {
        Roaming,
        Chasing,
        Running,
        Idle
    }
    public enum AIType
    {
        Passive,
        Agressive,
        NoMove,
        Dummy
    }
}
