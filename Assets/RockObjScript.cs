using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObjScript : MonoBehaviour
{
    private Animator anim;
    [SerializeField]private Transform startPos;
    [SerializeField]private float respawnDelayTimer;
    [SerializeField]private FallingRock fallingRockScript;
    public bool hasRockFallen;

    [SerializeField]private Collider2D bodyCol, groundCol;

    private void Start() {
        anim = GetComponent<Animator>();
        
    }

    private void Update() => Physics2D.IgnoreCollision(bodyCol, groundCol);
    private void FixedUpdate() => Physics2D.IgnoreCollision(bodyCol, groundCol);

    private IEnumerator Respawn(){
        yield return new WaitForSeconds(respawnDelayTimer);
        anim.SetBool("Idle", false);
        anim.SetTrigger("Disassemble");
        yield return new WaitForSeconds(fallingRockScript.rockAnimClip[1].length);
        transform.position = startPos.position;
        hasRockFallen = false;

        anim.ResetTrigger("Disassemble");
        anim.SetTrigger("Respawn");
        yield return new WaitForSeconds(fallingRockScript.rockAnimClip[2].length);
        anim.ResetTrigger("Respawn");
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Respawn());
        }
    }
}
