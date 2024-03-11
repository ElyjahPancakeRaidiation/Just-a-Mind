using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    Abilities abilityScript;
    public KeyCode formChangeKey;
    public KeyCode rightformChangeKey;
    [SerializeField]public bool devControl;//Just used to override the locked forms(I got really lazy and I dont want to keep going back and fourth changing the bools)
    public int neareastSpawner;
    public float timer;
    [SerializeField]public int maxTime; //Can adjust the time it takes for text to appear accordingly
    public float textOffsetX;
    public float textOffsetY;
    public Transform spherePoint;
    public GameManager gm;
    [SerializeField]private SpriteRenderer playerSpriteRender;
    [SerializeField]private Sprite[] playerFormSprite;
    private Animator anim;
    public float jumpTime;
    public AudioManagerScript AMS;

    private static bool playerDead;

    public Collider2D circleCol; // checks for all colliders
    public Collider2D vineCol;
    

    public GameObject spawner;
    public GameObject player;
    public GameObject grabOn;

    public TMP_Text guideText;
    public Image thoughtBub;
    [SerializeField] GameObject thoughtBubble;
    public IEnumerator thoughtBubbleTime;

    IEnumerator playingSound;
    private bool soundIsPlaying;

    #region movements
    [Header("Movement")]
    public bool canMove = true;
    [SerializeField]private bool isMoving;
    public Rigidbody2D rb;
    public float horizontal, vertical;
    public int horiLatestInput = 1, vertLatestInput = 0;
    public float speed,jumpSpeedX,jumpSpeedY;
    [SerializeField]private float bonusRotationSpeed;
    [SerializeField]private float rotChangePointMax;//The max amount rb rotation can get to before giving a boost when changing dir
    [SerializeField]private float rotChangePointMin;//The minimal amount rb rotation can get to before stoping the boost when changing dir
    private bool canBoostRotSpeed;

    [Header("Interaction")]
    private Collider2D interactCol;
    [SerializeField]public float interactRadius;
    [SerializeField]public LayerMask interactMask, groundMask;//interact mask is for objects you can interact with by pressing E. Ground is for ground
    
    
    [Header("Player Forms")]
    private int maxForm;
    public enum playerForms{Ball, Pogo, Arm}
    public static playerForms playerForm;
    // Change to false false false for game and initialize in gm
    public static bool[] playerPieces = {true, true, true};//bools for the player pieces {0: ball, 1: pogo, 2: arm}


    [Header("Physics")]
    public bool ignoreResistences = false;
    [SerializeField] float coefficientOfAirResistence, coefficientOfFriction;
    isGroundedScript groundedScript;

    #region Ball movement variables
    [Header("Head")]
    [SerializeField]private Collider2D ballCol;
    private const float DECELERATION = 8, ACCELERATION = 4, POINTTOACCELERATE = 2;
    /// <Deceleration acceleration and accelerate explantion> 
    /// Decleration is to increase the turning speed from left to right
    /// Acceleration is to increase the speed when going from still to moving
    /// pointToAccelerate when it should stop accelerating ex: it will keep acclerating from 0 to 2 and stop
    /// </summary>
    #endregion

    #region Pogo movement variables
    [Header("Body")]
    [SerializeField]private Collider2D pogoCol;
    public IEnumerator jumping;
    public bool canJump = true;
	#endregion
	#region Arm movement variables
	#endregion


	#endregion

	private void Start(){
        thoughtBub.enabled = false;
        abilityScript = GetComponent<Abilities>();
        if (!devControl)
        {
            playerPieces[0] = true;
            playerPieces[1] = false;
            playerPieces[2] = false;
        }
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundedScript = GameObject.Find("Ground Ray Object").GetComponent<isGroundedScript>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        //gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        playerForm = playerForms.Ball;
        //guideText.text = "";
        FormSettings();
        AMS = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        
    }
        
    // Update is called once per frame
    void Update()
    {
        ChangeForm();//Controlls the changing of the players form
        InteractFunc();//The player interacts through this function
        //ChangeVel();//The velocity for the ball my brain rots from this
        RespawnParse();

        if (canMove) {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            Movements();
        }

        LatestInput((int)horizontal, (int)vertical);
        
    }

    private void FixedUpdate() {

        if (canMove){
            interactCol = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask);

            if (!ignoreResistences)
            {
                if (groundedScript.isGrounded())
                {
                    Friction();
                }
                else
                {
                    AirResistance();
                }
            }
        }

        if (rb.velocity.x > 1.5f || rb.velocity.x < -1.5)//the game will register if it is moving when its past a certain speed
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        
    }

    private void LatestInput(int horizontalInput, int verticalInput){//Finds the latest input for vertical and horizontal
        if (horizontalInput != 0)
        {
            int i = horizontalInput;
            horiLatestInput = i;
        }
        else
        {
            horiLatestInput = 0;
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
            maxForm = 0;
            // Start at one because ball always true and its playerform zero so dont increase max form for that
            for (int i = 1; i < playerPieces.Length; i++)//runs through the bools and see what form is not active yet
            {
                if (playerPieces[i])
                {
                    maxForm++;
                    print(maxForm);
                    break;
                }
            }
    
            if (Input.GetKeyDown(formChangeKey) || Input.GetKeyDown(rightformChangeKey))
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
            if (Input.GetKeyDown(formChangeKey) || Input.GetKeyDown(rightformChangeKey))
            {
                playerForm++;
                if ((int)playerForm >= 2)
                {
                    playerForm = 0;
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
                    rb.mass = 1;
                    ballCol.enabled = true;
                    pogoCol.enabled = false;
                    if (pogoCol.enabled == false)
                    {
                        print("wait but this is working");
                    }
                    playerSpriteRender.sprite = playerFormSprite[0];
                    anim.enabled = false;
                    rb.freezeRotation = false;
                    Debug.Log("ball");
                    break;

                case playerForms.Pogo:
                rb.mass = 2.5f;
                    transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
                    ballCol.enabled = false;//changes the collider from ball to pogo
                    pogoCol.enabled = true;
                    //anim.enabled = true;
                    playerSpriteRender.sprite = playerFormSprite[1];//changes the sprites from ball to pogo man
                    rb.freezeRotation = true;
                    anim.SetInteger("Horizontal", (int)horizontal);//this is for walking animation 
                    Debug.Log("Head");
                    canJump = true; 
                    break;

                case playerForms.Arm:
                rb.mass = 3.5f;
                    //Where all of the settings for arm goes
                    ballCol.enabled = false;
                    pogoCol.enabled = true;
                    rb.freezeRotation = false;
                    /*foreach (Abilities.shoulderType shoulder in abilityScript.shoulders)
                    {
                        shoulder.shoulderObject.SetActive(true);
                    }*/
                    break;
            }
        }
        

    private void Movements()
    {//different movements for each form
		switch (playerForm)
		{
			case playerForms.Ball:
                rb.AddForce(new Vector2(horizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);//moves the player in the direction the player is pressing
                RotationSpeed();
                break;
			case playerForms.Pogo:
                if (horizontal != 0 && groundedScript.isGrounded() && canJump)
                {
                    canJump = false;
                    print("being called");
                    jumping = Jump();
                    StartCoroutine(jumping);
                }
                break;
			case playerForms.Arm:
				break;
			default:
				break;
		}

	}

    public IEnumerator Jump() 
    {
        Vector2 jumpForce = new Vector2(horizontal * jumpSpeedX, jumpSpeedY);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
		yield return new WaitUntil (() => groundedScript.isGrounded());
		canJump = true;
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

    void RespawnParse()
    {
        circleCol = Physics2D.OverlapCircle(spherePoint.transform.position, interactRadius, interactMask); //set circleCol to Overlap Cirlce
		if (circleCol != null)
		{

        }


        if (circleCol == spawner || circleCol == null) //if cirlce collider is equal or if circle collider is equal to null return
        {
            return; //ensure that that there's never a null in the spawner
        } 
        
        else if (circleCol != spawner && circleCol != null)
		{
            spawner = circleCol.gameObject; 
        }


    }

    private IEnumerator PlaySound(float waitAmount){//Plays the sound and waits until it is finished + however amount you want to add
        AMS.sfx.clip = AMS.currentSfx;
        AMS.sfx.Play();
        yield return new WaitForSeconds(AMS.sfx.clip.length + waitAmount);
        soundIsPlaying = false;
    }

    private void RotationSpeed(){

        bonusRotationSpeed = -(rb.angularVelocity/2);
        
        if (!groundedScript.isGrounded())
        {
            if(horizontal == 1){
                if (rb.angularVelocity > 0.02f)
                {
                    rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime;
                    print("Off ground and going right");
                }
            }else if(horizontal == -1){
                if (rb.angularVelocity < -0.02f)
                {
                    rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime;
                    print("Off ground and going right");
                }
            }
        }
        
        
        switch (horizontal)
        {
            case 1:

                if(canBoostRotSpeed){
                    if (rb.angularVelocity < rotChangePointMin){
                        rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity < -rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                    print("going Right at negative");
                }
                break;

            case -1:

                if (canBoostRotSpeed)
                {
                    if(rb.angularVelocity > rotChangePointMin){
                        rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity > rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                    print("going Left at positive");
                }
                break;
            
        }
    }


    private void OnDrawGizmos()  
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		switch (collision.gameObject.tag)
		{
            case "Spike":
                Debug.Log("dead");
                this.transform.position = spawner.transform.position;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0;
                playerDead = true;
                break;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Guide")
		{
			timer += Time.deltaTime;
			if (timer >= maxTime)
			{
                thoughtBub.enabled = true;
            }
			else
			{
                thoughtBub.enabled = false;
			}
		}
		if (collision.tag == "sfx")
		{
            if (!soundIsPlaying && isMoving)
            {
                playingSound = PlaySound(0.6f);
                StartCoroutine(playingSound);
                soundIsPlaying = true;
            }
            
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
        /*Destroy(thoughtBubble);
		guideText.text = "";*/
        if (collision.tag == "Guide")
        {
            thoughtBub.enabled = false;
            timer = 0;
        }

        if (collision.tag == "sfx")
        {
            StopCoroutine(playingSound);
            soundIsPlaying = false;
        }

	}


}