using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class Ball : MonoBehaviour
{
    public BallPlaced ballplacedobj;
    public bool ispickable;
    public NutsColor nutscolor;
    public float raycastDistance = 10f;
    public LayerMask layerMask; // Optional: to limit which layers the rays can hit

    // Define directions in which rays will be cast
    private Vector3[] directions = {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };
    private void Start()
    {
       // gameObject.name = nutscolor.ToString();
        ispickable = true;
       
        foreach (Vector3 dir in directions)
        {
            // Cast the ray from the object's position in the specified direction
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, raycastDistance, layerMask);

            // Process each hit
            foreach (RaycastHit hit in hits)
            {
                Collider hitCollider = hit.collider;
                if (hitCollider.gameObject.tag == "place")
                {
                    ballplacedobj = hit.collider.gameObject.GetComponent<BallPlaced>();
                    hit.collider.gameObject.GetComponent<BallPlaced>().Nut = this;

                    Debug.Log("Hit object: " + hitCollider.name + " in direction: " + dir);
                }
                    

               
            }
        }
        
    }
    private void Update()
    {
        
    }

}
public enum NutsColor
{
    Brown,
    Red,
    Cream,
    Blue,
    Black,
    White,
    Green,
    Yellow,
    Orange
}

