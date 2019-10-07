using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSAudio : MonoBehaviour
{
    public AudioSource GPSSource;

    // Start is called before the first frame update
    void Start()
    {
        GPSSource = GetComponent<AudioSource> ();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerCar")
        {

            GPSSource.Play();
        }
    }
}
