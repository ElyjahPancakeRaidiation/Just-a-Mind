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
	public GameObject grabOn;

	public Collider2D vineCol;

	public float hookshotRange; // Sets the range the hookshot can go
	public float hookshotSpeed; // Sets how fast you're going towards your hookshot
	public float distance = 3.5f;

	public LayerMask grappleLayer;

	public LineRenderer LR;

	public Vector2 hookShotTarget; // Where you are hook shooting to

	public Transform spherePoint;

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
		LR = GetComponent<LineRenderer>();
		LR.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(hookShotKey)&& !areYouHookShooting) // If the player hits the Fire1 button and is not hookshooting
		{
			StartHookShot(); // Start the hookshoot function
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

	public void StartHookShot()
	{
		//Change this to OverlapCircleAll so that it can cycle through a list and choose the closet one
		//Add Raycast in order to not make it break while going through walls 

		pc.vineCol = Physics2D.OverlapCircle(spherePoint.transform.position, hookshotRange, grappleLayer); //set circleCol to Overlap Cirlce

		if (pc.vineCol != null)
		{
			Debug.Log("Respawner at index " + pc.vineCol + " is within the circle cast.");
		}


		if ((pc.vineCol == pc.grabOn || pc.vineCol == null)) //if cirlce collider is equal or if circle collider is equal to null return
		{
			isConnected = false;
			return; //ensure that that there's never a null in the spawner
		}

		else if ((pc.vineCol != pc.grabOn && pc.vineCol != null))
		{
			pc.grabOn = pc.vineCol.gameObject;
			isConnected = true;
		}

		if (isConnected) // If the cirlce hits something that is in the hook shot range and is in the ground layer
		{
			areYouHookShooting = true; // You are hook shooting
			hookShotTarget = pc.grabOn.transform.position; // Hook shot target is equal to the point the raycast hit

			LR.enabled = true; // Line Renderer is enabled
			LR.SetPosition(0, transform.position); // Starts at grapple tip
			LR.SetPosition(1, pc.grabOn.transform.position); // Ends at the target
		}

	}

	public void HookshotMovement()
	{
		Vector2 hookshotDirection = (hookShotTarget - (Vector2)transform.position).normalized; // Fire off a vector 2 at the shooting target subtracting its transform.position at a magnitude of 1
		playersRB2D.velocity = hookshotDirection * hookshotSpeed; // Shoot the player in the hookshotDirection at the hookshootSpeed

		LR.SetPosition(0, transform.position);
		LR.SetPosition(1, pc.grabOn.transform.position);

		if (Vector2.Distance(transform.position, pc.grabOn.transform.position) < 1) // If the distance from the transform.position and hookShotTarget is less than 1
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
