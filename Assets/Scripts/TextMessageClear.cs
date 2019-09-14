using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessageClear : MonoBehaviour
{

    public bool clearMessage;
    public GameObject PhoneMessage;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        /*
        if(clearMessage == true)
        {
            GameObject playerCar = GameObject.Find("PlayerCar");
            examplePhoneMsgScript phoneScript = playerCar.GetComponent<examplePhoneMsgScript>();
            phoneScript.currentDialogueLines = phoneScript.blankTest;
        }
        */
        
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "MsgClear")
        {
            clearMessage = true;
            //PhoneMessage.SetActive(false);

            Debug.Log("MsgClear");


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MsgClear")
        {
            clearMessage = false;
            //PhoneMessage.SetActive(true);
        }
    }
}
