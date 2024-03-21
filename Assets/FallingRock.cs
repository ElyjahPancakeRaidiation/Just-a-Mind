using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField]private float fallingDelay;
    //[SerializeField]private float fallingSpeed;
    [SerializeField]private GameObject rockObj;
    //[SerializeField]private Vector2 stopPosition;//The rock stops falling past this point
    [SerializeField]private bool isRockFalling;
    private Rigidbody2D rockRb;

    private RaycastHit2D rockCast;
    [SerializeField]private float rockcastDist;
    [SerializeField]private LayerMask fallingRock;


    // Start is called before the first frame update
    void Start()
    {
        rockRb = rockObj.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Search if rockcast is still in ray, if not stop its movement
        if (isRockFalling)
        {
            rockRb.gravityScale = 1;
        }
        else
        {
            rockRb.gravityScale = 0;
        }

        if (rockCast.collider == null)
        {
            isRockFalling = false; 
            print("Rock is not in"); 
        }
    }

    private void FixedUpdate() {
        rockCast = Physics2D.Raycast(transform.position, Vector2.down, rockcastDist, fallingRock);
        //Debug.DrawRay(transform.position, Vector2.down * rockcastDist, Color.red);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //When the player touches it activate enum for rock
        if (!isRockFalling)
        {
            StartCoroutine(ActivateRock());
        }
    }

    private IEnumerator ActivateRock(){
        //Play animation and wait before it is able to fall.
        //Do animation + particals1
        yield return new WaitForSeconds(fallingDelay);
        isRockFalling = true;
    } 

    private void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, Vector2.down * rockcastDist);
    }
}
