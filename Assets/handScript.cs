using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor.Callbacks;
using UnityEngine;

public class handScript : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    PlayerController controller;
    Abilities ability;
    HingeJoint2D hj;
    bool vineInRange;
    public bool isLeftHand;
    public int leftRightID;
    GameObject connectedVine;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        controller = player.GetComponent<PlayerController>();
        ability = player.GetComponent<Abilities>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = player.GetComponent<Rigidbody2D>().velocity;
        hj = GetComponent<HingeJoint2D>();
        
        //print(ability.shootVector);
        //rb.AddForce(ability.shootVector);
        if (!isLeftHand)
        {
            leftRightID = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (vineInRange && Input.GetKeyDown(ability.handKeys[leftRightID]))
        {
            hj.enabled = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "vine")
        {
            vineInRange = true;
            connectedVine = other.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "vine")
        {
            vineInRange = false;
        }
    }
}
