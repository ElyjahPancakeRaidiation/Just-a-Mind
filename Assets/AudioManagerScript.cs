using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource[] audioSourceArray;
    public AudioClip[] AudioClips;

    public AudioClip currentClip;

    public float musicDir;
    public double endTime;

    public int audioToggle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   /* public void OnPlayMusic() 
    {
        endTime = AudioSettings.dspTime + 0.5;
        audioSourceArray.clip = currentClip;
        
    }*/
}
