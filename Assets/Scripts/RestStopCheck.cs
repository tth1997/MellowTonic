using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestStopCheck : MonoBehaviour
{

    public bool InRest;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RestStopTrigger")
        {
            InRest = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RestStopTrigger")
        {
            InRest = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
