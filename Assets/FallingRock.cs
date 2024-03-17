using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField]private float fallingDelay;
    [SerializeField]private float fallingSpeed;
    [SerializeField]private GameObject rockObj;
    [SerializeField]private Vector2 stopPosition;//The rock stops falling past this point
    private bool isRockFalling;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private IEnumerator ActivateRock(){
        isRockFalling = true;
        //Do animation + particals
        yield return new WaitForSeconds(fallingDelay);



    } 
}
