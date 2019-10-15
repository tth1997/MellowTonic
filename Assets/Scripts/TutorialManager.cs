using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool usingTutorial;
    public int tutorialCount;
    [HideInInspector]
    public GameObject tutorial;

    // Start is called before the first frame update
    void Start()
    {
        
        tutorialCount = 0;

        tutorial = GameObject.Find("Tutorial");
        tutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TutorialTrigger")
        {
            tutorial.SetActive(true);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TutorialTrigger")
        {
            tutorial.SetActive(false);
            tutorialCount++;
            Debug.Log(tutorialCount);
        }
    }
}
