using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    Camera cam;
    CameraScript camController;
    [SerializeField]private float camBorderRight, camBorderLeft;
    public static bool atBorder;

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
    }

    // Update is called once per frame
    void Update()
    {
        //Overall Script is to check if the border is between two points of the screen. If it is it will stop the camera from moving until it is not. 

        float borderCamX = cam.WorldToViewportPoint(transform.position).x;
        //Uncomment this to see the objects position in the screen if you don't fully understand.
        //print(borderCamX);

        
        if (borderCamX <= camBorderRight && borderCamX >= camBorderRight - 0.18f)
        {
            atBorder = true;
            if (CameraScript.playerObj.transform.position.x > cam.gameObject.transform.position.x)
            {
                //right
                camController.isFollowingPlayer = false;
            }
            else if(CameraScript.playerObj.transform.position.x < cam.gameObject.transform.position.x )
            {
                //left
                camController.isFollowingPlayer = true;
            }
        }else if(borderCamX >= camBorderLeft && borderCamX <= camBorderLeft + 0.08f){
            atBorder = true;
            if (CameraScript.playerObj.transform.position.x > cam.gameObject.transform.position.x)
            {
                //right
                camController.isFollowingPlayer = true;
            }
            else if(CameraScript.playerObj.transform.position.x < cam.gameObject.transform.position.x )
            {
                //left
                camController.isFollowingPlayer = false;
            }
        }else{
            atBorder = false;
        }
        

        
        
    }
}
