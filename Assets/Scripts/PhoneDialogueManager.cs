using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialogueManager : MonoBehaviour
{
    private Animation dialogueScroll;
    public int dialogueCount;

    // Start is called before the first frame update
    void Start()
    {
        dialogueCount = 0;
        dialogueScroll = GetComponent <Animation>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RestStopTrigger")
        {
            PlayAnimation();
            Debug.Log("Dialogue Animation"+ dialogueCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RestStopTrigger")
        {
            dialogueCount++;
        }
    }

    void PlayAnimation()
    {
        if (dialogueCount == 1)
        {
            dialogueScroll.Play("TextScrollAni1");
        }

        if (dialogueCount == 2)
        {
            dialogueScroll.Play("TextScrollAni2");
        }

        if (dialogueCount == 3)
        {
            dialogueScroll.Play("TextScrollAni3");
        }
    }
}
