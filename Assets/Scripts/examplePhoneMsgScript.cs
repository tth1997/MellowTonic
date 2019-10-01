using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class examplePhoneMsgScript : MonoBehaviour
{
    public Text exampleLHText;

    public float messageDelayTime = 5f;

    public List<string> currentDialogueLines;

    public List<string> stop1DialogueLines;

    public List<string> stop2DialogueLines;

    public List<string> stop3DialogueLines;

    //public List<string> blankTest;

    int lineNum = 0;
    int restStopNum = 1;
    bool playingDialogue;

    void Start()
    {

        stop1DialogueLines = new List<string>(new string[] 
        {
            "Hey, just stopping for a rest. long day",
            "Here's some more filler text. This should pop up " + messageDelayTime.ToString() + " seconds after the first bit.",
        });

        stop2DialogueLines = new List<string>(new string[]
        {
            "Here is where Stop 2 dialogue starts. Girlfriend has dinner, etc. etc." + messageDelayTime.ToString() +
            "Pretty far... just really tired as well"
        });

        stop3DialogueLines = new List<string>(new string[]
        {
            "Hey. Get close I think. See soon." + messageDelayTime.ToString() + "Yep..." + messageDelayTime.ToString() + 
            "I'm not that for now" + messageDelayTime.ToString() + "Before we met? What was that like?" + messageDelayTime.ToString() + "Oh...",
            "Zack, is this what you expected?"
        });

        /*
        blankTest = new List<string>(new string[]
        {
        "",
        ""
        });
        */



        // Method 1 for adding strings to currentDialogueLines. This will take each individual 
        // string (i.e. each line of dialogue) in stop1DialogueLines and add it do currentDialogueLines.
        // This is useful if you want to ADD lines.
        /*
        foreach (string line in stop1DialogueLines)
        {
            currentDialogueLines.Add(line);
        }
        */





        // Method 2 for adding strings to currentDialogueLines. This will take ALL strings in
        // stop1DialogueLines and add them to currentDialogueLines. This will OVERWRITE any
        // strings in currentDialogueLines.
        currentDialogueLines = stop1DialogueLines;






        Debug.Log("Dialogue lines count: " + currentDialogueLines.Count.ToString());

        StartCoroutine("DialogueCreaterCoroutine");
    }

    void Update()
    {
        //MsgClearCheck();
    }

    IEnumerator DialogueCreaterCoroutine()
    {

        while (playingDialogue)
        {
            yield return new WaitForSeconds(messageDelayTime);
            if (currentDialogueLines.Count > lineNum)
            {
                exampleLHText.text += "\n" + "\n" + currentDialogueLines[lineNum];
                lineNum++;
            }
            else
            {
                restStopNum++;
                playingDialogue = false;
            }

            Debug.Log("Dialogue coroutine cycle complete.");
        }
    }

    // We might have use this if we want to carry dialogue over from previous rest stops.
    /*
    IEnumerator DialogueScrollerCoroutine()
    {
        int linesToScroll = currentDialogueLines.Count;

        while (linesToScroll > 0)
        {
            yield return new WaitForSeconds(1);
            exampleLHText.rectTransform.anchoredPosition = new Vector2(exampleLHText.rectTransform.anchoredPosition.x, exampleLHText.rectTransform.anchoredPosition.y + 0.01f);

            linesToScroll -= 1;

            Debug.Log("Dialogue scroller cycle complete");
            yield return linesToScroll;
        }
    }
    */
    
    // Use this function after interacting with a rest stop. It will cycle dialogue and unpause the DialogueCreatorCoroutine.
    public void MsgClearCheck()
    {
        currentDialogueLines.Clear();

        if (restStopNum == 2)
        {
            exampleLHText.text = null;
            currentDialogueLines = stop2DialogueLines;
            lineNum = 0;
            playingDialogue = true;

            Debug.Log("Stop 2 dialogue loaded.");
        }
        else if (restStopNum == 3)
        {
            // same thing but for Stop 3
        }





        /*
        GameObject playerCar = GameObject.Find("PlayerCar");
        TextMessageClear messageClear = playerCar.GetComponent<TextMessageClear>();
        
        if(messageClear.clearMessage == true)
        {
            currentDialogueLines = blankTest;
            Debug.Log("MESSSAGECLEAR");
        }
        //examplePhoneMsgScript phoneScript = playerCar.GetComponent<examplePhoneMsgScript>();
        */
    }
    
}
