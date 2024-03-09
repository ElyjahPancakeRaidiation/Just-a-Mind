using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

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
    private bool inBorder;


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
        
        //Overall Script is to check if the border is between two points of the screen. If it is it will stop the camera from moving until it is not. 
        if (camCol != null)
        {
            inBorder = true;
            if (inBorder)
            {
                StopCamAxis();
            }
            
        }else{
            inBorder = false;
            
        }
        
    }

    private void FixedUpdate() => camCol = Physics2D.OverlapBox(colTransform.position, camVec, camRadius, playerMask);    
    private void OnDrawGizmosSelected() => Gizmos.DrawWireCube(colTransform.position, camVec);

    private void StopCamAxis(){
        Vector2 borderCamVec = cam.WorldToViewportPoint(transform.position);//The border cam but in viewport position or the cameras point of view
        //Uncomment this to see the objects position in the screen if you don't fully understand.
        //print(borderCamVec);
        
        if (xAxisBorder)
        {
            if (borderCamVec.x <= 0.99f && borderCamVec.x >= 0.56f)//Right Side
            {
                
                if (CameraScript.playerObj.transform.position.x >= camController.gameObject.transform.position.x)
                {
                    if (borderCamVec.x <= 0.90f && borderCamVec.x >= 0.56f)//Checks if the border is past the pos it should be
                    {
                        camController.notFollowingX = true;   
                        float edgeOfCameraX = cam.ViewportToWorldPoint(new Vector3(0.98f, 0, 0)).x;//Puts the edge of the camera into a world position
                        float correctPosX = edgeOfCameraX - cam.ViewportToWorldPoint(new Vector3(borderCamVec.x, 0, 0)).x;//Converting the border cameras X back to world position
                        camController.gameObject.transform.position = new Vector3(camController.gameObject.transform.position.x - (correctPosX), camController.gameObject.transform.position.y, camController.gameObject.transform.position.z);
                        //In summary this entire section gets the correct position on where the camera needs to be if the border is closer to the camera than intended.                
                    }
                    else{
                        camController.notFollowingX = true;
                    }
                }
                else{
                    camController.notFollowingX = false;
                }
            }
            else if(borderCamVec.x >= 0.01f && borderCamVec.x <= 0.46f)//Left Side
            {
                if (CameraScript.playerObj.transform.position.x <= camController.gameObject.transform.position.x)
                {
                    if (borderCamVec.x >= 0.09f && borderCamVec.x <= 0.46f)//Checks if the border is past the pos it should be
                    {
                        
                        camController.notFollowingX = true;
                        float edgeOfCameraX = cam.ViewportToWorldPoint(new Vector3(0.02f, 0, 0)).x;//Puts the edge of the camera into a world position
                        float correctPosX = edgeOfCameraX - cam.ViewportToWorldPoint(new Vector3(borderCamVec.x, 0, 0)).x;//Converting the border cameras X back to world position
                        camController.gameObject.transform.position = new Vector3(camController.gameObject.transform.position.x - (correctPosX), camController.gameObject.transform.position.y, camController.gameObject.transform.position.z);
                        //In summary this entire section gets the correct position on where the camera needs to be if the border is closer to the camera than intended.                
                    }
                    else{
                        camController.notFollowingX = true;
                    }
                }
                else{
                    camController.notFollowingX = false;
                }

            }
            else{
                camController.notFollowingX = false;
            }
            
        }
        else{//For Y axis
            if (borderCamVec.y <= 0.99f && borderCamVec.y >= 0.56f)//Up Side
            {
                if (CameraScript.playerObj.transform.position.y >= camController.gameObject.transform.position.y)
                {
                    if (borderCamVec.y <= 0.90f && borderCamVec.y >= 0.56f)//Checks if the border is past the pos it should be
                    {
                        camController.notFollowingY = true;
                        float edgeOfCameraY = cam.ViewportToWorldPoint(new Vector3(0, 0.98f, 0)).y;//Puts the edge of the camera into a world position
                        float correctPosY = edgeOfCameraY - cam.ViewportToWorldPoint(new Vector3(0, borderCamVec.y, 0)).y;//Converting the border cameras Y back to world position
                        camController.gameObject.transform.position = new Vector3(camController.gameObject.transform.position.x, camController.gameObject.transform.position.y - (correctPosY), camController.gameObject.transform.position.z);
                        //In summary this entire section gets the correct position on where the camera needs to be if the border is closer to the camera than intended.                
                    }
                    else{
                        camController.notFollowingY = true;
                    }
                }
                else{
                    camController.notFollowingY = false;
                }
            }
            else if(borderCamVec.y >= 0.01f && borderCamVec.y <= 0.46f){//Down Side
                if (CameraScript.playerObj.transform.position.y <= camController.gameObject.transform.position.y)
                {
                    if (borderCamVec.y >= 0.09f && borderCamVec.y <= 0.46f)//Checks if the border is past the pos it should be
                    {
                        camController.notFollowingY = true;
                        float edgeOfCameraY = cam.ViewportToWorldPoint(new Vector3(0, 0.02f, 0)).y;//Puts the edge of the camera into a world position
                        float correctPosY = edgeOfCameraY - cam.ViewportToWorldPoint(new Vector3(0, borderCamVec.y, 0)).y;//Converting the border cameras Y back to world position
                        camController.gameObject.transform.position = new Vector3(camController.gameObject.transform.position.x, camController.gameObject.transform.position.y - (correctPosY), camController.gameObject.transform.position.z);
                        //In summary this entire section gets the correct position on where the camera needs to be if the border is closer to the camera than intended.                
                    }
                    else{
                        camController.notFollowingY = true;
                    }
                }
                else{
                    camController.notFollowingY = false;
                }
            }
            else{
                camController.notFollowingY = false;
            }
        }
    }

}
