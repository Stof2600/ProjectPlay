using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject ScorePrefab;
    public GameObject SettingsMenu;
    public Text ScoreText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(!FindObjectOfType<ScoreManager>())
        {
            Instantiate(ScorePrefab);
        }
        else
        {
            FindObjectOfType<ScoreManager>().InLevel = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerPrefs.DeleteAll();
            FindFirstObjectByType<ScoreManager>().SaveScore();
            FindFirstObjectByType<ScoreManager>().LoadScore();
        }
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            FindFirstObjectByType<ScoreManager>().SaveScore();
            FindFirstObjectByType<ScoreManager>().LoadScore();
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            FindFirstObjectByType<ScoreManager>().AddScore(5);
        }

        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        ScoreManager SM = FindObjectOfType<ScoreManager>();

        string LoadedText = "HIGH SCORES\n";

        for (int i = 0; i < SM.LastScores.Length; i++)
        {
            string ScoreTextRow = "S" +(i + 1) .ToString()+ "= " + SM.LastScores[i].ToString() + "\n";
            LoadedText += ScoreTextRow;
        }

        ScoreText.text = LoadedText;
    }

    public void StartGame()
    {
        FindObjectOfType<ScoreManager>().InLevel = true;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        if (FindObjectOfType<ScoreManager>())
        {
            FindObjectOfType<ScoreManager>().SaveScore();
        }

        Application.Quit();
        print("ClosedGame");
    }

    public void OpenSettings()
    {
        if(SettingsMenu.activeSelf)
        {
            SettingsMenu.SetActive(false);
        }
        else
        {
            SettingsMenu.SetActive(true);
        }
    }
}
