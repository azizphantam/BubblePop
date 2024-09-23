using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManagerInstance;
    public int WrongsBalls;
    public GameObject WonPAnel;
    public Text wrongballs;
    private void Start()
    {
        wrongballs.text = "0 "+WrongsBalls.ToString();
        if (levelManagerInstance == null)
        {
            levelManagerInstance = this;
        }
    }
    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
    public void WonLevel()
    {
        WonPAnel.SetActive(true);
        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
    }
    public void DecrementWrong()
    {
       
        wrongballs.text = "0 " + WrongsBalls.ToString();
    }
}
