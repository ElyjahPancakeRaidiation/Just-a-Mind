using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisTrig : MonoBehaviour
{
	public GameObject platform;
	public float breakTimer;
	public AudioManagerScript ams;

	private void Start()
	{
		ams = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			breakTimer -= Time.deltaTime;
			ams.currentSfx = ams.soundFX[0];
			ams.sfx.Play();
		}

		if (breakTimer <= 0)
		{
			Destroy(platform);
		}

		
	}
}
