using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Unity.VisualScripting;
public class MainMenuManager : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Image LoadingBar;
    public List<GameObject> IconsList;
    public List<GameObject> PanelList;
    public static MainMenuManager Instance;
    int icondefault = 0;

    public List<GameObject> LevelsScrollerBlackImages;
    public GameObject levelScrollerObj;
    public Text Coinsmenu;
    public Text levelcoins;
    public GameObject SoundImg,Haptics,bg;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            

            return;
        }

    }
    private void Start()
    {
        checkSoundPanelSetting();
        levelScrollerObj.transform.DOMoveY(levelScrollerObj.transform.position.y - ( 400 *  PlayerPrefs.GetInt("CurrentLevel") ), 1);
        Coinsmenu.text = PlayerPrefs.GetInt("Coins").ToString();
        if (CustomPlayLevel.instance.isSelectCustomLevel == true)
        {
            PanelList[1].transform.DOLocalMoveX(1120f, 0);
            PanelList[1].transform.DOLocalMoveX(0, .5f).SetEase(Ease.Linear);
        }
        for (int i = 0; i < LevelsScrollerBlackImages.Count; i++)
        {
            if(i< PlayerPrefs.GetInt("CurrentLevel"))
            {
                LevelsScrollerBlackImages[i].SetActive(false);
            }
            else
            {
                LevelsScrollerBlackImages[i].SetActive(true);
            }
        }
    }
    public void GamePlayScene()
    {
        LoadingPanel.SetActive(true);
        LoadingBar.transform.DOScale(1, 1.5f);
        Invoke(nameof(loadScene), 1.4f);
    }
    public void loadScene()
    {
        LoadingPanel.transform.GetChild(0).DOMoveX(1288, 0.5f);
        SceneManager.LoadScene(2);
    }
    public void SelectIcon(int selectedicon)
    {
        if (icondefault != selectedicon)
        {
            IconsList[icondefault].transform.DOScale(0, .5f).SetEase(Ease.Linear);
            icondefault = selectedicon;
            IconsList[icondefault].transform.DOScale(1, .5f).SetEase(Ease.Linear);
        }
        
    }
    public void Home()
    {
        CustomPlayLevel.instance.isSelectCustomLevel = false;
        PanelList[1].transform.DOLocalMoveX(-1120f, .5f).SetEase(Ease.Linear);
    }
    public void Levels()
    {
        CustomPlayLevel.instance.isSelectCustomLevel = true;
        PanelList[1].transform.DOLocalMoveX(1120f, 0);
        PanelList[1].transform.DOLocalMoveX(0, .5f).SetEase(Ease.Linear);
        levelcoins.text = PlayerPrefs.GetInt("Coins").ToString();
    }
    public void BtnClickSound() 
    {

        SoundsManager.instance.PlayButtonClipSound(SoundsManager.instance.AS);
        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
    }


    public void Clicksound()
    {
        if (PlayerPrefs.GetInt("Sounds") == 0)
        {
            PlayerPrefs.SetInt("Sounds", 1);
            SoundImg.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("Sounds", 0);
            SoundImg.SetActive(false);
        }
    }
    public void ClickHaptics()
    {
        if (PlayerPrefs.GetInt("Haptics") == 0)
        {
            PlayerPrefs.SetInt("Haptics", 1);
            Haptics.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("Haptics", 0);
            Haptics.SetActive(false);
        }
    }

    public void ClickBackground()
    {
        if (PlayerPrefs.GetInt("bg") == 0)
        {
            PlayerPrefs.SetInt("bg", 1);
            bg.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("bg", 0);
            bg.SetActive(false);
        }
    }




    public void checkSoundPanelSetting()
    {
        if (PlayerPrefs.GetInt("Sounds") == 0)
        {
           
            SoundImg.SetActive(false);
        }
        else
        {
           
            SoundImg.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Haptics") == 0)
        {
           
            Haptics.SetActive(false);
        }
        else
        {
           
            Haptics.SetActive(true);
        }

        if (PlayerPrefs.GetInt("bg") == 0)
        {
           
            bg.SetActive(false);
        }
        else
        {
           
            bg.SetActive(true);
        }
    }
}
