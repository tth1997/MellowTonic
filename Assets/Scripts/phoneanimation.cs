using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phoneanimation : MonoBehaviour
{
    private Animation anim;
    [HideInInspector]
    public bool holdUp = false;

    PhoneDialogueManager phoneDialogueManager;

    void Start()
    {
        anim = GetComponent<Animation>();
        phoneDialogueManager = GetComponentInChildren<PhoneDialogueManager>();
    }

    private void Update()
    {

    }

    public void TogglePhone()
    {
        if (!holdUp)
        {
            // play phone animation 
            anim.Play("phone_pullUp");
            holdUp = true;
            Debug.Log("Phone was toggled up.");
        }
        else
        {
            // play pull down animation
            anim.Play("phone_pullDown");
            holdUp = false;
            Debug.Log("Phone was toggled down.");
        }
    }
}
