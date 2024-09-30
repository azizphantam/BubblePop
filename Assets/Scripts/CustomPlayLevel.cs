using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomPlayLevel : MonoBehaviour
{
    public  int levelnumber;
    public bool isSelectCustomLEvel = false;
    public static CustomPlayLevel instance;
   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }

    }
   
    public void SelectLevel(int level)
    {
        levelnumber = level;
       

        MainMenuManager.Instance.GamePlayScene();
    }
}
