﻿using UnityEngine;
using System.Collections;

public class MessageScript : MonoBehaviour {

    float alpha = 0f;
    public bool startOn = false;
    public float timeBeforeVisibleFade = 0f;
    public float visibleFadeTime = 1f;
    public float timeBeforeDissapearFade = 5f;
    public float dissapearFadeTime = 1f;
    UnityEngine.UI.Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<UnityEngine.UI.Text>();
        if (startOn)
        {
            alpha = 1f;
        }
        else
        {
            alpha = 0f;
        }

        timeBeforeVisibleFade += Time.time;
        visibleFadeTime += Time.time;
        timeBeforeDissapearFade += Time.time;
        dissapearFadeTime += Time.time;
	}

	// Update is called once per frame
	void Update () {
	    if(Time.time > timeBeforeVisibleFade)
        {
            alpha = (Time.time - timeBeforeVisibleFade) / visibleFadeTime; 
        }
        if (Time.time > timeBeforeDissapearFade)
        {
            alpha = ((timeBeforeDissapearFade - Time.time) + dissapearFadeTime) / dissapearFadeTime;
        }
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        if(alpha == 0f && Time.time > timeBeforeDissapearFade)
        {
            Destroy(gameObject);
        }
    }
}
