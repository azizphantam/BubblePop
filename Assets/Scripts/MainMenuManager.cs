using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class MainMenuManager : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Image LoadingBar;
    public void GamePlayScene()
    {
        LoadingPanel.SetActive(true);
        LoadingBar.transform.DOScale(1, 1.5f);
        Invoke(nameof(loadScene), 1.4f);
    }
    public void loadScene()
    {
        SceneManager.LoadScene(2);
    }
}
