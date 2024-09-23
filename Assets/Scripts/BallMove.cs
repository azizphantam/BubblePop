

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using System.Collections;

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
    public List<GameObject> ScrewPlacing = new List<GameObject>();
    public List<GameObject> WrongScrews = new List<GameObject>();

    public List<Ball> HintScrew_List = new List<Ball>();
    public List<BallPlaced> ScrewSwapPlacing_List = new List<BallPlaced>();
    #endregion

    public List<Ball> DoubleTapScrew = new List<Ball>();
    public bool ispopup = false;
    public int numberofscrews;


    #region UndoFuntionality
    [Header("Undo Screws")]
    [Space]
    public BallPlaced PlacedScrew_Undo;
    public Ball Screw_Undo;
    #endregion

    
    void Start()
    {
        mainCamera = Camera.main;
        numberofscrews = DoubleTapScrew.Count;


        Invoke(nameof(CallWrongScrewAction), .2f);

        
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
        #endregion
    }
    public void SwapNuts()
    {
        for (int i = 0; i < WrongScrews.Count; i++)
        {

            if ((int)WrongScrews[i].GetComponent<Ball>().nutscolor != (int)WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>().nutsplacedcolor)
            {
                HintScrew_List.Add(WrongScrews[i].GetComponent<Ball>());
                ScrewSwapPlacing_List.Add(WrongScrews[i].GetComponent<Ball>().ballplacedobj);

            }

        }
        for (int i = 0; i < ScrewSwapPlacing_List.Count; i++)
        {
            if ((int)HintScrew_List[0].GetComponent<Ball>().nutscolor == (int)WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>().nutsplacedcolor)
            {
                HintScrew_List[0].GetComponent<Ball>().ballplacedobj.isEmptySpace = true;
                HintScrew_List[0].GetComponent<Ball>().ballplacedobj = WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>();
                HintScrew_List[0].GetComponent<Ball>().ballplacedobj.isEmptySpace = false;
                WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>().gameObject.GetComponent<BallPlaced>().Nut = SelectedScrew.GetComponent<Ball>();
                HintScrew_List[0].GetComponent<Ball>().transform.DOJump(WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>().transform.GetChild(0).transform.position, 3, 1, 1);
                ScrewSwapPlacing_List.Remove(WrongScrews[i].GetComponent<Ball>().ballplacedobj.GetComponent<BallPlaced>());
                // Invoke(nameof(CallActionComplete), 1.1f);
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
                        Destroy(hit.collider.gameObject);
                        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
                        numberofscrews--;

                        if (numberofscrews <= 0)
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
                    #region Tap Failed on same screw color placed
                    //if ((int)hit.collider.gameObject.GetComponent<Ball>().nutscolor != (int)hit.collider.gameObject.GetComponent<Ball>().ballplacedobj.nutsplacedcolor)
                    //{
                    //    MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
                    //    if (hit.collider.gameObject.GetComponent<Ball>().ispickable == true)
                    //    {
                    //        foreach (Ball ball in DoubleTapScrew)
                    //        {
                    //            if (ball.ispickable == false && ((int)ball.nutscolor != (int)ball.ballplacedobj.nutsplacedcolor))
                    //            {
                    //                ball.ispickable = true;
                    //                ball.transform.DOLocalMoveY(ball.transform.localPosition.y - ScrewPosValf, .5f);
                    //            }
                    //        }
                    //        hit.collider.gameObject.GetComponent<Ball>().ispickable = false;
                    //        SelectedScrew = hit.collider.gameObject;
                    //        SelectedScrew.transform.DOLocalMoveY(SelectedScrew.transform.localPosition.y + ScrewPosValf, .5f);

                    //    }
                    //    else
                    //    {
                    //        hit.collider.gameObject.GetComponent<Ball>().ispickable = true;
                    //        hit.collider.gameObject.transform.DOLocalMoveY(hit.collider.gameObject.transform.localPosition.y - ScrewPosValf, .5f);

                    //    }
                    //}
                    //else
                    //{
                    //    MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
                    //} 
                    #endregion


                    if (hit.collider.gameObject.GetComponent<Ball>().ispickable == true)
                    {
                        foreach (Ball ball in DoubleTapScrew)
                        {
                            //if (ball.ispickable == false && ((int)ball.nutscolor != (int)ball.ballplacedobj.nutsplacedcolor))
                            //{
                            //    ball.ispickable = true;
                            //    ball.transform.DOMoveY(ball.transform.localPosition.y - GameManager.gm.ScrewPosVal, .5f);
                            //}


                            if (ball.ispickable == false)
                            {
                                ball.ispickable = true;
                                ball.transform.DOMoveY(ball.transform.position.y - GameManager.gm.ScrewPosVal, .2f).SetEase(Ease.InOutBounce);
                            }

                        }
                        hit.collider.gameObject.GetComponent<Ball>().ispickable = false;
                        SelectedScrew = hit.collider.gameObject;
                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y + GameManager.gm.ScrewPosVal, .2f).SetEase(Ease.InOutBounce);

                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<Ball>().ispickable = true;
                        hit.collider.gameObject.transform.DOMoveY(hit.collider.gameObject.transform.position.y - GameManager.gm.ScrewPosVal, .2f).SetEase(Ease.InOutBounce);

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



                    if ((int)SelectedScrew.GetComponent<Ball>().nutscolor == (int)hit.collider.gameObject.GetComponent<BallPlaced>().nutsplacedcolor)
                    {
                       
                        Vector3 placementPosition = hit.point;

                        if ((int)SelectedScrew.GetComponent<Ball>().ballplacedobj.nutsplacedcolor != (int)SelectedScrew.GetComponent<Ball>().nutscolor)
                        {
                            levelManager.WrongsBalls--;
                            levelManager.wrongballs.text = levelManager.WrongsBalls.ToString();
                            WrongScrews.Remove(SelectedScrew);
                        }//for decrease value only if ball placed from wrong color to right color

                        SelectedScrew.GetComponent<Ball>().ballplacedobj.isEmptySpace = true;
                        SelectedScrew.GetComponent<Ball>().ballplacedobj = hit.collider.gameObject.GetComponent<BallPlaced>();
                        SelectedScrew.GetComponent<Ball>().ballplacedobj.isEmptySpace = false;
                        hit.collider.gameObject.GetComponent<BallPlaced>().Nut = SelectedScrew.GetComponent<Ball>();
                        SelectedScrew.transform.DOJump(hit.collider.gameObject.transform.GetChild(0).transform.position, 3, 1, .5f);
                        Invoke(nameof(CallActionComplete), .6f);

                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y - GameManager.gm.ScrewPosVal, .2f).SetEase(Ease.InOutBounce).SetDelay(.6f);
                        SelectedScrew.GetComponent<Ball>().ispickable = true;
                        SelectedScrew = null; // Deselect the object
                        if (levelManager.WrongsBalls == 0)
                        {
                            Debug.Log("Level Won");
                            Invoke(nameof(EnablePopup), 1);
                           
                        }


                        levelManager.DecrementWrong();

                    }
                    else
                    {
                        SelectedScrew.transform.DOMoveY(SelectedScrew.transform.position.y - GameManager.gm.ScrewPosVal, .2f).SetEase(Ease.InOutBounce);
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
}
