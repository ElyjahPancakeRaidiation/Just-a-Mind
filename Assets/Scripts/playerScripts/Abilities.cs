using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Abilities : MonoBehaviour
{

    private PlayerController player;

    public KeyCode abilityKey;
   
    private int bonusCharges;//bonus charges for the abilities 

    #region Dashing variables

    [SerializeField]private float dashingDuration;//How long the dash will go for
    private const float DASHPOWER = 12f;
    public static bool isDashing;
    [SerializeField]private int maxDashAmount;
    private int dashAmount;
    
    #endregion

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
        player.rb.centerOfMass = COM;
        hJ = this.GetComponent<HingeJoint2D>();
        playerTransform = GetComponent<Transform>();
    }


    void Update()
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
        if (Input.GetKeyDown(abilityKey) && !isDashing)
        {
            if (dashAmount > 0 || bonusCharges > 0)
            {
                StartCoroutine(Dashing(dashingDuration));
            }
        }
    }
    
    private IEnumerator Dashing(float duration){//Will push the player forward for a certain amount of time at a certain amount of speed
        CameraScript.isCameraShaking = true;
        yield return new WaitForSeconds(0.3f);
        player.rb.velocity = Vector2.zero;
        CameraScript.isCameraShaking = false;
        player.rb.AddForce(new Vector2(player.horiLatestInput * DASHPOWER, player.vertLatestInput * DASHPOWER), ForceMode2D.Impulse);//dash in any direction boom kachow
        isDashing = true;
        yield return new WaitForSeconds(duration);
        if (bonusCharges <= 0){
            dashAmount--;
        }else{
            bonusCharges--;
        }
        ResetDash();
    }
    private void ResetDash(){//Resets all of the variables in dash mechanic
        player.canMove = true;
        isDashing = false;
    }
    #endregion
    
    #region Pogo abilities
    void Jumping()
    {

        bool isOnGround()
        {
            if (Physics2D.Raycast(groundPoint.position, Vector2.down, 1.5f, player.groundMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (Input.GetKey(abilityKey))
        {
            keyHoldDown += Time.deltaTime * 3f;
        }

        if (keyHoldDown >= 5)
        {
            canSuperJump = true;
            
        }

        if (isOnGround())
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

        if (!isOnGround() && bonusCharges > 0)//if player has bonus charges and presses space do a super jump but only while the player is not on the ground
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

    #region Arm abilities 

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

    #endregion
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Ground")
        {
            dashAmount = maxDashAmount;
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Ground")
        {
            dashAmount = maxDashAmount;
        }
    } 

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "BonusCrystal")
        {
            bonusCharges++;
            Destroy(other.gameObject);
        }
    }   
    
}
