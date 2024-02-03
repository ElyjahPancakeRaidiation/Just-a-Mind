using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FORNOWGAMEMANAGER : MonoBehaviour
{

    private float camRadius;
    [SerializeField]private Vector2 camVec;
    private Collider2D camHitCol;
    [SerializeField]private LayerMask playerMask;

    GameObject player;
    [SerializeField]Transform playerTP;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (camHitCol != null)
        {
            player.transform.position = playerTP.position;
        }
    }

    private void FixedUpdate() => camHitCol = Physics2D.OverlapBox(transform.position, camVec, camRadius, playerMask);

    private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, camVec);
}
