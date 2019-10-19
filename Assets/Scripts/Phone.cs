using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    public AudioClip SoundToPlay;
    public float Volume;
    AudioSource audio;
    public bool alreadyPlayed = false;
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!alreadyPlayed && col.GetComponentInParent<Rigidbody>().tag == "Player")
        {
            audio.PlayOneShot(SoundToPlay, Volume);
            alreadyPlayed = true;
            Debug.Log("phone notification triggered");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        alreadyPlayed = false;
        Debug.Log("notification trigger reset");
    }
}

