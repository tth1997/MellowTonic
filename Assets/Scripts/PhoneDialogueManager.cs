using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialogueManager : MonoBehaviour
{
    private Animation dialogueScroll;
    public int dialogueCount;

    phoneanimation phoneAnimation;

    [HideInInspector]
    public bool isPlayingAnim = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogueCount = 1;
        dialogueScroll = GetComponent <Animation>();
        phoneAnimation = GetComponentInParent<phoneanimation>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnimation()
    {
        isPlayingAnim = true;
        StartCoroutine(PullUpPhoneCoroutine());

        Debug.Log("Dialogue Animation: " + dialogueCount);


    }




    IEnumerator PullUpPhoneCoroutine()
    {
        phoneAnimation.TogglePhone();

        yield return new WaitForSeconds(2f);

        dialogueScroll.Play("TextScrollAni" + dialogueCount.ToString());

        while (dialogueScroll.isPlaying)
        {
            yield return null;
        }
        dialogueCount++;
        isPlayingAnim = false;

        phoneAnimation.TogglePhone();
    }
}
