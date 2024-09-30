using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomPlayLevel : MonoBehaviour
{
    public static  int levelnumber;
    public bool isSelectCustomLevel = false;
    public static CustomPlayLevel instance;
   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }

        //Destroy(gameObject);

    }
   
    public void SelectLevel(int level)
    {
        levelnumber = level;
        Debug.Log("levelnumber"+levelnumber);
        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
        SoundsManager.instance.PlayButtonClipSound(SoundsManager.instance.AS);
        MainMenuManager.Instance.GamePlayScene();
        
    }
}
