using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
	#region Movement
	public Vector2 movementDir;
    public float movementSpeed;
    public Rigidbody2D rb;
    #endregion

    #region Jumping
    public float jumpForce;
    public Transform groundPoint;
    public LayerMask groundMask;
    public KeyCode jumpKey;
	#endregion

	// Start is called before the first frame update
	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jumping();
	}

    void Movement() 
    {
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		movementDir = new Vector2(x, y);
		rb.AddForce(movementDir * movementSpeed); 
    }

    void Jumping() 
    {
        if (Input.GetKeyDown(jumpKey) && isOnGround()) 
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        bool isOnGround() 
        {
            RaycastHit2D hit;
            if (Physics2D.Raycast(groundPoint.position,Vector2.down,1.5f,groundMask))
            {
                return true;
            }
            else 
            {
                return false;
            }
            
        }
		Debug.DrawRay(groundPoint.position, Vector2.down * 1.5f, Color.red);
	}
	
}
