using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class handScript : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    PlayerController controller;
    Abilities ability;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        controller = player.GetComponent<PlayerController>();
        ability = player.GetComponent<Abilities>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = player.GetComponent<Rigidbody2D>().velocity;
        print(ability.shootVector);
        rb.AddForce(ability.shootVector);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag != "Guide")
        {
            
        }
    }
}
