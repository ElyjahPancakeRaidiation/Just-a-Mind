using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("----Cam Movement Variables----")]
    private GameObject cameraObj;
    public static GameObject playerObj;
    private Camera cam;
    public float cameraSpeed, transitionSpeed;
    [SerializeField]private float zoomBackSpeed, Distance;//How fast the camera goes back to following the player and how close before switching movements to follow the player
    public bool isFollowingPlayer, isComingBack, isTransitioning;



    [Header("----Cam Control Variables----")]
    public bool notFollowingX, notFollowingY;
    public static bool isCameraShaking, isCameraZooming;
    [SerializeField, Range(0, 3)]private float shakeAmount;
    private Vector2 origCamPos;
    public struct cameraDefualt
    {
        public float camFOV;//Called orthographic size in code 
    }
    public cameraDefualt camDefaultValues;//I wanted to play around with structs.

    
    //private bool goingRight, goingUp;//If player is going left, right will be false if player is going down, up will be false
    //[SerializeField]private Vector2 cameraOffset;
    //private Vector2 startCamOffset; 

    private void Start() {
        cam = GetComponentInChildren<Camera>();
        camDefaultValues.camFOV = 8;
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        //playerRB = playerObj.GetComponent<Rigidbody2D>();
        //startCamOffset = cameraOffset;
    }

    private void OnEnable(){
        //cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        origCamPos = cameraObj.transform.position;
    }

    private void Update(){
        CameraShake();    
        if (isFollowingPlayer)
        {
            if (isComingBack)
            {
                FollowBackToPlayer(cameraSpeed, playerObj.transform);
            }
        }
    }
    private void FixedUpdate() {
        if (isFollowingPlayer)
        {
            if (!isComingBack)
            {
                FollowBackToPlayer(cameraSpeed, playerObj.transform);
            }
        }
    }

    public void FollowObjDelay(float speed, Transform followObj)//Follows any obj(Should always be put in fixed update so it can add the rigidbody. If it is not in F.U it will make anything with a rigidbody jitter when moved.)
    {
        transform.position = Vector2.Lerp(transform.position, (Vector2)followObj.position , speed);
        //GoingToFast(40, 40);

    }

    public void FollowBackToPlayer(float speed, Transform followObj){

        float playerDist = Vector2.Distance(playerObj.transform.position, transform.position);
        if (!notFollowingX && !notFollowingY)
        {
            if (playerDist < Distance)
            {
                isComingBack = false;
            }
        }else if(notFollowingX || notFollowingY){
            if (playerDist < 2f)
            {
                isComingBack = false;
            }
        }

        if (notFollowingX && notFollowingY)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }
        
        if (!notFollowingX || !notFollowingY)//This is used to make sure it doesn't interfere with the both of the axis being stopped.
        {
            if (notFollowingX)
            {
                float Y = (!isComingBack) ? Mathf.Lerp(transform.position.y, playerObj.transform.position.y, speed):
                Mathf.MoveTowards(transform.position.y, playerObj.transform.position.y, 20 * Time.deltaTime);//Only moves on the Y axis and switches smoothly when the camera is coming back to the player
                
                transform.position = new Vector2(transform.position.x, Y);
                //print("Not following X");
            }

            if (notFollowingY)
            {
                float X = (!isComingBack) ? Mathf.Lerp(transform.position.x, playerObj.transform.position.x, speed):
                Mathf.MoveTowards(transform.position.x, playerObj.transform.position.x, 20 * Time.deltaTime);////Only moves on the X axis and switches smoothly when the camera is coming back to the player

                transform.position = new Vector2(X, transform.position.y);
                //print("Not following Y");
            }
        }
        
        if (!notFollowingX && !notFollowingY)
        {
            transform.position = (isComingBack) ? Vector2.MoveTowards(transform.position, (Vector2)followObj.transform.position, zoomBackSpeed * Time.deltaTime):
            Vector2.Lerp(transform.position, (Vector2)followObj.position, speed);//Moves in all directions and smoothly comes back to the player
        }

    }

    public void ZoomCameraChange(float FOV, float zoomSpeed){//Zooms back and fourth wether it is the player or not. Never make the desired FOV smaller than the defualt FOV which is 5
        if (!isFollowingPlayer)
        {
            if (cam.orthographicSize < FOV)
            {
                cam.orthographicSize += Time.deltaTime * zoomSpeed;
            }else{
                cam.orthographicSize = FOV;
            }
        }
        else
        {
            if (cam.orthographicSize > camDefaultValues.camFOV)
            {
                cam.orthographicSize -= Time.deltaTime * zoomSpeed;
            }else{
                cam.orthographicSize = camDefaultValues.camFOV;
            }
        }
    }
    
    private void CameraShake()//Shake camera.
    {
    
        if (isCameraShaking){
            //cameraObj.transform.localPosition = origCamPos + Random.insideUnitCircle * shakeAmount;
            //cameraObj.transform.localPosition = new Vector3(cameraObj.transform.localPosition.x, cameraObj.transform.localPosition.y, -10f);
        }
        else
        {
            //cameraObj.transform.localPosition = (Vector2)origCamPos;
            //cameraObj.transform.localPosition = new Vector3(cameraObj.transform.localPosition.x, cameraObj.transform.localPosition.y, -10f);
        }
        
    }

    public void TransitionWithPlayer(Transform transitionCamEndPos){
        if (transform.position == transitionCamEndPos.position)
        {
            isTransitioning = false;
        }

        if (isTransitioning)
        {
            isComingBack = false;
            isFollowingPlayer = false;
            transform.position = Vector2.MoveTowards(transform.position, transitionCamEndPos.position, transitionSpeed * Time.deltaTime);
        }
    }



    /*
    private void GoingToFast(float maxLimitX, float maxLimitY){
        //Should increase the offset in the direction its going to fast in.
        if (playerRB.velocity.x > maxLimitX)
        {
            print("Going Right");
            goingRight = true;
            cameraOffset.x += Time.deltaTime / playerRB.velocity.x;
            
        }else if(playerRB.velocity.x < -maxLimitX){
            print("Going Left");
            goingRight = false;
            cameraOffset.x += Time.deltaTime / playerRB.velocity.x;
        }

        if (goingRight && playerRB.velocity.x < maxLimitX)
        {
            if (cameraOffset.x > startCamOffset.x)
            {
                cameraOffset.x -= Time.deltaTime / playerRB.velocity.x;
            }
            else
            {
                cameraOffset.x = startCamOffset.x;
            }
        }else if(!goingRight && playerRB.velocity.x > -maxLimitX){
            if (cameraOffset.x < startCamOffset.x)
            {
                cameraOffset.x -= Time.deltaTime / playerRB.velocity.x;
            }
            else
            {
                cameraOffset.x = startCamOffset.x;
            }
        }

    }
    */

}
