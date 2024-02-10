using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    Camera cam;
    CameraScript camController;
    [SerializeField]private bool xAxisBorder, isTransitionActivator;//if true it is an X border. If false it is an Y border.
    
    [Header("----Transition Variable----")]
    [SerializeField]private Transform transitionCamEndPos;
    [SerializeField]private float zoomCameraAmount, zoomCameraSpeed;


    [Header("----Cam collider----")]
    private Collider2D camCol;
    private float camRadius;
    [SerializeField]private Transform colTransform;
    [SerializeField]private Vector2 camVec;
    [SerializeField]private LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        try
        {
            camController = GameObject.Find("MainCameraHolder").GetComponent<CameraScript>();
        }
        catch (System.Exception)
        {
            Debug.LogError("The cameras holder is not named MainCameraHolder or something else broke");
            throw;
        }
        if(transitionCamEndPos == null){return;}//Return if theres nothing set as a transform
    }

    // Update is called once per frame
    void Update()
    {
        StopCamAxis();
        /*
        //Overall Script is to check if the border is between two points of the screen. If it is it will stop the camera from moving until it is not. 
        if (camCol != null)
        {
            if (!isTransitionActivator)
            {
                if (!CamController.activeController)
                {
                    StopCamAxis();
                }
            }
            else
            {
                isTransitionActivator = true;
                camController.isTransitioning = true;
            }
        }

        if (isTransitionActivator)
        {
            if (camController.isTransitioning)
            {
                camController.TransitionWithPlayer(transitionCamEndPos);
                camController.ZoomCameraChange(zoomCameraAmount, zoomCameraSpeed);
            }
        }
        */
    }

    private void FixedUpdate() => camCol = Physics2D.OverlapBox(colTransform.position, camVec, camRadius, playerMask);    
    private void OnDrawGizmos() => Gizmos.DrawWireCube(colTransform.position, camVec);

    private void StopCamAxis(){
        Vector2 borderCamVec = cam.WorldToViewportPoint(transform.position);
        /*
        Vector2 toFarCol = (Vector2)cam.WorldToViewportPoint(borderCamVec) - (Vector2)cam.WorldToViewportPoint(new Vector2(0.99f, 0));
        print("How far the border is" + toFarCol);
        Vector2 toFarSubtract = (Vector2)cam.WorldToViewportPoint(borderCamVec) - toFarCol;
        print("The border at the correct position" + toFarSubtract);
        this.transform.position = toFarSubtract;
        */

        cam.gameObject.transform.position = (Vector2)cam.WorldToViewportPoint(new Vector2(0.99f, 0.46f));

        //Uncomment this to see the objects position in the screen if you don't fully understand.
        //print(borderCamVec);

        if (xAxisBorder)
        {
            if (borderCamVec.x <= 0.99f && borderCamVec.x >= 0.56f)//Right Side
            {
                if (CameraScript.playerObj.transform.position.x > cam.gameObject.transform.position.x)
                {
                    //if the player is going right
                    camController.notFollowingX = true;
                }
                else if(CameraScript.playerObj.transform.position.x < cam.gameObject.transform.position.x )
                {
                    //if the player is going left
                    camController.notFollowingX = false;
                }
            }
            else if(borderCamVec.x >= 0.01f && borderCamVec.x <= 0.46f){//Left Side
                if (CameraScript.playerObj.transform.position.x > cam.gameObject.transform.position.x)
                {
                    //right
                    camController.notFollowingX = false;
                }
                else if(CameraScript.playerObj.transform.position.x < cam.gameObject.transform.position.x)
                {
                    //left
                    camController.notFollowingX = true;
                }
            }
            
        }
        else{
            if (borderCamVec.y <= 0.98f && borderCamVec.y >= 0.90f)//Up Side
            {
                if (CameraScript.playerObj.transform.position.y > cam.gameObject.transform.position.y)
                {
                    //if the player is going up
                    camController.notFollowingY = true;
                }
                else if(CameraScript.playerObj.transform.position.y < cam.gameObject.transform.position.y)
                {
                    //if the player is going down
                    camController.notFollowingY = false;
                    camController.isComingBack = true;
                }
            }
            else if(borderCamVec.y >= 0.01f && borderCamVec.y <= 0.05f){//Down Side
                if (CameraScript.playerObj.transform.position.y > cam.gameObject.transform.position.y)
                {
                    //if the player is going up
                    camController.notFollowingY = false;
                    camController.isComingBack = true;
                }
                else if(CameraScript.playerObj.transform.position.y < cam.gameObject.transform.position.y)
                {
                    //if the player is going down
                    camController.notFollowingY = true;
                }
            }
            else{
                camController.notFollowingY = false;
            }
        }
        
    }

}
