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

    public PlayerController pc;
    public isGroundedScript gs;

    public GameObject player;
    public GameObject groundRay;

    /*public Rigidbody2D rb;*/

    public bool isMoving = false;
	public bool isMoveSoundPlaying;

	public IEnumerator walkingSounds;

    bool musicIsPlaying;

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
		if (gs.isGrounded() && PlayerController.playerForm == PlayerController.playerForms.Ball && Mathf.Abs(pc.rb.velocity.x) > .2f)
		{
            StartMoving(); 
		}
		else if(!gs.isGrounded() || PlayerController.playerForm != PlayerController.playerForms.Ball || Mathf.Abs(pc.rb.velocity.x) <= .2f) 
        {
            StopMoving();
        }
        if (pc.rb.velocity.x < 2)
        {
            audioSource.pitch = pc.rb.velocity.x / 2;
        }
        else{
            audioSource.pitch = 1;
        }
    }

    void StartMoving() 
    {
		isMoving = true;
		onMoveStart.Invoke();

		if (movingClips.Length > 0 && !musicIsPlaying)
		{
            walkingSounds = playMovingSounds();
            StartCoroutine(walkingSounds);
		}
    }

    void StopMoving() 
    {
		isMoving = false;
		onMoveStop.Invoke();

		if (walkingSounds != null)
		{
            StopCoroutine(walkingSounds);
            musicIsPlaying = false;
		}
    }

    IEnumerator playMovingSounds() 
    {
        Debug.Log("called");
        isMoving = true;
        musicIsPlaying = true;
        for (int i = 0; i < movingClips.Length; i++)
        {
            audioSource.PlayOneShot(movingClips[i]);
            yield return new WaitForSeconds(movingClips[i].length);
        }
        musicIsPlaying = false;
        isMoving = false;

		
    }
}
