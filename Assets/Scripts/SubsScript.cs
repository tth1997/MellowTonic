using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubsScript : MonoBehaviour {

    public GameObject textBox;

    void Start() {
        StartCoroutine(TheSequence());

    }

    IEnumerator TheSequence()
    {
        yield return new WaitForSeconds(2);
        textBox.GetComponent<Text>().text = "Phone Ringing";
        yield return new WaitForSeconds(2);
        textBox.GetComponent<Text>().text = "Phone Ringing";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "Hey Just letting you know that i've fueled up and finally left the office";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "OK cool, long day then. How long till you get home?";
    }
}
