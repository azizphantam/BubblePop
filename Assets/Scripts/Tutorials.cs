using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    public List<SphereCollider> Screws;
    public List<SphereCollider> WinnignSCrews;

    public List<MeshCollider> screwholder;
    public List<GameObject> Hand;
    public static Tutorials t_instance;
    public int CurrentIndex = 0;

    public int CurrentHAndIndex = 0;
    private void Awake()
    {
        t_instance = this;
    }
    private void Start()
    {


        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            StartCoroutine(nameof(DelayTutorial));
        }
       
       
       
    }
    IEnumerator DelayTutorial()
    {
        yield return new WaitForSeconds(.5f);
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {

            foreach (SphereCollider item in Screws)
            {
                item.enabled = false;

            }
           
            foreach (MeshCollider item in screwholder)
            {
                item.enabled = false;
            }
            Screws[CurrentIndex].enabled = true;
            screwholder[CurrentIndex].enabled = true;
            Hand[CurrentHAndIndex].SetActive(true);

        }
    }
    public void HandActive()
    {
        if (CurrentHAndIndex < Hand.Count - 1)
        {
            CurrentHAndIndex++;
            foreach (GameObject item in Hand)
            {
                item.SetActive(false);

            }
            Hand[CurrentHAndIndex].SetActive(true);
        }
    }
    public void EnableColliders()
    {
        foreach (SphereCollider item in WinnignSCrews)
        {
            item.enabled = true;

        }
    }
    public void MoveNextHand()
    {
      
        if (CurrentIndex < Screws.Count-1)
        {
            CurrentIndex++;
            foreach (SphereCollider item in Screws)
            {
                item.enabled = false;
               
            }
          
            foreach (MeshCollider item in screwholder)
            {
                item.enabled = false;
            }
            Screws[CurrentIndex].enabled = true;
            screwholder[CurrentIndex].enabled = true;

          
        }
        
    }
}
