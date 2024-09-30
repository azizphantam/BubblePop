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
    public GameObject LoadingPanel;
    private void Start()
    {
        wrongballs.text = "0 "+WrongsBalls.ToString();
        if (levelManagerInstance == null)
        {
            levelManagerInstance = this;
        }
    }
    public void Home()
    {
        LoadingPanel.SetActive(true);
        Invoke(nameof(SceneLoading),1.4f);
      
    }
    public void SceneLoading()
    {
      
        SceneManager.LoadScene(1);
    }
    public void WonLevel()
    {
        Invoke(nameof(WinGame), 1);

    }
    public void WinGame()
    {
      
        WonPAnel.SetActive(true);
        SoundsManager.instance.PlayLevelWinSound(SoundsManager.instance.AS);
        GameManager.gm.IncreaseCoins(35);
        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
    }
    public void DecrementWrong()
    {
       
        wrongballs.text = "0 " + WrongsBalls.ToString();
    }
   
}
