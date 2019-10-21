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
        dialogueCount = 1;
        dialogueScroll = GetComponent <Animation>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnimation()
    {

        dialogueScroll.Play("TextScrollAni" + dialogueCount.ToString());
        Debug.Log("Dialogue Animation: " + dialogueCount);

        dialogueCount++;
    }
}
