using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Splash : MonoBehaviour
{
    public Image loadingimage;
    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            //PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 30);
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.SetInt("Swap", 1);
            PlayerPrefs.SetInt("Hint", 1);
        }
        loadingimage.transform.DOScale(1, 1.8f);
        Invoke(nameof(loadScene),1.7f);
    }
    public void loadScene()
    {
        SceneManager.LoadScene(1);
    }
}
