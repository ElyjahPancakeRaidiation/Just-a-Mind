using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [Header("General Variables")]
    private PlayerController player;
    public KeyCode abilityKey;
   
    private int bonusCharges;//bonus charges for the abilities 
    isGroundedScript groundedScript;
    #region Dashing variables
    [Header("Dash Variables")]
    
    [SerializeField]private float dashingDuration;//How long the dash will go for
    [SerializeField] float DASHPOWER;
    public static bool isDashing;
    [SerializeField]private int maxDashAmount;
    private int dashAmount;
    
    [SerializeField]float dashDelay;

    [SerializeField]float yDashModifier;
    
    #endregion
    [Header("Pogo Variables")]

    #region Pogo Jumpy up up variables
    public float jumpForce;
    public float superJumpForce;
    public Transform groundPoint;
    public bool canSuperJump = false;
    public float keyHoldDown;//Amount the jump is held down

    #endregion

    #region Arm variables
    
    public Transform playerTransform;
    [Space(10)]

    // this variable may not be relevent and may be able to be removed
    public static Vector2 hingeJointAnchorDistance = new Vector2(1.9f, 0);
    public bool isGrappling = false;
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
    #region Hookshot
	public float hookshotSpeed; // Sets how fast you're going towards your hookshot
	public LineRenderer LR;
    public List<GameObject> arms;
    // not sure this stuff is necessary
    /*public struct shoulderType
    {
        public GameObject shoulderObject;
        public bool isLeftShoulder;
        public Transform shoulderTransform;
        public Vector2 shoulderPosition;
        public Vector2 handPosition;
        public HingeJoint2D socketJoint;

    }
    shoulderType curShoulder;
    // Left shoulder is the 0 shoulder and right shoulder is the 1 shoulder
    public List<shoulderType> shoulders;
    */
    [SerializeField] float armShootSpeed;
    GameObject grapplingAnchorPointObject;
    public float armScale;
    bool usingLeft;
    public float grappleForce;
    const float armUnitsToScale = 1 / 2; // 1 scale per 2 unity units
    [SerializeField] Vector2 toShoulderDist; // the distance to add to get to the shoulder from center of player
	#endregion
    #endregion

    #region New Arm Variables
    bool handShot;
    [SerializeField] GameObject hand;
    [SerializeField] int horrizontalShootForce;
    [SerializeField] int diagnolxShootForce;
    [SerializeField] int diagnolyShootForce;
    [HideInInspector] public Vector2 shootVector;

    #endregion
    private void Start() {
        player = GetComponent<PlayerController>();
        dashAmount = maxDashAmount;
        //player.rb.centerOfMass = COM;
        hJ = this.GetComponent<HingeJoint2D>();
        playerTransform = GetComponent<Transform>();
        groundedScript = GameObject.Find("Ground Ray Object").GetComponent<isGroundedScript>();
        LR = GetComponent<LineRenderer>();
		LR.enabled = false;

        // grab arms

    }


    void Update()
    {
        // We must call in update because input breaks if we dont
        switch (PlayerController.playerForm)
        {
            case PlayerController.playerForms.Ball:
                Dash();
                break;
            case PlayerController.playerForms.Pogo:
                //Have the functions for pogos abilities
                Jumping();
                break;
            case PlayerController.playerForms.Arm:
                armMovement();
                break;
        }
    }

    IEnumerator ignoreResistences()
    {
        player.ignoreResistences = true;
        yield return new WaitForSeconds(.25f);
        player.ignoreResistences = false;
    }
    #region Ball abilites
    private void Dash(){
        if (!TestManager.transitioned)
        {
            if (Input.GetKeyDown(abilityKey) && !isDashing && player.horiLatestInput != 0)
            {
                if (dashAmount > 0 || bonusCharges > 0)
                {
                    StartCoroutine(Dashing(dashingDuration));
                    StartCoroutine(ignoreResistences());
                }
            }
        }
    }
    
    private IEnumerator Dashing(float duration){//Will push the player forward for a certain amount of time at a certain amount of speed
        // Starts camera shaking
        CameraScript.isCameraShaking = true;
        if (player.horizontal == 1)
        {
            player.rb.angularVelocity += 300 * player.horizontal;
        }
        else if(player.horizontal == -1)
        {
            player.rb.angularVelocity -= 300 * player.horizontal;
        }
        // We then add the dash force
        player.rb.AddForce(new Vector2(player.horiLatestInput * DASHPOWER, yDashModifier * DASHPOWER), ForceMode2D.Impulse);//dash in any direction boom kachow
        isDashing = true;        
        // wait then turn off cammera shake
        yield return new WaitForSeconds(duration);
        CameraScript.isCameraShaking = false;
        yield return new WaitUntil(() => groundedScript.isGrounded());
        ResetDash();
        // after we reset the dassh we can then transition to an arm boost which may allow more arier movement
    }
    private void ResetDash(){//Resets all of the variables in dash mechanic
        isDashing = false;
    }
    #endregion
    
    #region Pogo abilities
    void Jumping()
    {
        // Only allows if the player is grounded which
        if (groundedScript.isGrounded())
        {   if (Input.GetKeyDown(abilityKey))
            {
                StartCoroutine(ignoreResistences());
                player.rb.AddForce(new Vector2(0, superJumpForce), ForceMode2D.Impulse);
            }
        }
        /*IEnumerator debugger()
        {
            for(int i = 0; i < 100; i++)
            {
                Debug.Log(this.GetComponent<Rigidbody2D>().velocity.y);
                yield return null;
            }
        }*/
    }

    #endregion

    // Not done
    
    #region Old Arm abilities 
    /*
    void armMovement()
    {
        if (Input.GetKeyDown(abilityKey) && !isGrappling)
        {
            if (player.horiLatestInput == -1)
            {
                usingLeft = true;
            }
            else
            {
                usingLeft = false;
            }
            grapplingAnchorPointObject = findClosestValidVine();
            if (grapplingAnchorPointObject == null)
            {
                return;
            }
            StartCoroutine(shootArmOut(grapplingAnchorPointObject));
        }
    }
    public GameObject findClosestValidVine()
    {
        // returns an array of the objects with layer vineColliders
        Collider2D[] vines = Physics2D.OverlapCircleAll(this.gameObject.transform.position, maxRange, vineColliders);
        // initialize at a 10000 as the game is not that big so shouldnt be an issue
        float min = maxRange + 1;
        int place = -1;
        int iteration = 0;
        foreach (Collider2D vine in vines)
        {
            // If the x position of the vine is less then the players and left arm is active we execute
            if (player.horiLatestInput == -1 && vine.transform.position.x - playerTransform.position.x < 0)
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
            else if (player.horiLatestInput == 1 && playerTransform.position.x > 0 && !isLeftArmActive)
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

    // The arm is two unity units long 
    IEnumerator shootArmOut(GameObject vinePosObj)
    {
        int armNum;
        if (usingLeft)
        {
            armNum = 0;
            toShoulderDist.x = Mathf.Abs(toShoulderDist.x) * -1;
        }
        else
        {
            armNum = 1;
            toShoulderDist.x = Mathf.Abs(toShoulderDist.x);
        }
        float renderDist = 0;
        bool breakLoop = false;
        do
        {
            // Finds the distance
            armScale += hookshotSpeed * Time.deltaTime;
            // Gets the angle between the player and the vine
            float theta = Mathf.Atan((playerTransform.position.y - vinePosObj.transform.position.y) 
            / (playerTransform.position.x - vinePosObj.transform.position.x)); 
            
			

            // grab distance between the two objects
            float distance = Vector2.Distance(vinePosObj.transform.position, playerTransform.position);
            if (renderDist >= distance)
            {
                renderDist = distance;
                breakLoop = true;
                LR.SetPosition(1, vinePosObj.transform.position); // Sets the end of the arm to the anchor point
            }
            yield return new WaitForEndOfFrame();
        }
        while(!breakLoop);
        // Shoot the arm out over a time interval
        // Todo if space is pressed here just cancel

        // Call Connecton
        StartCoroutine(armsConnected(vinePosObj));
        yield return null;
    }

     IEnumerator armsConnected(GameObject anchorObject)
     {
        do 
        {
            // add a y bias to counteract gravity
            float theta = Mathf.Atan((playerTransform.position.y - anchorObject.transform.position.y) 
            / (playerTransform.position.x - anchorObject.transform.position.x)); 
            player.rb.AddForce(new Vector2(grappleForce * Mathf.Cos(theta), grappleForce * Mathf.Sin(theta)));
        }
        while (Vector2.Distance(playerTransform.position, anchorObject.transform.position) < 2);
        yield return null;
     }
    /*void initializeArmValues()
    {

    }

    void armMovement()
    {
        if (Input.GetKeyDown(abilityKey) && !isConnected)
        {
            if (player.horiLatestInput == 1)
            {

                curShoulder = shoulders[1];
            }
            else
            {
                curShoulder = shoulders[0];
            }
            armMovementAbilityInput();
        }
    }
    private void armMovementAbilityInput()
    {
        isConnected = true;
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
        LR.enabled = true; // Line Renderer is enabled
        float distance = Vector2.Distance(vinePosObj.transform.position, curShoulder.shoulderPosition);
        // Gets rotation needed to point towards vinePosObj
        float renderDist = 0;
        bool breakLoop = false;
        do
        {
            // Finds the distance
            renderDist += hookshotSpeed * Time.deltaTime;
            // Gets the angle between the player and the vine
            float theta = Mathf.Atan((curShoulder.shoulderPosition.y - vinePosObj.transform.position.y) 
            / (curShoulder.shoulderPosition.x - vinePosObj.transform.position.x)); 
            // When we grab the grapple start position we can then expand the sprite to let the arm fly out
			LR.SetPosition(0, curShoulder.shoulderPosition); // Starts at grapple tip
			LR.SetPosition(1, new Vector2(renderDist * Mathf.Cos(theta), renderDist * Mathf.Sin(theta))); // Ends at the target
            if (renderDist >= distance)
            {
                renderDist = distance;
                breakLoop = true;
            }
            yield return null;
        }
        while(!breakLoop);
        // Shoot the arm out over a time interval
        // Todo if space is pressed here just cancel

        // Call Connecton
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
        float min = maxRange + 1;
        int place = -1;
        int iteration = 0;
        foreach (Collider2D vine in vines)
        {
            // If the x position of the vine is less then the players and left arm is active we execute
            if (curShoulder.isLeftShoulder && vine.transform.position.x - playerTransform.position.x < 0)
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
    */
    #endregion
    
    #region New Arm Abilities
    void armMovement()
    {
        if (Input.GetKeyDown(abilityKey))
        {
            if (player.horiLatestInput != 0 && !handShot)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W))
                {
                    shootVector = new Vector2(diagnolxShootForce * player.horiLatestInput, diagnolyShootForce);
                }
                else
                {
                    shootVector = new Vector2(horrizontalShootForce * player.horiLatestInput, 0);
                }
                // SHift the hand left or right to not spawn directly on the player
                Instantiate(hand, player.transform.position + new Vector3(1.2f * player.horiLatestInput, 0, 0), quaternion.identity);
            }
        }
    }

    public void armAnchored(Vector3 anchorPoint)
    {
        
    }
    #endregion
}
