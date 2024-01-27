using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject cameraObj;
    private Camera cam;
    public static GameObject playerObj;
    private Rigidbody2D playerRB;
    public float cameraSpeed;
    [SerializeField]private float zoomBackSpeed, zoomBackSpeedFaster;//How fast the camera goes back to following the player
    private bool isComingBack;
    private bool goingRight, goingUp;//If player is going left, right will be false if player is going down, up will be false

    [SerializeField]public bool isFollowingPlayer;
    [SerializeField]private Vector2 cameraOffset;
    private Vector2 startCamOffset; 
    //public float yIncreaser;

    public static bool isCameraShaking, isCameraZooming;
    [SerializeField, Range(0, 3)]private float maxCameraShakePosX, maxCameraShakePosY;
    public struct cameraDefualt
    {
        public float camPosX, camPosY;
        public float camFOV;//Called orthographic size in code 
    }
    public cameraDefualt camDefaultValues;//I wanted to play around with structs.
    

    private void Start() {
        cam = GetComponentInChildren<Camera>();
        camDefaultValues.camPosX = 0;
        camDefaultValues.camPosY = 0;
        camDefaultValues.camFOV = 8;
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerRB = playerObj.GetComponent<Rigidbody2D>();
        startCamOffset = cameraOffset;


    }

    private void Update(){
        if (isFollowingPlayer)
        {
            if(cam.orthographicSize != camDefaultValues.camFOV){
                isComingBack = true;
                if (isComingBack)
                {
                    ZoomBackToPlayer();
                }
            }
        }
        CameraShake();      
    }
    private void FixedUpdate() {
        if (isFollowingPlayer)
        {
            if (cam.orthographicSize == camDefaultValues.camFOV)
            {
                isComingBack = false;
                if (!isComingBack)
                {
                    FollowObjDelay(cameraSpeed, playerObj.transform);
                }
            }
        }
    }

    public void FollowObjDelay(float speed, Transform followObj)//Follows any obj(Should always be put in fixed update so it can add the rigidbody. If it is not in F.U it will make anything with a rigidbody jitter when moved.)
    {
        print("Following obj");
        transform.position = Vector2.Lerp(transform.position, (Vector2)followObj.position + cameraOffset, speed);
        GoingToFast(40, 40);

    }

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

    public void ZoomBackToPlayer(){
        print("Zooming back");
        float dist = Vector2.Distance(playerObj.transform.position, transform.position);
        if (dist < 2)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)playerObj.transform.position + cameraOffset, zoomBackSpeed);
        }else if(dist > 2){
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)playerObj.transform.position + cameraOffset, zoomBackSpeedFaster);
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
            float posX = Random.Range(0, maxCameraShakePosX) + transform.position.x;
            float posY = Random.Range(0, maxCameraShakePosY) + transform.position.y;
            cameraObj.transform.position = new Vector3(posX, posY, -10);
        }
        else
        {
            cameraObj.transform.localPosition = new Vector3(camDefaultValues.camPosX, camDefaultValues.camPosY, -10);
        }
    }


}
