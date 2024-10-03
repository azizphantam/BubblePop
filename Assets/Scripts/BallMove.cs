

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class BallMove : MonoBehaviour
{


    private GameObject SelectedScrew;
    private Camera mainCamera;
    public LevelManager levelManager;
    public GameObject WinningMeshObj;


    [Space]
    public List<GameObject> Screws = new List<GameObject>();

    public List<GameObject> WrongScrews = new List<GameObject>();


    public List<BallPlaced> ScrewHolesList = new List<BallPlaced>();






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



    #region Swapping/Hint

    [Header("Swapping Screws")]
    [Space]
    public List<Ball> SwapScrew_L = new List<Ball>();
    public List<BallPlaced> SwapHoles_L = new List<BallPlaced>();


    public List<BallPlaced> SwappingHolesList = new List<BallPlaced>();

    public Ball SwapedScrew_1, SwapedScrew_2;
    public BallPlaced SwapHolder1, SwapHolder2;

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
        GameManager.gm.progressionamount = 1 / (float)levelManager.WrongsBalls;

        #endregion
    }
    public void SwapNuts()
    {

        SwapScrew_L.Clear();
        SwappingHolesList.Clear();
        SwapHoles_L.Clear();

        SwapedScrew_1 = null;
        SwapedScrew_2 = null;
        SwapHolder1 = null;
        SwapHolder2 = null;



        for (int i = 0; i < WrongScrews.Count; i++)
        {
            SwapScrew_L.Add(WrongScrews[i].GetComponent<Ball>());
            SwapHoles_L.Add(SwapScrew_L[i].GetComponent<Ball>().ballplacedobj);

        }

        // we have screws and screws holders


        for (int i = 0; i < WrongScrews.Count; i++)
        {
            for (int j = 0; j < SwapHoles_L.Count; j++)
            {


                if (WrongScrews.Count >= 2)
                {
                    if ((int)WrongScrews[i].GetComponent<Ball>().nutscolor == (int)SwapHoles_L[j].nutsplacedcolor && (int)WrongScrews[i].GetComponent<Ball>().ballplacedobj.nutsplacedcolor == (int)SwapHoles_L[j].Nut.nutscolor)
                    {
                        if (!SwappingHolesList.Contains(SwapHoles_L[j]))
                        {
                            SwappingHolesList.Add(SwapHoles_L[j]); // add the swapped screw

                            SwapedScrew_1 = WrongScrews[i].GetComponent<Ball>();  // Add first screw
                            SwapedScrew_2 = SwapHoles_L[j].Nut; // Add second screw

                            SwapHolder1 = SwapedScrew_1.ballplacedobj;
                            SwapHolder2 = SwapedScrew_2.ballplacedobj;

                            SwapedScrew_1.transform.DOJump(SwapedScrew_2.ballplacedobj.transform.GetChild(0).transform.position, .5f, 1, .5f);
                            
                            SwapedScrew_2.transform.DOJump(SwapedScrew_1.ballplacedobj.transform.GetChild(0).transform.position, .5f, 1, .5f);
                           


                            SwapedScrew_1.ballplacedobj = SwapHolder2;
                            SwapHolder2.Nut = SwapedScrew_1;


                            SwapedScrew_2.ballplacedobj = SwapHolder1;
                            SwapHolder1.Nut = SwapedScrew_2;

                            SwapedScrew_1.transform.DOMoveY(SwapedScrew_1.transform.position.y + .2f, .2f).SetDelay(.6f);
                            SwapedScrew_2.transform.DOMoveY(SwapedScrew_2.transform.position.y + .2f, .2f).SetDelay(.6f);



                            levelManager.WrongsBalls -= 2;
                            WrongScrews.Clear();
                            CallWrongScrewAction();

                            if (levelManager.WrongsBalls == 0)
                            {
                                Debug.Log("Level Won");
                                GameManager.gm.IsTimerRunning = false;
                                GameManager.gm.IsHintTimerRunning = false;
                                Invoke(nameof(EnablePopup), 1);

                            }
                            return;

                        }
                    }

                }
                else
                {
                    GameManager.gm.EnoughSwaps.SetActive(true);
                }

            }
        }

    }


    public void HintNuts()
    {

        SwapScrew_L.Clear();
        SwappingHolesList.Clear();
        SwapHoles_L.Clear();

        SwapedScrew_1 = null;
        SwapedScrew_2 = null;
        SwapHolder1 = null;
        SwapHolder2 = null;



        for (int i = 0; i < WrongScrews.Count; i++)
        {
            for (int j = 0; j < ScrewHolesList.Count; j++)
            {

                
                    if ((int)WrongScrews[i].GetComponent<Ball>().nutscolor == (int)ScrewHolesList[j].nutsplacedcolor && ScrewHolesList[j].isEmptySpace == true)
                    {
                        WrongScrews[i].GetComponent<Ball>().ballplacedobj.isEmptySpace = true;
                        WrongScrews[i].GetComponent<Ball>().ballplacedobj = ScrewHolesList[j];
                        WrongScrews[i].GetComponent<Ball>().ballplacedobj.isEmptySpace = false;
                        ScrewHolesList[j].Nut = WrongScrews[i].GetComponent<Ball>();
                        WrongScrews[i].transform.DOJump(ScrewHolesList[j].gameObject.transform.GetChild(0).transform.position, .5f, 1, .5f);
                        WrongScrews[i].transform.DOMoveY(WrongScrews[i].transform.position.y + .2f, .2f).SetDelay(.6f);


                        levelManager.WrongsBalls -= 1;
                        WrongScrews.Clear();
                        CallWrongScrewAction();

                        if (levelManager.WrongsBalls == 0)
                        {
                            Debug.Log("Level Won");
                            GameManager.gm.IsTimerRunning = false;
                            GameManager.gm.IsHintTimerRunning = false;
                            Invoke(nameof(EnablePopup), 1);

                        }
                        return;
                    }
               
               
            }
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
                            ParticleSystem a = Instantiate(item, hit.collider.transform.position, Quaternion.identity);
                            a.transform.position = new Vector3(a.transform.position.x, a.transform.rotation.y + .8f, a.transform.position.z);
                            a.Play();
                        }

                        Destroy(hit.collider.gameObject, .1f);
                        PopUp.clip = select_deselect[0];
                        PopUp.Play();
                        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
                        numberofscrews--;

                        if (numberofscrews <= 0)
                        {
                            ispopup = false;

                            levelManager.WonLevel();
                            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
                            {
                                GameManager.gm.HandAnim.SetActive(false);
                            }
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
                            GameManager.gm.IsHintTimerRunning = false;
                            Invoke(nameof(EnablePopup), 1);

                        }


                        levelManager.DecrementWrong();
                        if (GameManager.gm.IsHintTimerRunning)
                        {
                            HintNuts();
                        }
                       
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
        levelManager.Popitnow.transform.DOScale(1, 1).SetEase(Ease.OutBounce);
        for (int i = 0; i < ScrewHolesList.Count; i++)
        {


            if (ScrewHolesList[i].isEmptySpace == true)
            {
                Debug.Log("Available placed ");

                FinalScrew.gameObject.transform.DOJump(ScrewHolesList[i].gameObject.transform.GetChild(0).transform.position, .5f, 1, .5f);
                FinalScrew.gameObject.transform.DOMoveY(FinalScrew.gameObject.transform.position.y + 0.05f, .4f).SetDelay(.4f);
                numberofscrews = numberofscrews + 1;
                LastScrewHole = ScrewHolesList[i].gameObject;
                Invoke(nameof(LastScrewSound), .4f);

                Debug.Log("Sounds");
            }
        }

    }

    public void LastScrewSound()
    {
        PopUp.clip = select_deselect[0];
        PopUp.Play();
        LastScrewHole.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            GameManager.gm.HandAnim.SetActive(true);
        }
       
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

    #region MovesFunctionality
    public void CheckMovesUP()
    {


        Invoke(nameof(CheckMoves), 1);
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
        for (int i = 0; i < MovesScrewlist.Count; i++)
        {
            if ((int)MovesScrewlist[i].nutsplacedcolor == (int)MovesScrewlist[i].Nut.nutscolor)
            {
                screwDone++;
                if (screwDone == listLimit - 1)
                {
                    foreach (var t in MovesScrewlist)
                    {
                        if (t.isEmptySpace && levelManager.WrongsBalls > 0)
                        {
                            Debug.Log("Movesup");
                            levelManager.TopInfoOutOfMoves();
                        }
                    }

                }


            }

        }
    }
    #endregion
}