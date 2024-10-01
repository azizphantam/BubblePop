using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : MonoBehaviour
{
    public List<Button> lvlbtns;
    public List<GameObject> playbtn;
    public List<GameObject> LockImg;

    
    private void OnEnable()
    {
        for (int i = 0; i < lvlbtns.Count; i++)
        {
            if (playbtn[i] == null)
                playbtn[i] = lvlbtns[i].transform.GetChild(3).gameObject;
            if (LockImg[i] == null)
                LockImg[i] = lvlbtns[i].transform.GetChild(1).gameObject;

            if (i < PlayerPrefs.GetInt("CurrentLevel"))
            {
                lvlbtns[i].interactable = true;
                playbtn[i].SetActive(true);
                LockImg[i].SetActive(false);
            }
            else
            {
                lvlbtns[i].interactable=false;
                playbtn[i].SetActive(false);
                LockImg[i].SetActive(true);
            }

            if (PlayerPrefs.GetInt("AllLevelsDone") == 1)
            {
                lvlbtns[i].interactable = true;
                playbtn[i].SetActive(true);
                LockImg[i].SetActive(false);
            }
        }
    }
}
