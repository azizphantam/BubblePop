using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplaySoundsSystem : MonoBehaviour
{
    public GameObject SoundImg, Haptics, bg;

    private void Start()
    {
        checkSoundPanelSetting();
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
        BtnClickSound();
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
        BtnClickSound();
    }

    public void ClickBackground()
    {
        if (PlayerPrefs.GetInt("bg") == 0)
        {
            PlayerPrefs.SetInt("bg", 1);
            bg.SetActive(true);
            SoundsManager.instance.bg.gameObject.SetActive(false);

        }
        else
        {
            PlayerPrefs.SetInt("bg", 0);
            bg.SetActive(false);
            SoundsManager.instance.bg.gameObject.SetActive(true);
        }
        BtnClickSound();
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
            SoundsManager.instance.bg.gameObject.SetActive(false);
            SoundsManager.instance.bg.Play();
        }
        else
        {
            SoundsManager.instance.bg.gameObject.SetActive(false);
            bg.SetActive(true);
        }
    }
}
