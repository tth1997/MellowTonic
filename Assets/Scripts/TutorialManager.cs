using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool tutorial;
    public int tutorialCount;
    public GameObject tutorialText;

    // Start is called before the first frame update
    void Start()
    {
        
        tutorialCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TutorialTrigger")
        {
            tutorialText.SetActive(true);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TutorialTrigger")
        {
            tutorialText.SetActive(false);
            tutorialCount++;
            Debug.Log(tutorialCount);
        }
    }
}
