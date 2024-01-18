using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HookShootScript : MonoBehaviour
{
	#region Hookshot
	public Rigidbody2D playersRB2D;

	public PlayerController pc;
	public GameObject Player;

    public float hookshotRange; // Sets the range the hookshot can go
	public float hookshotSpeed; // Sets how fast you're going towards your hookshot
	public float distance = 3.5f;

	public LayerMask grappleLayer;

	public LineRenderer LR;

	public Vector2 hookShotTarget; // Where you are hook shooting to

	public Transform spherePoint;
	public Transform grapplePoint;

	public bool areYouHookShooting = false; // Are you hook shooting?
	public bool isConnected = false;
	#endregion

	#region Keycodes
	public KeyCode hookShotKey;
	#endregion

	// Start is called before the first frame update
	void Start()
	{
		playersRB2D = GetComponent<Rigidbody2D>();
		Player = GameObject.Find("Player");
		pc = GetComponent<PlayerController>();
		LR.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(hookShotKey)&& !areYouHookShooting) // If the player hits the Fire1 button and is not hookshooting
		{
			/*StartHookShot();*/ // Start the hookshoot function
		}
		else if (Input.GetKeyDown(hookShotKey) && areYouHookShooting) // If the player hits the Fire1 button and is hookshooting
		{
			EndHookshot(); // Start the end hookshoot function
		}

		if (areYouHookShooting) // If the player is hookshooting
		{
			HookshotMovement(); // Start hookshot movement function
		}
	}

	/*public void StartHookShot()
	{
		pc.vineCol = Physics2D.OverlapCircle(spherePoint.transform.position, pc.interactRadius, interactMask); //set circleCol to Overlap Cirlce

		if (isConnected) // If the cirlce hits something that is in the hook shot range and is in the ground layer
		{
			areYouHookShooting = true; // You are hook shooting
			hookShotTarget = grapplePoint.position; // Hook shot target is equal to the point the raycast hit

			LR.enabled = true; // Line Renderer is enabled
			LR.SetPosition(0, transform.position); // Starts at grapple tip
			LR.SetPosition(1, grapplePoint.position); // Ends at the target
		}

	}*/

	public void HookshotMovement()
	{
		Vector2 hookshotDirection = (hookShotTarget - (Vector2)transform.position).normalized; // Fire off a vector 2 at the shooting target subtracting its transform.position at a magnitude of 1
		playersRB2D.velocity = hookshotDirection * hookshotSpeed; // Shoot the player in the hookshotDirection at the hookshootSpeed

		LR.SetPosition(0, transform.position);
		LR.SetPosition(1, grapplePoint.position);

		if (Vector2.Distance(transform.position, grapplePoint.position) < 1) // If the distance from the transform.position and hookShotTarget is less than 1
		{
			EndHookshot();
		}
	}

	public void EndHookshot()
	{
		areYouHookShooting = false; // You are not hookshooting
		playersRB2D.velocity = Vector2.zero; // The player's Vector 2 velocity is equal to zero
		LR.enabled = false; // The Line Renderer is not enabled
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(spherePoint.position, hookshotRange);
	}

}
