using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
public class DailyReward : MonoBehaviour
{
    public int[] dailyRewards = { 100, 150, 200, 250, 300, 350, 2 }; // Array of daily reward amounts for 7 days
    public string lastRewardDateKey = "LastRewardDate"; // Key to store last reward date in PlayerPrefs
    public string consecutiveDaysKey = "ConsecutiveDays"; // Key to store consecutive login days in PlayerPrefs
                                                          //  public TMP_Text rewardTest;
    public Button getreward_btns;
    private int current;
    public List<GameObject> RewardImage; // reward panel images
    public List<GameObject> TickImages; // reward panel images
   
   
  
    private void Start()
    {
        
        DateTime currentDate = DateTime.Today;
        DateTime lastRewardDate;
        int consecutiveDays = PlayerPrefs.GetInt(consecutiveDaysKey, 0) -1;
        for (int i = 0; i < dailyRewards.Length; i++)
        {
            if(i<= consecutiveDays)
            {
                RewardImage[i].SetActive(true);
                TickImages[i].SetActive(true);
            }
            else
            {
                RewardImage[i].SetActive(false);
                TickImages[i].SetActive(false);
            }
        }
        if (PlayerPrefs.HasKey(lastRewardDateKey))
        {
            lastRewardDate = DateTime.Parse(PlayerPrefs.GetString(lastRewardDateKey));
        }
        else
        {
            lastRewardDate = currentDate.AddDays(-1); // Set to yesterday if no last reward date exists
        }

        if (currentDate.Date > lastRewardDate.Date)
        {

            getreward_btns.interactable = true;
        }
        else
        {
            getreward_btns.interactable = false;
        }
      
    }
    private void OnDisable()
    {
        MainMenuManager.Instance.Coinsmenu.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    void GrantDailyReward(int consecutiveDays)
    {
       
       
      
      

        switch (consecutiveDays)
        {


            case 1:
               
               
                getreward_btns.interactable = false;
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 100);
                break;
            case 2:

                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 150);
                getreward_btns.interactable = false;
              
              
                
                break;
            case 3:
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 200);
                getreward_btns.interactable = false;
               
                
               
                break;
            case 4:
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 250);
                getreward_btns.interactable = false;
              
              
              
                break;
            case 5:
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 300);
                getreward_btns.interactable = false;
               
              
               
                break;
            case 6:
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 350);
                getreward_btns.interactable = false;
              
                break;
            case 7:

                PlayerPrefs.SetInt("Hints", PlayerPrefs.GetInt("Hints") + 2);
                getreward_btns.interactable = false;
                
               
                
                break;

        }



        // rewardTest.text = "You've earned a daily reward of " + rewardAmount + " coins for logging in " +
        //consecutiveDays + " consecutive days!";
    }
   

    public void ClaimReward()
    {
        DateTime currentDate = DateTime.Today;
        DateTime lastRewardDate;
       
        if (PlayerPrefs.HasKey(lastRewardDateKey))
        {
            lastRewardDate = DateTime.Parse(PlayerPrefs.GetString(lastRewardDateKey));
        }
        else
        {
            lastRewardDate = currentDate.AddDays(-1); // Set to yesterday if no last reward date exists
        }

        if (currentDate.Date > lastRewardDate.Date)
        {
            // Player logged in after last reward date
            if (lastRewardDate.Date.AddDays(1) == currentDate.Date)
            {
                // Player logged in consecutively
                int consecutiveDays = PlayerPrefs.GetInt(consecutiveDaysKey, 0) + 1;

                if (consecutiveDays > dailyRewards.Length)
                {
                    consecutiveDays = 1; // Reset consecutive days if more than 7 days have passed
                }

                // Grant reward for the current day

                GrantDailyReward(consecutiveDays);

                RewardImage[consecutiveDays-1].SetActive(true);
                TickImages[consecutiveDays-1].SetActive(true);
                // getreward_btns[PlayerPrefs.GetInt("ConsecutiveDays")].interactable = true;
                PlayerPrefs.SetInt(consecutiveDaysKey, consecutiveDays);
            }
            else
            {
                // Player logged in after missing a day
                GrantDailyReward(1);
                PlayerPrefs.SetInt(consecutiveDaysKey, 1);
            }

            // Save today's date as the last reward date
            PlayerPrefs.SetString(lastRewardDateKey, currentDate.ToString("yyyy-MM-dd"));
        }
        else
        {
            // Player already claimed the reward for today
            Debug.Log("You've already claimed your reward for today.");
            // rewardTest.text = "You've already claimed your reward for today.";
        }
    }

}
