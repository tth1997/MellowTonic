using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool usingTutorial;
    public int tutorialCount;
    [HideInInspector]
    public TutorialText tutorialTextScript;

    // Start is called before the first frame update
    void Start()
    {
        
        tutorialCount = -1;

        tutorialTextScript = GameObject.Find("Tutorial").GetComponent<TutorialText>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TutorialTrigger")
        {
            tutorialTextScript.TutorialFadeIn();
            tutorialCount++;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TutorialTrigger")
        {
            tutorialTextScript.TutorialFadeOut();
            Debug.Log("Tutorial counter: " + tutorialCount);
        }
    }
}
