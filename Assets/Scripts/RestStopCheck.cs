using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestStopCheck : MonoBehaviour
{
    GameObject playerCar;
    CarMovementScript carMovementScript;

    public bool inRest;
    private Animation phoneRestAnim;
    //public float inputDelayTimeRS;


    // Start is called before the first frame update
    void Start()
    {
        playerCar = GameObject.Find("PlayerCar");
        carMovementScript = playerCar.GetComponent<CarMovementScript>();

        phoneRestAnim = GetComponent<Animation>();

    }

    // Update is called once per frame
    void Update()
    {
        if(inRest == true)
        {
            
            
            carMovementScript.inputDelayTime = 0;
            carMovementScript.currentSwayMagnitude = 0;
            carMovementScript.currentFocusDistance = .5f;
            carMovementScript.blackoutTimer = Time.time + 180;
            carMovementScript.blackoutBaseDuration = .1f;
            phoneRestAnim.Play("phone_reststop1");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "RestStopTrigger")
        {
            inRest = true;
            Debug.Log("RestStop");

            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RestStopTrigger")
        {
            inRest = false;
        }
    }
}
