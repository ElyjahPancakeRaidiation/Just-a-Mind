using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public Vector3 jumpForce;
    public Vector3 superJumpForce;
    public Transform groundPoint;
    public bool canSuperJump = false;
    public float keyHoldDown;//Amount the jump is held down

    #endregion

    #region Arm variables
    
    public float speed;
    public float dashMag;
    public float shiftTime = 0;
    public float shiftTimeMax;
    public Transform playerTransform;
    [SerializeField] private Vector3 COM;
    [Space(10)]

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

    #endregion

    private void Start() {
        player = GetComponent<PlayerController>();
        dashAmount = maxDashAmount;
        //player.rb.centerOfMass = COM;
        hJ = this.GetComponent<HingeJoint2D>();
        playerTransform = GetComponent<Transform>();
        groundedScript = GameObject.Find("Ground Ray Object").GetComponent<isGroundedScript>();
    }


    void Update()
    {

    }

    void FixedUpdate() 
    {
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
                armMovementAbilityInput();
                break;
        }
    }

    #region Ball abilites
    private void Dash(){
        if (Input.GetKeyDown(abilityKey) && !isDashing && groundedScript.isGrounded())
        {
            if (dashAmount > 0 || bonusCharges > 0)
            {
                StartCoroutine(Dashing(dashingDuration));
            }
        }
    }
    
    private IEnumerator Dashing(float duration){//Will push the player forward for a certain amount of time at a certain amount of speed
        // Starts camera shacking
        CameraScript.isCameraShaking = true;
        // We then add the dash force
        player.rb.AddForce(new Vector2(player.horiLatestInput * DASHPOWER, yDashModifier * player.vertLatestInput * DASHPOWER), ForceMode2D.Impulse);//dash in any direction boom kachow
        isDashing = true;        
        // wait then turn off cammera shake
        yield return new WaitForSeconds(duration);
        CameraScript.isCameraShaking = false;
        yield return new WaitUntil(() => groundedScript.isGrounded());
        yield return new WaitForSeconds(dashDelay);
        ResetDash();
    }
    private void ResetDash(){//Resets all of the variables in dash mechanic
        isDashing = false;
    }
    #endregion
    
    #region Pogo abilities
    void Jumping()
    {

        if (Input.GetKey(abilityKey))
        {
            keyHoldDown += Time.deltaTime * 3f;
        }

        if (keyHoldDown >= 5)
        {
            canSuperJump = true;
            
        }

        if (groundedScript.isGrounded())
        {
            if (Input.GetKeyUp(abilityKey))
            {
                if (!canSuperJump)//if its a regular jump use regular jump force
                {
                    player.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    canSuperJump = false;
                    keyHoldDown = 0;
                }
    
                if (canSuperJump)//if its a super jump use super jump force
                {
                    player.rb.AddForce(Vector2.up * superJumpForce, ForceMode2D.Impulse);
                    canSuperJump = false;
                    keyHoldDown = 0;
                }
            }
        }

        if (!groundedScript.isGrounded() && bonusCharges > 0)//if player has bonus charges and presses space do a super jump but only while the player is not on the ground
        {
            if (Input.GetKeyDown(abilityKey))
            {
                player.rb.AddForce(Vector2.up * superJumpForce, ForceMode2D.Impulse);
                canSuperJump = false;
                keyHoldDown = 0;
                bonusCharges--;
            }
        }

        Debug.DrawRay(groundPoint.position, Vector2.down * 1.5f, Color.red);
    }

    #endregion

    // Not done
    #region Arm abilities 

    void armMovement()
    {
        if (Input.GetKeyDown(abilityKey) && !isConnected)
        {
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
        // Finds the distance
        float distance = Vector2.Distance(vinePosObj.transform.position, playerTransform.position);
        // Gets rotation needed to point towards vinePosObj
        float rotation = Mathf.Rad2Deg * Mathf.Atan((playerTransform.position.y - vinePosObj.transform.position.y) 
        / (playerTransform.position.x - vinePosObj.transform.position.x)); 
        curArm.transform.eulerAngles = new Vector3(curArm.transform.rotation.x, curArm.transform.rotation.y, rotation);
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

    #endregion
    
}
