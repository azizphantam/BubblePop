using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
public class MainMenuManager : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Image LoadingBar;
    public List<GameObject> IconsList;
    public List<GameObject> PanelList;
    public static MainMenuManager Instance;
    int icondefault = 0;
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

        if (CustomPlayLevel.instance.isSelectCustomLevel == true)
        {
            PanelList[1].transform.DOLocalMoveX(1120f, 0);
            PanelList[1].transform.DOLocalMoveX(0, .5f).SetEase(Ease.Linear);
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
        
    }
    public void BtnClickSound() 
    {
        SoundsManager.instance.PlayButtonClipSound(SoundsManager.instance.AS);
        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
    }
}
