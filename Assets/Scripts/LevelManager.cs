using DG.Tweening;
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
    public Text Popitnow;
    public GameObject MoveUpPanel;
    public GameObject TopOutofMovesImg;
    public Text MovestimerText; // Assign in Inspector
    private float timerDuration = 6f;
    public Coroutine timerCoroutine; // Store reference to the coroutine
    public List<GameObject> Boosters;
    public List<GameObject> BoosterPanel;
    public GameObject HandTutorialsFirst;
    public Text winpanel_coinsAMount;

    public GameObject ScrollerImage;
    int currentscrollerindex;
    public List<int> RewardAmounts;
    public Button rewardbtn;
    bool isclimaedwoncoins = true;
    int coins;
    public void MoveSCroller()
    {
        if (currentscrollerindex < 4)
        {
            currentscrollerindex++;
        }
        else
        {
            currentscrollerindex = 0;
        }

       
       
    }
    public void ClaimedReward()
    {
        SoundsManager.instance.PlayButtonClipSound(SoundsManager.instance.AS);
        Debug.Log("reward "+ RewardAmounts[currentscrollerindex]);
        ScrollerImage.GetComponent<DOTweenAnimation>().DOPause();
        AdsManager.Instance.ShowRewardAd(WatchRewardAd);
        CancelInvoke("MoveSCroller");

        coins = RewardAmounts[currentscrollerindex];
    }
    public void WatchRewardAd(string status)
    {
       
        isclimaedwoncoins = false;
        GameManager.gm.IncreaseCoins(coins);
        winpanel_coinsAMount.text = PlayerPrefs.GetInt("Coins").ToString();
        rewardbtn.interactable = false;

    }
    private void Start()
    {
        wrongballs.text = "0 "+WrongsBalls.ToString();
        
        if (levelManagerInstance == null)
        {
            levelManagerInstance = this;
        }
        if (PlayerPrefs.GetInt("CurrentLevel") ==0 && PlayerPrefs.GetInt("AllLevelsDone")==0)
        {
            foreach (var item in Boosters)
            {
                item.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("CurrentLevel") > 1 && PlayerPrefs.GetInt("AllLevelsDone") == 0 )
        {
            Boosters[0].SetActive(true);
        }  
        if (PlayerPrefs.GetInt("CurrentLevel") > 4 && PlayerPrefs.GetInt("AllLevelsDone") == 0 )
        {
            Boosters[1].SetActive(true);
        }
        if (PlayerPrefs.GetInt("CurrentLevel") ==1 && PlayerPrefs.GetInt("AllLevelsDone") == 0 && CustomPlayLevel.instance.isSelectCustomLevel == false)
        {
            GameManager.gm.IsClickAble = false;
            BoosterPanel[0].SetActive(true);
            PlayerPrefs.SetInt("Swap", 1);
        }
        if (PlayerPrefs.GetInt("CurrentLevel") == 4 && PlayerPrefs.GetInt("AllLevelsDone") == 0 && CustomPlayLevel.instance.isSelectCustomLevel == false)
        {
            GameManager.gm.IsClickAble = false;
            BoosterPanel[1].SetActive(true);
            PlayerPrefs.SetInt("Hint", 1);
        }
    }

   
   

   





    public void Home()
    {
        Time.timeScale = 1; 
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
        Popitnow.transform.DOScale(0,.5f).SetEase(Ease.OutBounce);
    }
    public void WinGame()
    {
      
        WonPAnel.SetActive(true);
        winpanel_coinsAMount.text = PlayerPrefs.GetInt("Coins").ToString();
        InvokeRepeating(nameof(MoveSCroller), .5f, .5f);
        SoundsManager.instance.PlayLevelWinSound(SoundsManager.instance.AS);
        if (isclimaedwoncoins)
        {
            GameManager.gm.IncreaseCoins(35);
            winpanel_coinsAMount.text = PlayerPrefs.GetInt("Coins").ToString();
        }
       
       
        if (CustomPlayLevel.instance.isSelectCustomLevel != true)
        {
            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
        }
        
    }
    public void DecrementWrong()
    {
       
        wrongballs.text = "0 " + WrongsBalls.ToString();
    }
    public void TopInfoOutOfMoves()
    {
        TopOutofMovesImg.gameObject.SetActive(true);
       
        //StartCoroutine(StartTimer());
        timerCoroutine = StartCoroutine(StartTimer());
    }
    IEnumerator StartTimer()
    {
       
        float currentTime = timerDuration;

        while (currentTime > 0)
        {
            // Update the UI Text to show the timer
            MovestimerText.text = currentTime.ToString("F0")+"s"; // Show 1 decimal place

            // Wait for 1 second
            yield return new WaitForSeconds(1f);

            // Reduce the time
            currentTime -= 1f;
        }

        // Once the timer reaches 0, stop the countdown
        MovestimerText.text = "0s";
        TopOutofMovesImg.SetActive(false);
       
        MoveUpPanel.transform.DOScale(1, 1).SetEase(Ease.OutBounce);
    }
}
