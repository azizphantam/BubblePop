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
        loadingimage.transform.DOScale(1, 1.5f);
        Invoke(nameof(loadScene),1.4f);
    }
    public void loadScene()
    {
        SceneManager.LoadScene(1);
    }
}
