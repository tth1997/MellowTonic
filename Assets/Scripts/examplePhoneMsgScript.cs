using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class examplePhoneMsgScript : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollAmount;

    Animation scrollAnim;

    public Text LHText;
    public Text RHText;

    public float messageDelayTime = 5f;
    public List<string> currentDialogueLinesLH;
    public List<string> currentDialogueLinesRH;

    public List<string> gasStationDialogueLinesLH;
    
    public List<string> gasStationDialogueLinesRH;
    [HideInInspector]
    public List<string> stop1DialogueLinesLH;
    [HideInInspector]
    public List<string> stop1DialogueLinesRH;
    [HideInInspector]
    public List<string> stop2DialogueLinesLH;
    [HideInInspector]
    public List<string> stop2DialogueLinesRH;
    [HideInInspector]
    public List<string> stop3DialogueLinesLH;
    [HideInInspector]
    public List<string> stop3DialogueLinesRH;

    //public List<string> blankTest;

    int lineNum = 0;
    int restStopNum = 1;
    bool playingDialogue;

    void Start()
    {
        scrollAnim = GetComponent<Animation>();



        gasStationDialogueLinesLH = new List<string>(new string[]
        {
            ""
        });
        gasStationDialogueLinesRH = new List<string>(new string[]
        {
            ""
        });
        //
        stop1DialogueLinesLH = new List<string>(new string[] 
        {
            ""
        });
        stop1DialogueLinesRH = new List<string>(new string[]
        {
            ""
        });
        //
        stop2DialogueLinesLH = new List<string>(new string[]
        {
            ""
        });
        stop2DialogueLinesRH = new List<string>(new string[]
        {
            ""
        });
        //
        stop3DialogueLinesLH = new List<string>(new string[]
        {
            ""
        });
        stop3DialogueLinesRH = new List<string>(new string[]
        {
            ""
        });

        // Method 2 for adding strings to currentDialogueLines. This will take ALL strings in
        // stop1DialogueLines and add them to currentDialogueLines. This will OVERWRITE any
        // strings in currentDialogueLines.
        currentDialogueLinesLH = gasStationDialogueLinesLH;
        currentDialogueLinesRH = gasStationDialogueLinesRH;

        StartCoroutine("DialogueCreaterCoroutine");
    }

    void Update()
    {
        scrollRect.verticalNormalizedPosition = scrollAmount;

        ScrollAnimate();
    }

    IEnumerator DialogueCreaterCoroutine()
    {

        while (playingDialogue)
        {
            yield return new WaitForSeconds(messageDelayTime);
            if (currentDialogueLinesLH.Count > lineNum)
            {
                LHText.text += "\n" + "\n" + currentDialogueLinesLH[lineNum];
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
    IEnumerator DialogueScrollerCoroutine()
    {
        int linesToScroll = currentDialogueLinesLH.Count;

        while (linesToScroll > 0)
        {
            yield return new WaitForSeconds(1);
            LHText.rectTransform.anchoredPosition = new Vector2(LHText.rectTransform.anchoredPosition.x, LHText.rectTransform.anchoredPosition.y + 0.01f);

            linesToScroll -= 1;

            Debug.Log("Dialogue scroller cycle complete");
            yield return linesToScroll;
        }
    }
    
    // Use this function after interacting with a rest stop. It will cycle dialogue and unpause the DialogueCreatorCoroutine.
    public void MsgClearCheck()
    {
        currentDialogueLinesLH.Clear();

        if (restStopNum == 2)
        {
            LHText.text = null;
            currentDialogueLinesLH = stop2DialogueLinesLH;
            lineNum = 0;
            playingDialogue = true;

            Debug.Log("Stop 2 dialogue loaded.");
        }
        else if (restStopNum == 3)
        {
            // same thing but for Stop 3
        }
    }

    void ScrollAnimate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (scrollAnim != null)
            {
                scrollAnim.Play();
            }
        }
    }
}
