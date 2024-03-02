using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    private Collider2D groundCol;
    [SerializeField]private List<Vector2> colSize;
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
        transform.position = player.transform.position + new Vector3(0, -1 * (rayScales[(int) PlayerController.playerForm] + .2f), 0);
    }

    public bool isGrounded()
    {
        /*
        groundCol = Physics2D.OverlapBox(transform.position, colSize[(int) PlayerController.playerForm], groundLayer);

        if (groundCol)
        {
            return true;
        }
        
        return false;
        */
        
        
        if (Physics2D.Raycast(transform.position, Vector2.up, rayScales[(int) PlayerController.playerForm], groundLayer))
        {
            Debug.DrawRay(transform.position, Vector2.up * rayScales[(int) PlayerController.playerForm], Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, Vector2.up * rayScales[(int) PlayerController.playerForm], Color.red);
            return false;
        }
        
    }

    private void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position, colSize[(int) PlayerController.playerForm]);
}
