using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phoneanimation : MonoBehaviour
{
    private Animation anim;
    public bool holdUp;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        TogglePhone();
    }

    public void TogglePhone()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (!holdUp)
            {
                // play phone animation 
                anim.Play("phone_pullUp");
                holdUp = true;
            }

            else
            {
                // play pull down animation
                anim.Play("phone_pullDown");
                holdUp = false;
            }
        }
    }
}
