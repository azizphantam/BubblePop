

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BallMove : MonoBehaviour
{


    private GameObject SelectedScrew;
    private Camera mainCamera;
    public LevelManager levelManager;
    public GameObject WinningMeshObj;

    #region Swapping/Hint
    [Header("Swapping Screws")]
    [Space]
    public List<GameObject> Screws = new List<GameObject>();
  
    public List<GameObject> WrongScrews = new List<GameObject>();

    public List<Ball> HintScrew_List = new List<Ball>();
    public List<BallPlaced> ScrewHolesList = new List<BallPlaced>();

    


    #endregion

    public List<Ball> DoubleTapScrew = new List<Ball>();
    public bool ispopup = false;
    public int numberofscrews;
    float Y_Axis;
    public Ball FinalScrew;
    public AudioClip[] select_deselect;
    public AudioSource PopUp;
    public List<ParticleSystem> DragScrewPArticles;
    List<ParticleSystem> instanlist;
    public float Time;
    public Sprite lvlbg;
    public Sprite lvlimg;
    public GameObject LastScrewHole;

    public currentEmptyColor EmptyScrewcol;
   
    int countScrewFixed;
    public List<BallPlaced> MovesScrewlist;
    #region UndoFuntionality
    [Header("Undo Screws")]
    [Space]
    public BallPlaced PlacedScrew_Undo;
    public Ball Screw_Undo;
    #endregion

    public enum currentEmptyColor
    {
        Brown,
        Red,
        Cream,
        Blue,
        Black,
        White,
        Green,
        Yellow,
        Orange,
        pink
    }
    void Start()
    {
        mainCamera = Camera.main;
        numberofscrews = DoubleTapScrew.Count;
        Y_Axis = GameManager.gm.ScrewPosVal;
        LastScrewHole = null;
        countScrewFixed = 0;
        MovesScrewlist.Clear();
        CheckMoveHolesCol();
        Invoke(nameof(CallWrongScrewAction), .2f);
        listLimit = MovesScrewlist.Count;
        
    }
    public void CallWrongScrewAction()
    {
        #region WrongScrews
        for (int i = 0; i < Screws.Count; i++)
        {

            if ((int)Screws[i].GetComponent<Ball>().nutscolor != (int)Screws[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>().nutsplacedcolor)
            {
                WrongScrews.Add(Screws[i]);
            }
        }
        levelManager.WrongsBalls = WrongScrews.Count;
        levelManager.wrongballs.text = levelManager.WrongsBalls.ToString();
        GameManager.gm.progressionamount =  1 / (float) levelManager.WrongsBalls;
       
        #endregion
    }
    public void SwapNuts()
    {
        HintScrew_List.Clear();
        ScrewHolesList.Clear();
        for (int i = 0; i < WrongScrews.Count; i++)
        {
            HintScrew_List.Add(WrongScrews[i].GetComponent<Ball>());
            ScrewHolesList.Add(WrongScrews[i].GetComponent<Ball>().ballplacedobj);
        }

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            SelectObject();
            if (SelectedScrew != null)
            {
                PlaceObjectAtMousePosition();

            }


        }


        if (ispopup)
        {
            if (Input.touchCount > 0)
            {
                // Get the first touch input
                Touch touch = Input.GetTouch(0);

                // Convert touch position to a ray
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the raycast hits this object
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        
                        hit.collider.GetComponent<SphereCollider>().enabled = false;
                        foreach (var item in DragScrewPArticles)
                        {
                            ParticleSystem a = Instantiate(item,hit.collider.transform.position,Quaternion.identity);
                            a.transform.position = new Vector3(a.transform.position.x, a.transform.rotation.y+.8f, a.transform.position.z);
                            a.Play();
                        }

                        Destroy(hit.collider.gameObject,.1f);
                        PopUp.clip = select_deselect[0];
                        PopUp.Play();
                        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
                        numberofscrews--;

                        if (numberofscrews <=    0)
                        {
                            ispopup = false;
                           
                            levelManager.WonLevel();
                        }
                    }
                }
            }
        }
    }

    void SelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.collider.gameObject.CompareTag("Player"))
            {

                if (ispopup == false)

                {
                  


                    if (hit.collider.gameObject.GetComponent<Ball>().ispickable == true)
                    {
                        foreach (Ball ball in DoubleTapScrew)
                        {
                            
                            if (ball.ispickable == false)
                            {
                                ball.ispickable = true;
                                ball.transform.DOMoveY(ball.transform.position.y - Y_Axis, .1f);
                            }

                        }
                        hit.collider.gameObject.GetComponent<Ball>().ispickable = false;
                        SelectedScrew = hit.collider.gameObject;
                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y + Y_Axis, .1f);
                        PopUp.clip = select_deselect[0];
                        PopUp.Play();
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<Ball>().ispickable = true;
                        hit.collider.gameObject.transform.DOMoveY(hit.collider.gameObject.transform.position.y - Y_Axis, .1f);
                        PopUp.clip = select_deselect[1];
                        PopUp.Play();
                        
                    }

                }

            }

        }
    }

    void PlaceObjectAtMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.collider.gameObject.CompareTag("place"))
            {
                Debug.Log(hit.collider.gameObject);
                MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
                if (hit.collider.gameObject.GetComponent<BallPlaced>().isEmptySpace == true)
                {

                    PopUp.clip = select_deselect[1];
                    PopUp.Play();
                   
                    if ((int)SelectedScrew.GetComponent<Ball>().nutscolor == (int)hit.collider.gameObject.GetComponent<BallPlaced>().nutsplacedcolor)
                    {
                       
                        Vector3 placementPosition = hit.point;

                        if ((int)SelectedScrew.GetComponent<Ball>().ballplacedobj.nutsplacedcolor != (int)SelectedScrew.GetComponent<Ball>().nutscolor)
                        {
                            levelManager.WrongsBalls--;
                           
                            GameManager.gm.IncreaseProgression();
                            levelManager.wrongballs.text = levelManager.WrongsBalls.ToString();
                            WrongScrews.Remove(SelectedScrew);

                            CheckMovesUP();
                        }//for decrease value only if ball placed from wrong color to right color

                        SelectedScrew.GetComponent<Ball>().ballplacedobj.isEmptySpace = true;
                        SelectedScrew.GetComponent<Ball>().ballplacedobj = hit.collider.gameObject.GetComponent<BallPlaced>();
                        SelectedScrew.GetComponent<Ball>().ballplacedobj.isEmptySpace = false;
                        hit.collider.gameObject.GetComponent<BallPlaced>().Nut = SelectedScrew.GetComponent<Ball>();
                        SelectedScrew.transform.DOJump(hit.collider.gameObject.transform.GetChild(0).transform.position, .5f, 1, .5f);
                        Invoke(nameof(CallActionComplete), .6f);
                        hit.collider.gameObject.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y - Y_Axis, .2f).SetDelay(.4f);
                        SelectedScrew.GetComponent<Ball>().ispickable = true;
                        SelectedScrew = null; // Deselect the object
                        if (levelManager.WrongsBalls == 0)
                        {
                            Debug.Log("Level Won");
                            GameManager.gm.IsTimerRunning = false;
                            Invoke(nameof(EnablePopup), 1);
                           
                        }


                        levelManager.DecrementWrong();
                       
                    }
                    else
                    {
                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y - Y_Axis, .2f).SetEase(Ease.InOutBounce);
                        SelectedScrew.GetComponent<Ball>().ispickable = true;
                        SelectedScrew = null;
                    }
                }
                else
                {

                    SelectedScrew = null;

                    MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);

                }
            }

        }
    }
    public void EnablePopup()
    {
        ispopup = true;
        WinningMeshObj.SetActive(true);
        for (int i = 0; i < ScrewHolesList.Count; i++)
        {
           
           
            if (ScrewHolesList[i].isEmptySpace == true)
            {
                Debug.Log("Available placed ");

                FinalScrew.gameObject.transform.DOJump(ScrewHolesList[i].gameObject.transform.GetChild(0).transform.position,.5f, 1, .5f);
                FinalScrew.gameObject.transform.DOMoveY(FinalScrew.gameObject.transform.position.y + 0.05f , .4f).SetDelay(.4f);
                numberofscrews = numberofscrews + 1;
                LastScrewHole = ScrewHolesList[i].gameObject;
                Invoke(nameof(LastScrewSound),.4f);
                
                Debug.Log("Sounds");
            }
        }
       
    }
   
    public void LastScrewSound()
    {
        PopUp.clip = select_deselect[0];
        PopUp.Play();
        LastScrewHole.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
    }
    public void CallActionComplete()
    {
       
        MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
        
    }
   


    public void Undo()
    {
        for (int i = 0; i < WrongScrews.Count; i++)
        {
            //  if (nutsplaced[i].transform.Ge)
        }
    }

    public void CheckMovesUP()
    {


        Invoke(nameof(CheckMoves) , 1);
    }
    public void CheckMoveHolesCol()
    {
        for (int i = 0; i < ScrewHolesList.Count; i++)
        {

            if ((int)ScrewHolesList[i].nutsplacedcolor == (int)EmptyScrewcol)
            {
                MovesScrewlist.Add(ScrewHolesList[i]);

            }

        }
    }
    int listLimit, screwDone;
    public void CheckMoves()
    {
        screwDone = 0;
        for (int i = 0;i < MovesScrewlist.Count; i++)
        {
            if ((int)MovesScrewlist[i].nutsplacedcolor == (int)MovesScrewlist[i].Nut.nutscolor)
            {
                screwDone++;
                if (screwDone == listLimit-1)
                {
                    foreach (var t in MovesScrewlist)
                    {
                        if(t.isEmptySpace && levelManager.WrongsBalls>0)
                            Debug.Log("Movesup"); 
                    }

                }
                   
               
            }
           
        }
    }
}