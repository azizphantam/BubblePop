using System.Linq;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    public AudioClip levelWinSound;
    public AudioClip levelFailSound;
    public AudioClip buttonClipSound;
    public AudioClip GameBackGroundMusic;
    public AudioClip CountDownSound;
   
   
    public AudioSource AS;
    public AudioSource bg;
  
  
    public float volume = 0.5f;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        PlayGameBuilding_Bg(bg);
    }
    public void ButtonClick()
    {
        AS.PlayOneShot(buttonClipSound);
    }
  
  
    private static void PlaySound(AudioClip a, AudioSource @as)
    {

        @as.clip = a;
      //  @as.loop = false;
        @as.Play();
    }
    public void PlayLevelWinSound(AudioSource @as)
    {
        if (@as == null)
            return;
        @as.volume = 1f;
        PlaySound(levelWinSound, @as);
    }
    public void StopMusic()
    {
        bg.enabled = false;
    }
    public void StartMusic()
    {
        bg.enabled = true;
    }
    public void PlayLevelFailSound(AudioSource @as)
    {
        if (@as == null)
            return;
        @as.volume = 1f;
        PlaySound(levelFailSound, @as);
    }
   
  

    public void PlayButtonClipSound(AudioSource @as)
    {
        ////if (@as == null)
        ////    return;
        @as.volume = 1f;
        PlaySound(buttonClipSound, @as);
    }


    public void PlayCountDown(AudioSource @as)
    {
        ////if (@as == null)
        ////    return;
        @as.volume = 1f;
        PlaySound(CountDownSound, @as);
    }







    public void PlayGameBuilding_Bg(AudioSource @as)
    {
        if (@as == null)
            return;
       
            @as.volume = 1f;
            PlaySound(GameBackGroundMusic, @as);
       
           
    }
  
}
