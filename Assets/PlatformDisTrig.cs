using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisTrig : MonoBehaviour
{
    public GameObject platform;
    public float breakTimer;
	public bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        platform.SetActive(true);
    }

	private void Update()
	{
		Debug.Log(isPlayer);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			breakTimer -= Time.deltaTime;
			isPlayer = true;
		}

		/*if (breakTimer <= 0)
		{
			platform.SetActive(false);
		}*/
	}
}
