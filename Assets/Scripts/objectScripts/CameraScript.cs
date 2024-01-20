using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject cameraObj;
    public static GameObject playerObj;
    private Rigidbody2D playerRB;
    public float cameraSpeed;

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
        public float camFOV;
    }
    public cameraDefualt camDefaultValues;//I wanted to play around with structs.
    

    private void Start() {
        camDefaultValues.camPosX = 0;
        camDefaultValues.camPosY = 0;
        camDefaultValues.camFOV = 5;
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerRB = playerObj.GetComponent<Rigidbody2D>();
        startCamOffset = cameraOffset;


    }

    private void Update(){
        CameraShake();      
        
    }
    private void FixedUpdate() {
        if (isFollowingPlayer)
        {
            FollowObjDelay(cameraSpeed);
        }
    }

    public void FollowObjDelay(float speed)//Follows any obj(Should always be put in fixed update so it can add the rigidbody. If it is not in F.U it will make anything with a rigidbody jitter when moved.)
    {
        transform.position = Vector2.Lerp(transform.position, (Vector2)playerObj.transform.position + cameraOffset, speed);
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
        /*
        if (playerRB.velocity.y > maxLimitY)
        {
            print("Going Up");
            cameraOffset.y += Time.deltaTime / playerRB.velocity.y;
            goingUp = true;
        }else if(playerRB.velocity.y < -maxLimitY){
            print("Going Down");
            goingUp = false;
            cameraOffset.y += -(Time.deltaTime * yIncreaser);
        }
        */

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

        /*
        if (goingUp && playerRB.velocity.y < maxLimitY)
        {
            if (cameraOffset.y > startCamOffset.y)
            {
                cameraOffset.y -= Time.deltaTime / playerRB.velocity.y;
            }
            else
            {
                cameraOffset.y = startCamOffset.y;
            }
        }else if(!goingUp && playerRB.velocity.y > -maxLimitY){
            if (cameraOffset.y < startCamOffset.y)
            {
                cameraOffset.y -= Time.deltaTime / playerRB.velocity.y;
            }
            else
            {
                cameraOffset.y = startCamOffset.y;
            }
        }
        */

    }

    public void ZoomCameraChange(float FOV, float zoomSpeed){//Zooms back and fourth wether it is the player or not. Never make the desired FOV smaller than the defualt FOV which is 5

        if (!isFollowingPlayer)
        {
            if (cameraObj.GetComponent<Camera>().orthographicSize < FOV)
            {
                cameraObj.GetComponent<Camera>().orthographicSize += Time.deltaTime * zoomSpeed;
            }else{
                cameraObj.GetComponent<Camera>().orthographicSize = FOV;
            }
        }
        else
        {
            if (cameraObj.GetComponent<Camera>().orthographicSize > camDefaultValues.camFOV)
            {
                cameraObj.GetComponent<Camera>().orthographicSize -= Time.deltaTime * zoomSpeed;
            }else{
                cameraObj.GetComponent<Camera>().orthographicSize = camDefaultValues.camFOV;
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
