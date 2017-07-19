using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    // Displayed once the countdown finishes
    public string startString = "GO!";

    private float tickDuration = 0.8f;

    public AudioClip tickSound;
    public AudioClip goSound;

    private AudioSource tickSource;
    private AudioSource goSource;

    private TextMesh text;

    private float shrinkSteps = 60f;

    public delegate void CountdownEvent();

    public void StartCountdown(int delay, CountdownEvent function) {
        StartCoroutine(DisplayCountdown(delay, function));
    }

    private void Setup() {
        text = GetComponent<TextMesh>();

        if(tickSound != null) {
            tickSource = gameObject.AddComponent<AudioSource>();
            tickSource.clip = tickSound;
            tickSource.playOnAwake = false;
        }
        if(goSound != null) {
            goSource = gameObject.AddComponent<AudioSource>();
            goSource.clip = goSound;
            goSource.playOnAwake = false;
        }
    }

    private IEnumerator DisplayCountdown(int delay, CountdownEvent function) {
        Setup();
        for(int i = 0; i <= delay; i++) {
            if(delay - i > 0) {
                tickSource.Play();
                text.text = (delay - i).ToString();
            } else {
                goSource.Play();
                text.text = startString;
            }
            yield return ShrinkOverTime(tickDuration);
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
