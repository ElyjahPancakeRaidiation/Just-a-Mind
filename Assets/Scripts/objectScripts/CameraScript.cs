using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public bool firstCam;
    public bool secondCam;
    [Space(50)]
    private GameObject cameraObj;
    public static GameObject playerObj;
    public float cameraSpeed;

    [SerializeField]public bool isFollowingPlayer;
    [SerializeField]private Vector2 cameraOffset;
    private Vector2 camVel = Vector2.zero;//Keeps track of the cameras velocity


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


    }

    private void Update(){
        CameraShake();      
        

        if (isFollowingPlayer)
        {
            if(playerObj.GetComponent<Rigidbody2D>().velocity.x != 0){
                cameraOffset.x = playerObj.GetComponent<PlayerController>().horizontal;
            }
        }
    }
    private void FixedUpdate() {
        if (isFollowingPlayer)
        {
            if (firstCam)
            {
                FollowObjDelay(playerObj.transform, cameraSpeed);
            }
            
            if (secondCam)
            {
                FollowPlayer(cameraSpeed);
            }
        }
    }

    public void FollowObjDelay(Transform objTransform, float speed)//Follows any obj(Should always be put in fixed update so it can add the rigidbody. If it is not in F.U it will make anything with a rigidbody jitter when moved.)
    {
        Vector2 targetPosition = new Vector2(objTransform.position.x, objTransform.position.y);
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition + cameraOffset, ref camVel, speed);//With smoothDamp the lower the speed the faster it goes also itt's smoother than Lerp.
        //We can also cap the speed it can go at with smoothDamp

    }
    public void FollowPlayer(float speed){
        Vector2 targetPosition = new Vector2(playerObj.transform.position.x, playerObj.transform.position.y);
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition + cameraOffset, ref camVel, speed);
        if (playerObj.GetComponent<PlayerController>().horizontal == 0)
        {
            LowerNum(ref cameraOffset.x, 0, 2);
        }
    }

    private void LowerNum(ref float startingNum, float endNum, float decreasePower){
        if (startingNum > endNum)
        {
            startingNum -= decreasePower * Time.deltaTime;
        }

        if (startingNum < endNum)
        {
            startingNum = endNum;
        }
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
        }else{
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
