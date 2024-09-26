using System.Collections;
using System.Collections.Generic;
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

    public Text timerText; // UI Text to display the time. (Optional if using UI)
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

        remainingTime = totalTimeInSeconds; // Initialize remaining time
        StartCoroutine(Countdown());

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
        totalTimeInSeconds = Levels[PlayerPrefs.GetInt("CurrentLevel")].GetComponent<BallMove>().Time;
    }
    public void replaylevel()
    {
        SceneManager.LoadScene(2);
    }

    IEnumerator Countdown()
    {
        while (remainingTime > 0)
        {
            // Update time every second
            remainingTime -= Time.deltaTime;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            // Display the time in the format "mm:ss"
            string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);
            Debug.Log(timeFormatted); // Print to console

            // If you're using a UI Text component, update it
            if (timerText != null)
            {
                timerText.text = timeFormatted;
            }

            // Wait for the next frame before updating the time again
            yield return null;
        }

        // Once time reaches zero, you can handle what happens next (e.g., Game Over, etc.)
        Debug.Log("Time's up!");
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

    }
}
