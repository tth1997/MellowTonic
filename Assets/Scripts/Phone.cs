using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    public AudioClip SoundToPlay;
    public float Volume;
    AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponentInParent<Rigidbody>().tag == "Player")
        {
            audio.PlayOneShot(SoundToPlay, Volume);
            Debug.Log("phone notification triggered");
        }

    }
}

