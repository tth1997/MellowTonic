using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class examplePhoneMsgScript : MonoBehaviour
{
    public Text exampleLHText;

    public float messageDelayTime
    {
        get
        {
            return 5f;
        }
    }

    string yeet = "bruh";

    List<string> currentDialogueLines;

    List<string> stop1DialogueLines;



    int lineNum;

    void Start()
    {

        stop1DialogueLines = new List<string>(new string[] 
        {
        "This will be the first block of dialogue. Blah blah blah. I'm just adding filler here to test that this text will actually wrap properly.",
        "Here's some more filler text. This should pop up " + messageDelayTime.ToString() + " seconds after the first bit.",
        "And here's a third bit. Hi, hello, how are you. I love refrigerator!"
        });


        currentDialogueLines = stop1DialogueLines;

        Debug.Log("Dialogue lines: " + currentDialogueLines.Count.ToString());

        StartCoroutine("DialogueCreaterCoroutine");
    }

    IEnumerator DialogueCreaterCoroutine()
    {

        while (lineNum < currentDialogueLines.Count)
        {
            yield return new WaitForSeconds(messageDelayTime);

            string currentDialogueText = exampleLHText.text;
            exampleLHText.text = currentDialogueText + "\n" + "\n" + currentDialogueLines[lineNum];
            lineNum++;

            //exampleText.rectTransform.anchoredPosition = new Vector2(exampleText_PosX, exampleText_PosY);

            if (lineNum == currentDialogueLines.Count)
            {
                StartCoroutine("DialogueScrollerCoroutine");

            }

            Debug.Log("Dialogue coroutine cycle complete");
        }
    }

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
}
