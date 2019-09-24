using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestStopCheck : MonoBehaviour
{

    public bool inRest;
    private Animation phoneRestAnim;
    //public float inputDelayTimeRS;


    // Start is called before the first frame update
    void Start()
    {

        phoneRestAnim = GetComponent<Animation>();

    }

    // Update is called once per frame
    void Update()
    {
        if(inRest == true)
        {
            GameObject playerCar = GameObject.Find("PlayerCar");
            CarMovementScript carMovementScript = playerCar.GetComponent<CarMovementScript>();
            carMovementScript.inputDelayTime = 0;
            carMovementScript.currentSwayMagnitude = 0;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "RestStopTrigger")
        {
            inRest = true;
            phoneRestAnim.Play("phone_reststop1");
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
