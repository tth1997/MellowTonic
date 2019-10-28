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
        textBox.GetComponent<Text>().text = "*Phone Ringing*";
        yield return new WaitForSeconds(2);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2);
        textBox.GetComponent<Text>().text = "*Phone Ringing*";
        yield return new WaitForSeconds(3);
        textBox.GetComponent<Text>().text = "Hey just letting you know that i've fueled up and finally left the office";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "OK cool, long day then. How long till you get home?";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "Yeah shattered but glad it's over. I just looked and GPS says around 30 minutes";
        yield return new WaitForSeconds(6);
        textBox.GetComponent<Text>().text = "There's no traffic at this time of the night";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "Alright sweet, will you be hungry when you get in? Do you want me to reheat dinner?";
        yield return new WaitForSeconds(5);
        textBox.GetComponent<Text>().text = "Nah probably just relax a little bit then sleep I reckon";
        yield return new WaitForSeconds(4);
        textBox.GetComponent<Text>().text = "Ok drive safe, see you soon";
        yield return new WaitForSeconds(3);
        textBox.GetComponent<Text>().text = "See you in a bit";
        yield return new WaitForSeconds(2);
        textBox.GetComponent<Text>().text = "*Phone hangs up*";
    }
}
