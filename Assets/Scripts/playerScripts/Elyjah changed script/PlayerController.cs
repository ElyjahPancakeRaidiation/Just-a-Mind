using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode formChangeKey;
    [SerializeField]private bool devControl;//Just used to override the locked forms(I got really lazy and I dont want to keep going back and fourth changing the bools)

    #region movements
    public bool canMove = true;
    public Rigidbody2D rb;
    private float horizontal, vertical;
    public int horiLatestInput = 1, vertLatestInput = 0;
    public float speed;

    [SerializeField]private SpriteRenderer playerSpriteRender;
    [SerializeField]private Sprite[] playerFormSprite;
    private Animator anim;

    private Collider2D interactCol;
    [SerializeField]private float interactRadius;
    [SerializeField]public LayerMask interactMask, groundMask;//interact mask is for objects you can interact with by pressing E. Ground is for ground.

    public enum playerForms{Ball, Pogo, Arm}
    public static playerForms playerForm;
    public static bool[] playerPieces = {true, true, true};//bools for the player pieces {0: ball, 1: pogo, 2: arm}
    private int maxForm;
    [SerializeField] float coefficientOfAirResistence, coefficientOfFriction;

    isGroundedScript groundedScript;

    #region Pogo movement variables
    [SerializeField]private Collider2D pogoCol;
    #endregion

    #region Ball movement variables
    [SerializeField]private Collider2D ballCol;
    private const float MAXSPEED = 6;
    public float maxSpeedCopy;
    private const float DECELERATION = 8, ACCELERATION = 4, POINTTOACCELERATE = 2;
    /// <Deceleration acceleration and accelerate explantion> 
    /// Decleration is to increase the turning speed from left to right
    /// Acceleration is to increase the speed when going from still to moving
    /// pointToAccelerate when it should stop accelerating ex: it will keep acclerating from 0 to 2 and stop
    /// </summary>
    #endregion
    
    #region Arm movement variables
    public float dashMag;

    #endregion

    #endregion


    private void Start(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundedScript = GameObject.Find("Ground Ray Object").GetComponent<isGroundedScript>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        FormSettings();
    }
        
    // Update is called once per frame
    void Update()
    {
        ChangeForm();//Controlls the changing of the players form
        InteractFunc();//The player interacts through this function
        //ChangeVel();//The velocity for the ball my brain rots from this

        if (canMove) {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        LatestInput((int)horizontal, (int)vertical);
        
    }

    private void FixedUpdate() {

        if (canMove){
            Movements();
            Debug.Log("Can Move is true");
            if (groundedScript.isGrounded())
            {
                Friction();
                Debug.Log("Friction");
            }
            else
            {
                Debug.Log("Air Res");
                AirResistance();
            }
            interactCol = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask);

        }
    }

    private void LatestInput(int horizontalInput, int verticalInput){//Finds the latest input for vertical and horizontal
        if (horizontalInput != 0)
        {
            int i = horizontalInput;
            horiLatestInput = i;
        }

        if (verticalInput != 0)
        {
            int i = verticalInput;
            vertLatestInput = i;
        }
        else
        {
            vertLatestInput = 0;
        }
    }

    /*private void ChangeVel(){//This is for the ball movement
        if (horizontal == -1)
        {
            if (rb.velocity.x > 0)//if the player is going right decelerate towards the left
            {
                rb.velocity = new Vector2(rb.velocity.x - DECELERATION * Time.deltaTime, rb.velocity.y);
            }
            else if(rb.velocity.x <= 0 && rb.velocity.x > -POINTTOACCELERATE)//when the player is moving from 0 it will boost the player in order to get it moving at top speed(towards left)
            {
                rb.velocity = new Vector2(rb.velocity.x - ACCELERATION * Time.deltaTime, rb.velocity.y);
            }
        }
        else if(horizontal == 1)
        {
            if (rb.velocity.x < 0)//if the player is going left decelerate towards the right
            {
                rb.velocity = new Vector2(rb.velocity.x + DECELERATION * Time.deltaTime, rb.velocity.y);
            }
            else if(rb.velocity.x >= 0 && rb.velocity.x < POINTTOACCELERATE)//when the player is moving from 0 it will boost the player in order to get it moving at top speed(towards right)
            {
                rb.velocity = new Vector2(rb.velocity.x + ACCELERATION * Time.deltaTime, rb.velocity.y);
            }
        }
    }*/
    
    private void InteractFunc()
    {//To interact with objects in the world
        if (interactCol != null){
            if (Input.GetKeyDown(KeyCode.E))
            {                    
                interactCol.GetComponent<IInteractable>().Interact();
            }
        }
    }

    private void ChangeForm()
    {
        
        if (!devControl)
        {
            for (int i = 0; i < playerPieces.Length; i++)//runs through the bools and see what form is not active yet
            {
                if (!playerPieces[i])
                {
                    maxForm = i;
                    break;
                }
    
                maxForm = i;
            }
    
            if (Input.GetKeyDown(formChangeKey))
            {
                
                if ((int)playerForm >= maxForm)
                {
                    playerForm = 0;
                }
                else
                {
                    playerForm++;
                }
                FormSettings();//main change
            }
        }
        else
        {
            if (Input.GetKeyDown(formChangeKey))
            {
                
                if ((int)playerForm >= 3)
                {
                    playerForm = 0;
                }
                else
                {
                    playerForm++;
                }
                FormSettings();//main change
            }
        }

    }
    void FormSettings(){//defualt settings for each form(mainly for the sprites of each form)
            switch (playerForm)
            {
                case playerForms.Ball:
                    //Sets the balls sprite, unfreezes rotation, and changes the animation
                    ballCol.enabled = true;
                    pogoCol.enabled = false;
                    playerSpriteRender.sprite = playerFormSprite[0];
                    anim.enabled = false;
                    rb.freezeRotation = false;
                    Debug.Log("ball");
                    break;

                case playerForms.Pogo:
                    transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
                    ballCol.enabled = false;//changes the collider from ball to pogo
                    pogoCol.enabled = true;
                    anim.enabled = true;
                    playerSpriteRender.sprite = playerFormSprite[1];//changes the sprites from ball to pogo man
                    rb.freezeRotation = true;
                    anim.SetInteger("Horizontal", (int)horizontal);//this is for walking animation 
                    Debug.Log("Head");
                    break;

                case playerForms.Arm:
                    //Where all of the settings for arm goes
                    ballCol.enabled = false;
                    pogoCol.enabled = true;
                    rb.freezeRotation = false;
                    break;
            }
        }
        

    private void Movements()
    {//different movements for each form
        rb.AddForce(new Vector2(horizontal * speed * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);//moves the player in the direction the player is pressing

    }
    private void OnDrawGizmos()  
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    void AirResistance()
    {
        // Air resistance opposes motion
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfAirResistence * (rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfAirResistence * (rb.velocity.y * rb.velocity.y)));
    }
    void Friction()
    {
        // Air resistance opposes motion but in ball motion is reversed because rotation
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(rb.velocity.y * rb.velocity.y)));
    }
}