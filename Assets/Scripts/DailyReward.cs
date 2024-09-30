using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class DailyReward : MonoBehaviour
{
    public int[] dailyRewards = { 100, 200, 300, 400, 500, 600, 700 }; // Array of daily reward amounts for 7 days
    public string lastRewardDateKey = "LastRewardDate"; // Key to store last reward date in PlayerPrefs
    public string consecutiveDaysKey = "ConsecutiveDays"; // Key to store consecutive login days in PlayerPrefs
                                                          //  public TMP_Text rewardTest;
    public Button getreward_btns;
    private int current;
    public List<Sprite> RewardImage; // reward panel images
   
   
  
    private void Start()
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

            getreward_btns.interactable = true;
        }
        else
        {
            getreward_btns.interactable = false;
        }
      
    }

    void GrantDailyReward(int consecutiveDays)
    {
       
       
      
      

        switch (consecutiveDays)
        {


            case 1:
               
                Debug.Log(" 100 Dollars ");
                getreward_btns.interactable = false;

                break;
            case 2:

                PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives") + 1);
                Debug.Log(" 1 Live ");
                getreward_btns.interactable = false;
              
              
                
                break;
            case 3:
                PlayerPrefs.SetInt("Dollars",
                   PlayerPrefs.GetInt("Dollars") + 150);
                Debug.Log(" 150 Dollars ");
                getreward_btns.interactable = false;
               
                
               
                break;
            case 4:
                PlayerPrefs.SetInt("Dollars",
                   PlayerPrefs.GetInt("Dollars") + 180);
                PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives") + 1);
                Debug.Log(" 180 Dollars : 01 Live");
                getreward_btns.interactable = false;
              
              
              
                break;
            case 5:
                PlayerPrefs.SetInt("Dollars",
                   PlayerPrefs.GetInt("Dollars") + 1000);

                Debug.Log(" 1000 Dollars ");
                getreward_btns.interactable = false;
               
              
               
                break;
            case 6:
                PlayerPrefs.SetInt("Dollars",
                   PlayerPrefs.GetInt("Dollars") + 1250);

                Debug.Log(" 1250 Dollars ");
                getreward_btns.interactable = false;
              
                break;
            case 7:

                PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives") + 3);
                Debug.Log(" 3 Live ");
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
