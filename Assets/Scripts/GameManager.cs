using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<GameObject> Levels = new List<GameObject>();
    public float ScrewPosVal;
    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }
    private void Start()
    {
        LoadNextLevel();
    }
    public void LoadNextLevel()
    {
        foreach (GameObject level in Levels)
        {
            level.SetActive(false);
        }
        if(PlayerPrefs.GetInt("CurrentLevel")< Levels.Count)
        {
            Levels[PlayerPrefs.GetInt("CurrentLevel")].SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
            Levels[PlayerPrefs.GetInt("CurrentLevel")].SetActive(true);
        }
       
    }
    public void replaylevel()
    {
        SceneManager.LoadScene(0);
    }
}
