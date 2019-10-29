using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    Text tutorialText;
    Animator tutorialAnimator;

    GameObject playerCar;
    TutorialManager tutorialManager;
    public int _tutorialCount;
    

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GameObject.Find("PlayerCar");

        tutorialText = GetComponentInChildren<Text>();
        tutorialAnimator = GetComponent<Animator>();

        tutorialManager = playerCar.GetComponent<TutorialManager>();
        _tutorialCount = tutorialManager.tutorialCount;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCount();
        DisplayText();
    }

    public void TutorialFadeIn()
    {
        tutorialAnimator.SetBool("BoolFade", true);
    }
    public void TutorialFadeOut()
    {
        tutorialAnimator.SetBool("BoolFade", false);
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
            tutorialText.text = "Use 'Space Bar' to brake.";

        }

        if (_tutorialCount == 4)
        {
            tutorialText.text = "Hold 'Right Mouse Button' to free look without steering.";
        }

        if (_tutorialCount == 5)
        {
            tutorialText.text = "Press 'R' to toggle the radio.";
        }

        if (_tutorialCount == 6)
        {
            tutorialText.text = "Rest stops will lower your fatigue.";
        }

        if (_tutorialCount == 7)
        {
            tutorialText.text = "Press 'Tab' to pause the game.";
        }
    }
}
