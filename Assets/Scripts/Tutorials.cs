using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    public List<SphereCollider> Screws;
    public GameObject Hand;
    public List<Transform> StartPos_Hand;
    public List<Transform> EndPos_Hand;
    public static Tutorials t_instance;
    public int CurrentIndex = 0;
    private void Awake()
    {
        t_instance = this;
    }
    private void Start()
    {
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            MoveNextHand();
        }
        else
        {
            Hand.SetActive(false);
        }
       
    }
    public void MoveNextHand()
    {
        Hand.transform.DOMove(StartPos_Hand[CurrentIndex].transform.position, .5f);
        if (CurrentIndex < Screws.Count)
        {
            CurrentIndex++;
            foreach (SphereCollider item in Screws)
            {
                item.enabled = false;
            }
            Screws[CurrentIndex].enabled = true;
            Hand.transform.DOMove(EndPos_Hand[CurrentIndex].transform.position, .5f);
        }
        else
        {
            Hand.gameObject.SetActive(false);
        }
    }
}
