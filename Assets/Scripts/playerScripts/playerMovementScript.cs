using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class playerMovementScript : MonoBehaviour
{
    [Header("General Movement Data")] 
    Rigidbody2D rb;
    public float speed;
    public float dashMag;

    public float shiftTime = 0;
    public float shiftTimeMax;
    public KeyCode abilityKey;
    public int playerForm = 1;
    public Transform playerTransform;
    [SerializeField] private Vector3 COM;

    [Header("Arm Variables")] 
    // this variable may not be relevent and may be able to be removed
    public static Vector2 hingeJointAnchorDistance = new Vector2(1.9f, 0);
    public bool isConnected = false;
    [SerializeField] float maxRange;
    [SerializeField] LayerMask vineColliders;
    bool isLeftArmActive = false;
    public HingeJoint2D hJ;
    // Multiply to turn a singular unit into the scale of the object
    const float unitsToScale = 1 / 1.3f;
    GameObject curArm;
    // How much we want to favor higher vines in the algorithm
    const float yFavor = .5f;
    const float armExtendSpeed = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = COM;
        hJ = this.GetComponent<HingeJoint2D>();
        playerTransform = GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        // Just WASD add force 
        WASDMovementWithDash();
        // Ability actions
        if (Input.GetKeyDown(abilityKey))
        {
            switch (playerForm)
            {
                case 1:

                    break;
                case 2:
                    break;
                case 3:
                    armMovementAbilityInput();
                    break;
                default:
                    break;
            }

        }
    }
    private void OnDrawGizmos() 
    {
        // Draws a wirespehre where the hinge joints anchor point is
        // The arm is 1.9 units long so using trig we know the hypotenous is 1.9 and we can use trig to solve for the respective x and y values
        Gizmos.DrawWireSphere(new Vector3(1.9f * Mathf.Cos(Mathf.Deg2Rad * playerTransform.eulerAngles.z), 1.9f * Mathf.Sin(Mathf.Deg2Rad * playerTransform.eulerAngles.z), 0) + GetComponent<Transform>().position, .5f);
    }

    // Call this to first find the connected position and start sending out the arm
    private void armMovementAbilityInput()
    {
        // Stage Onea: searching for the valid vine
        GameObject vinePosObj = checkForClosestValidVine();
        // If no valid vine nothing should happen 
        if (vinePosObj == null)
        {
            return;
        }
        // Stage two
        StartCoroutine(shootArmOut(vinePosObj));
    }
    IEnumerator shootArmOut(GameObject vinePosObj)
    {
        // Finds the distance
        float distance = Vector2.Distance(vinePosObj.transform.position, playerTransform.position);
        // Gets rotation needed to point towards vinePosObj
        float rotation = Mathf.Rad2Deg * Mathf.Atan((playerTransform.position.y - vinePosObj.transform.position.y) 
        / (playerTransform.position.x - vinePosObj.transform.position.x)); 
        curArm.transform.eulerAngles = new Vector3(curArm.transform.rotation.x, curArm.transform.rotation.y, rotation);
        armMovementConnected(vinePosObj);
        yield return null;
    }
    // Call when armMovementAbilityInput finishes and the arm has reached the joint
    void armMovementConnected(GameObject vinePosObj)
    {
        // Stage three: Turning on Hinge Joint
        hJ.enabled = true;
        hJ.connectedBody = vinePosObj.GetComponentInParent<Rigidbody2D>();
        while (isConnected)
        {
            hJ.connectedAnchor = vinePosObj.transform.position;
        }
        // Stage 4
    }
    // Checks through each object with later vineColliders and eliminates the left or right side of the 
    // circle depending on which arm is active calculates which one is closest to the player returning that position
    public GameObject checkForClosestValidVine()
    {
        // returns an array of the objects with layer vineColliders
        Collider2D[] vines = Physics2D.OverlapCircleAll(this.gameObject.transform.position, maxRange, vineColliders);
        // initialize at a 10000 as the game is not that big so shouldnt be an issue
        float min = 100000;
        int place = -1;
        int iteration = 0;
        foreach (Collider2D vine in vines)
        {
            // If the x position of the vine is less then the players and left arm is active we execute
            if (isLeftArmActive && vine.transform.position.x - playerTransform.position.x < 0)
            {
                float dist = Vector2.Distance(vine.transform.position, playerTransform.position)
                // this adds a bias towards vines that are higher and 
                 - (yFavor * (vine.transform.position.y - playerTransform.position.y));
                if (min > dist)
                {
                    min = dist;
                    place = iteration;
                }
            }
            // If its to the right of the player and the right arm active
            else if (vine.transform.position.x - playerTransform.position.x > 0 && !isLeftArmActive)
            {
                float dist = Vector2.Distance(vine.transform.position, playerTransform.position);
                if (min > dist)
                {
                    min = dist;
                    place = iteration;
                }
            }
            iteration++;
        }
        // If no valid vine was found
        if (place == -1)
        {
            return null;
        }
        return vines[place].gameObject;
    }
    private void WASDMovementWithDash()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(new Vector2(speed, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(new Vector2(-speed, 0));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(new Vector2(0, speed));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(new Vector2(0, -speed));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(new Vector2(dashMag, 0));
            }
            else if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(new Vector2(-dashMag, 0));
            }
            else if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(new Vector2(0, dashMag));
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(new Vector2(0, -dashMag));
            }
        }
    }
}