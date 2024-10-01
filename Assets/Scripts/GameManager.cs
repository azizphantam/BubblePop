using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<GameObject> Levels = new List<GameObject>();
    public float ScrewPosVal;

    [Header("Timer Working")]
    private float totalTimeInSeconds; // Set the total time (e.g., 120 seconds = 2 minutes)
    private float remainingTime;
    public bool IsTimerRunning = true;
    public Text timerText; // UI Text to display the time. (Optional if using UI)

    public GameObject TimeOutPanel;
    public GameObject FailedPanel;
    public Text Coins;
    public Text levelnumber;

    public Image CurrentLvlImgbg;
    public Image CurrentLvlImg;
    public float progressionamount;
    public BallMove CurrentLevel;
    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }
    private void Start()
    {
        if (CustomPlayLevel.instance.isSelectCustomLevel == true)
        {
            foreach (GameObject level in Levels)
            {
                level.SetActive(false);
            }

            Levels[CustomPlayLevel.levelnumber].SetActive(true);
            totalTimeInSeconds = Levels[CustomPlayLevel.levelnumber].GetComponent<BallMove>().Time;
            CurrentLvlImgbg.sprite = Levels[CustomPlayLevel.levelnumber].GetComponent<BallMove>().lvlbg;
            CurrentLvlImg.sprite = Levels[CustomPlayLevel.levelnumber].GetComponent<BallMove>().lvlimg;
            remainingTime = totalTimeInSeconds; // Initialize remaining time
            CurrentLevel = Levels[CustomPlayLevel.levelnumber].GetComponent<BallMove>();
            levelnumber.text = "Level " + (CustomPlayLevel.levelnumber + 1).ToString("00");
        }
        else
        {
            LoadNextLevel();

            remainingTime = totalTimeInSeconds; // Initialize remaining time

            levelnumber.text = "Level " + (PlayerPrefs.GetInt("CurrentLevel") + 1).ToString("00");
            CurrentLevel = Levels[PlayerPrefs.GetInt("CurrentLevel")].GetComponent<BallMove>();
        }



    }
    public void IncreaseProgression()
    {
        CurrentLvlImg.fillAmount = CurrentLvlImg.fillAmount + progressionamount;
    }
    public void LoadNextLevel()
    {
        foreach (GameObject level in Levels)
        {
            level.SetActive(false);
        }
        if (PlayerPrefs.GetInt("CurrentLevel") < Levels.Count)
        {
            Levels[PlayerPrefs.GetInt("CurrentLevel")].SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
            PlayerPrefs.SetInt("AllLevelsDone", 1);
            Levels[PlayerPrefs.GetInt("CurrentLevel")].SetActive(true);
            
        }
        totalTimeInSeconds = Levels[PlayerPrefs.GetInt("CurrentLevel")].GetComponent<BallMove>().Time;
        CurrentLvlImgbg.sprite = Levels[PlayerPrefs.GetInt("CurrentLevel")].GetComponent<BallMove>().lvlbg;
        CurrentLvlImg.sprite = Levels[PlayerPrefs.GetInt("CurrentLevel")].GetComponent<BallMove>().lvlimg;
    }
    public void ReplayLevel()
    {
        LevelManager.levelManagerInstance.LoadingPanel.SetActive(true);
        Invoke(nameof(SceneLoading), 1.4f);

    }
    public void SceneLoading()
    {

        SceneManager.LoadScene(2);
    }
    public void SKipLevel()
    {
        LevelManager.levelManagerInstance.LoadingPanel.SetActive(true);
       // StopCoroutine(LevelManager.levelManagerInstance.timerCoroutine);
        Invoke(nameof(SkipLevelLoading), 1.4f);
    }
    public void SkipLevelLoading()
    {
        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if (remainingTime > 0 && IsTimerRunning)
        {

            // Update time every second
            remainingTime -= Time.deltaTime;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            // Display the time in the format "mm:ss"
            string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);


            // If you're using a UI Text component, update it
            if (timerText != null)
            {
                timerText.text = timeFormatted;
            }

            // Wait for the next frame before updating the time again
            if (remainingTime <= 10)
            {
                SoundsManager.instance.PlayCountDown(SoundsManager.instance.AS);
            }


        }
        else if (IsTimerRunning == true && remainingTime <= 0)
        {
            // Once time reaches zero, you can handle what happens next (e.g., Game Over, etc.)

            OpenPanel(TimeOutPanel);
            SoundsManager.instance.PlayLevelFailSound(SoundsManager.instance.AS);
            IsTimerRunning = false;
            string timeFormatted = string.Format("{0:00}:{1:00}", 00, 00);
            timerText.text = timeFormatted;
            Coins.text = PlayerPrefs.GetInt("Coins").ToString();
        }


    }
    // Optional function if you want to access remaining time as minutes and seconds
    public (int, int) GetMinutesAndSeconds()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        return (minutes, seconds);
    }

    public void OpenPanel(GameObject pn)
    {
        pn.gameObject.SetActive(true);
        pn.transform.GetChild(0).DOScale(0, 0);
        pn.transform.GetChild(0).DOScale(1, 0.5f).SetEase(Ease.Linear);
    }
    public void ClosePanel(GameObject pn)
    {
        pn.gameObject.SetActive(false);
        pn.transform.GetChild(0).DOScale(0, 0.5f).SetEase(Ease.Linear);
    }
    public void ReviveWithCoins()
    {
        if (PlayerPrefs.GetInt("Coins") >= 350)
        {
            ClosePanel(TimeOutPanel);
            decreaseCoins(350);
            remainingTime = 60;
            IsTimerRunning = true;
        }
        else
        {
            OpenPanel(FailedPanel);
        }

    }
    public void IncreaseCoins(int coinamount)
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coinamount);
        Coins.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    public void decreaseCoins(int coinamount)
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - coinamount);
        Coins.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    public void BtnClickSound()
    {
        SoundsManager.instance.PlayButtonClipSound(SoundsManager.instance.AS);
    }
    public void ErrorHaptics()
    {
        MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
    }


    public void CallBoostSwapping()
    {
        CurrentLevel.SwapNuts();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

}
