using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    [SerializeField] LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        transform.position = player.transform.position + new Vector3(0, -2.5f, 0);
    }

    public bool isGrounded()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
