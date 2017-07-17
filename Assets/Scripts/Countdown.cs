using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    // Displayed once the countdown finishes
    public string startString = "GO!";

    private TextMesh text;

    private float shrinkSteps = 60f;

    public delegate void CountdownEvent();

    public void StartCountdown(int delay, CountdownEvent function) {
        StartCoroutine(DisplayCountdown(delay, function));
    }

    private IEnumerator DisplayCountdown(int delay, CountdownEvent function) {
        text = GetComponent<TextMesh>();
        for(int i = 0; i <= delay; i++) {
            text.text = StringForCount(delay - i);
            yield return ShrinkOverTime(1f);
        }
        text.text = "";
        function();
    }

    private IEnumerator ShrinkOverTime(float time) {
        int startSize = text.fontSize;
        float threshold = shrinkSteps / 3f;

        for(int i = 0; i < shrinkSteps; i ++) {
            if(i > threshold) {
                text.fontSize = (int)Mathf.Lerp(startSize, 0, (i - threshold) / (shrinkSteps -  threshold));
            }
            yield return new WaitForSeconds(time / shrinkSteps);
        }
        text.fontSize = startSize;
    }

    private string StringForCount(int count) {
        if(count > 0) {
            return count + "";
        } else {
            return startString;
        }
    }
}
