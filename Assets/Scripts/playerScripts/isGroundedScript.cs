using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    GameObject groundPoint;
    LayerMask groundMask;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        groundPoint = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        this.transform.position = player.GetComponent<Transform>().position + new Vector3(0, 2.5f, 0);
    }

    public bool isOnGround()
    {
        if (Physics2D.Raycast(groundPoint.GetComponent<Transform>().position, Vector2.up, 1.5f, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
