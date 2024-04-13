using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    //Settings for the Rock falling and respawning
    [SerializeField]private GameObject[] rockObj;
    [SerializeField]private Rigidbody2D[] rockRb;
    [SerializeField]private Transform[] rockStartPos;
    [SerializeField]private bool[] hasRockFallen;
    private bool isRockFalling;
    [SerializeField]private float rockFallDelay, rockShakingTime, respawnTimer, rockRespawnTime;
    private float secDelay;

    //Raycast settings
    [SerializeField]private RaycastHit2D[] rockCast;
    [SerializeField]private float[] rockCastDistance;
    [SerializeField]private LayerMask rockMask;

    //Animations
    [SerializeField]private Animator[] rockAnims;
    [SerializeField]private AnimationClip[] rockAnimClip;//Shaking, Disassemble, Respawn {0, 1, 2}



    private void Start() {

        for (int i = 0; i < rockObj.Length; i++)//Makes sure all of the rocks have not fallen.
        {
            rockRb[i] = rockObj[i].GetComponent<Rigidbody2D>();
            rockAnims[i] = rockObj[i].GetComponent<Animator>();
            rockObj[i].transform.position = (Vector2)rockStartPos[i].position;
            hasRockFallen[i] = false;
        }

        rockCast = new RaycastHit2D[rockObj.Length];//Adds the amount of ray cast needed for each rock
    }


    private void Update()
    {

        for (int i = 0; i < rockObj.Length; i++)//Constantly changes it's status wether it has fallen or not
        {
            if (!isRockFalling)
            {
                rockRb[i].bodyType = RigidbodyType2D.Static;
                rockAnims[i].SetBool("Idle", true);
                //rockAnims[i].enabled = false;
            }else{
                //rockAnims[i].enabled = true;
            }

            if (rockCast[i].collider == null)
            {
                rockRb[i].bodyType = RigidbodyType2D.Static;
                hasRockFallen[i] = true;
            }else{
                hasRockFallen[i] = false;
            }
        }
    }

    private IEnumerator RockFalling(){
        
        isRockFalling = true;
        secDelay = 0;

        foreach (Animator item in rockAnims)
        {
            item.SetBool("Idle", false);
            item.SetBool("Shaking", true);
        }

        yield return new WaitForSeconds(rockShakingTime);//Shake

        for (int i = 0; i < hasRockFallen.Length; i++)
        {//Goes through each rock and checks to see if it has already fallen before turning its body type to dynamic

            //do Animation for the rock to start falling
            yield return new WaitForSeconds(rockFallDelay);//Extra delay
            rockAnims[i].SetBool("Shaking", false);
            rockAnims[i].SetBool("Idle", true);
            if (!hasRockFallen[i])
            {
                rockRb[i].bodyType = RigidbodyType2D.Dynamic;
                rockRb[i].gravityScale = 1;
            }
        }
        secDelay = 0;
        yield return new WaitUntil(() => allRocksFallen());
        //Waits until all of the rocks have fallen before moving on to respawning
        yield return new WaitForSeconds(respawnTimer);

        for (int i = 0; i < hasRockFallen.Length; i++)
        {
            rockRespawnTime = rockAnimClip[1].length;
            //do animation of rock going back to it's original position
            rockAnims[i].SetBool("Idle", false);
            rockAnims[i].SetTrigger("Disassemble");
            yield return new WaitForSeconds(rockRespawnTime - secDelay);//wait for animation to stop
            if (hasRockFallen[i])
            {
                rockAnims[i].ResetTrigger("Disassemble");
                rockObj[i].transform.position = (Vector2)rockStartPos[i].position;//Resets the rocks position
                rockRb[i].bodyType = RigidbodyType2D.Static;

            }

            rockAnims[i].SetTrigger("Respawn");
            rockRespawnTime = rockAnimClip[2].length;
            yield return new WaitForSeconds(rockRespawnTime);

            rockAnims[i].ResetTrigger("Respawn");
            
            //secDelay += 0.9f;
        }

        secDelay = 0;
        isRockFalling = false;
    }

    private IEnumerator RestartRocks(float time){
        yield return new WaitForSeconds(time);
        StartCoroutine(RockFalling());
    }

    private bool allRocksFallen(){//Checks if all of the rocks have fallen
        for (int i = 0; i < hasRockFallen.Length; i++)
        {
            if (!hasRockFallen[i])
            {
                return false;
            }
        }

        return true;
    }

    private void FixedUpdate()
    {
        
        for (int i = 0; i < rockObj.Length; i++)
        {
            //Goes through each raycastHit variable in the array turning them into usable ray cast for its own specific rock
            rockCast[i] = Physics2D.Raycast(rockStartPos[i].transform.position, Vector2.down, rockCastDistance[i], rockMask);
        }
        

    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < rockObj.Length; i++)
        {
            //Makes each raycast show in the inspector
            Gizmos.DrawRay(rockStartPos[i].transform.position, Vector2.down * rockCastDistance[i]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isRockFalling && !allRocksFallen())
            {
                //Starts the entire process of a rock falling and respawning
                StartCoroutine(RockFalling());
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isRockFalling)
        {
            print("Restarting");
            StartCoroutine(RestartRocks(1.5f));
        }
    }

}