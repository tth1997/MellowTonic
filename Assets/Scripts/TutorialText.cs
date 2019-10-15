﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    public Text tutorialText;
    GameObject playerCar;
    TutorialManager tutorialManager;
    public int _tutorialCount;
    

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GameObject.Find("PlayerCar");

        tutorialManager = playerCar.GetComponent<TutorialManager>();
        _tutorialCount = tutorialManager.tutorialCount;

        


    }

    // Update is called once per frame
    void Update()
    {
        CheckCount();
        DisplayText();
    }

    void CheckCount()
    {
        _tutorialCount = tutorialManager.tutorialCount;
    }

    void DisplayText()
    {
        if (_tutorialCount == 0)
        {
            tutorialText.text = "Use W and S keys to change car speed.";
        }

        if(_tutorialCount == 1)
        {
            tutorialText.text = "You do not have to hold the keys down to maintain speed.";
            
        }

        if (_tutorialCount == 2)
        {
            tutorialText.text = "Use mouse to steer the car.";

        }

        if (_tutorialCount == 3)
        {
            tutorialText.text = "Use 'space bar' to brake.";

        }

        if (_tutorialCount == 4)
        {
            tutorialText.text = "Holding 'Right Click' will allow free look movement.";

        }

        if (_tutorialCount == 5)
        {
            tutorialText.text = "Pressing 'Q' will bring up phone";
        }
    }
}
