using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{

    public UnityEvent onMoveStart;
    public UnityEvent onMoveStop;

    public AudioSource audioSource;
    public AudioClip[] movingClips;

    public int soundIndex = 0;

    public PlayerController pc;
    public isGroundedScript gs;

    public GameObject player;
    public GameObject groundRay;

    /*public Rigidbody2D rb;*/

    public bool isMoving = false;
    /*public bool isMoveSoundPlaying;*/

    public IEnumerator walkingSounds;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
        groundRay = GameObject.Find("Ground Ray Object");
        gs = groundRay.GetComponent<isGroundedScript>();
        audioSource = GetComponent<AudioSource>();
        /*rb = player.GetComponent<Rigidbody2D>();*/
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!isMoving && gs.isGrounded() && PlayerController.playerForm == PlayerController.playerForms.Ball && pc.rb.velocity != Vector2.zero)
		{
            StartMoving();
		}
		else if(isMoving || !gs.isGrounded() || PlayerController.playerForm != PlayerController.playerForms.Ball || pc.rb.velocity == Vector2.zero) 
        {
            StopMoving();
        }
        
    }

    void StartMoving() 
    {
        isMoving = true;
        onMoveStart.Invoke();

		if (movingClips.Length > 0)
		{
            walkingSounds = playMovingSounds();
            StartCoroutine(walkingSounds);
		}

        soundIndex = 0;
    }

    void StopMoving() 
    {
        isMoving = false;
        onMoveStop.Invoke();

		if (walkingSounds != null)
		{
            StopCoroutine(walkingSounds);
		}
    }

    IEnumerator playMovingSounds() 
    {
        isMoving = true;

        for (int i = 0; i < movingClips.Length; i++)
        {
            audioSource.PlayOneShot(movingClips[i]);
            yield return new WaitForSeconds(movingClips[i].length);
        }

        isMoving = false;

		
    }
}
