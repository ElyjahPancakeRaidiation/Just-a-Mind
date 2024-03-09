using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    [SerializeField] LayerMask groundLayer;
    public Vector2[] vecScales;
    private float angle;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

    }
    
    private void FixedUpdate() {
        transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
    }

    void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, vecScales[(int) PlayerController.playerForm]);

}
