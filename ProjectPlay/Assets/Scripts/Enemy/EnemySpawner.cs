using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs = new List<GameObject>();
    public List<GameObject> SpawnedEnemies = new List<GameObject>();
    public LayerMask GroundLayer;

    public int EnemyCount = 50;
    public Vector2 SpawnLimits;

    public GameObject SpawnerPrefab;

    int Limiter;

    void Start()
    {
        Limiter = 0;

        EnemyCount = EnemyCount + (1 * FindObjectOfType<ScoreManager>().CurrentLevel);

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        while(SpawnedEnemies.Count < EnemyCount && Limiter < 500)
        {
            Limiter += 1;

            Vector3 SpawnPos = transform.position +  new Vector3(Random.Range(-SpawnLimits.x, SpawnLimits.x), 0, Random.Range(-SpawnLimits.y, SpawnLimits.y));
            Vector3 Output = GroundCheck(SpawnPos);

            if (Output != Vector3.zero)
            {
                int R = Random.Range(0, EnemyPrefabs.Count);
                GameObject NewEnemy = Instantiate(EnemyPrefabs[R], Output, transform.rotation, GameObject.FindGameObjectWithTag("EnemyHolder").transform);
                SpawnedEnemies.Add(NewEnemy);
            }
        }

        if(!CheckQouta())
        {
            EnemySpawner ES = Instantiate(SpawnerPrefab, transform.position, transform.rotation).GetComponent<EnemySpawner>();
            ES.SpawnedEnemies.AddRange(SpawnedEnemies);
        }
    }
    Vector3 GroundCheck(Vector3 Pos)
    {
        bool Hitground = Physics.Raycast(Pos, Vector3.down, out RaycastHit GroundInfo, 300f, GroundLayer);
        
        if(Hitground)
        {
            if(GroundInfo.transform.CompareTag("SafeZone"))
            {
                print("HitSafeZone");
                return Vector3.zero;
            }

            return GroundInfo.point;
        }

        return Vector3.zero;
    }

    bool CheckQouta()
    {
        int ScoreCount = 0;

        for (int i = 0; i < SpawnedEnemies.Count; i++)
        {
            ScoreCount += SpawnedEnemies[i].GetComponent<EnemyController>().DeathScore;
        }

        if(ScoreCount >= FindObjectOfType<ScoreManager>().LevelQouta)
        {
            return true;
        }

        return false;
    }
}
