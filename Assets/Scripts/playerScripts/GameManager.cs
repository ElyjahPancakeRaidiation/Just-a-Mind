using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController pc;
    public GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		/*if (pc.devControl == true)
		{
            if (Input.GetKeyDown(KeyCode.R))
            {
                player.transform.position = pc.spawner.transform.position;
            }
        }*/
		
    }


}
