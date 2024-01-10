using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public int TotalScore;

    public int LevelQouta;
    public int CurrentLevel;

    public bool InfTime, InLevel;

    public float StartLevelTime = 301;
    public float LevelTimer;

    public int TotalSaves = 5;

    public int[] LastScores;

    public Vector3 StartPos;
    public Vector3 StartRot;
    Vector3 PlayerPos;
    Quaternion PlayerRot;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LastScores = new int[TotalSaves];

        PlayerPos = StartPos;
        PlayerRot = Quaternion.Euler(StartRot.x, StartRot.y, StartRot.z);
        LevelTimer = StartLevelTime;

        LevelQouta = 300;

        LoadScore();
    }

    private void Update()
    {
        if(InLevel)
        {
            LevelTimer -= Time.deltaTime;

            if(LevelTimer <= 0 || Input.GetKeyDown(KeyCode.LeftShift))
            {
                EndCheck();
            }
        }
        else if(!InLevel && TotalScore <= -1)
        {
            LevelTimer = StartLevelTime;
        }
    }

    public void AddScore(int Amount)
    {
        TotalScore += Amount;
    }

    public void SaveScore()
    {
        for (int i = 0; i < LastScores.Length; i++)
        {
            if (LastScores[i] < TotalScore && (LastScores[i] == 0 || i == LastScores.Length))
            {
                string ScoreKey = "(EM)ScoreKey" + i.ToString();
                print(ScoreKey);
                PlayerPrefs.SetInt(ScoreKey, TotalScore);
                PlayerPrefs.Save();
                return;
            }
        }
    }

    public void LoadScore()
    {
        for (int i = 0; i < TotalSaves; i++)
        {
            string ScoreKey = "(EM)ScoreKey" + i.ToString();
            LastScores[i] = PlayerPrefs.GetInt(ScoreKey);
        }

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < LastScores.Length - 1; i++)
            {
                int S1 = LastScores[i];
                int S2 = LastScores[i + 1];

                if (S1 < S2)
                {
                    LastScores[i] = S2;
                    LastScores[i + 1] = S1;
                }
            }
        }
    }

    void EndCheck()
    {
        if(TotalScore >= LevelQouta)
        {
            CurrentLevel += 1;
            LevelTimer = StartLevelTime;
            LevelQouta = (int)((float)LevelQouta * 1.3f);

            SaveScore();
            LoadScore();

            TotalScore = 0;

            SceneManager.LoadScene(0);
        }
        else
        {
            FindObjectOfType<PlayerController>().KillPlayer("DIDN'T REACH QOUTA");

            SaveScore();
            LoadScore();
        }
    }

    public void UpdatePlayerPos(Transform Target)
    {
        PlayerPos = Target.position;
        PlayerRot = Target.rotation;
    }

    public void ResetPlayerPos()
    {
        PlayerPos = StartPos;
        PlayerRot = Quaternion.Euler(0, 0, 0);
    }

    public void LoadPlayerPos(Transform Target)
    {
        Target.position = PlayerPos;
        Target.rotation = PlayerRot;
    }
}
