using UnityEngine;
using System.Collections;

public class ControllerFadeScript : MonoBehaviour {

    float alpha = 0f;
    public bool startOn = false;
    public float timeBeforeVisibleFade = 0f;
    public float visibleFadeTime = 1f;
    public float timeBeforeDissapearFade = 5f;
    public float dissapearFadeTime = 1f;
	float tempTimeBeforeVisibleFade = 0f;
	float tempVisibleFadeTime = 1f;
	float tempTimeBeforeDissapearFade = 5f;
	float tempDissapearFadeTime = 1f;
    UnityEngine.UI.Image image;

	// Use this for initialization
	void Start () {
        image = GetComponent<UnityEngine.UI.Image>();
        if (startOn)
        {
            alpha = 1f;
        }
        else
        {
            alpha = 0f;
        }

        tempTimeBeforeVisibleFade = Time.time + timeBeforeVisibleFade;
        tempVisibleFadeTime = Time.time + visibleFadeTime;
        tempTimeBeforeDissapearFade = Time.time + timeBeforeDissapearFade;
        tempDissapearFadeTime = Time.time + dissapearFadeTime;
	}

	// Update is called once per frame
	void Update () {
	    if(Time.time > tempTimeBeforeVisibleFade)
        {
            alpha = (Time.time - tempTimeBeforeVisibleFade) / tempVisibleFadeTime; 
        }
        if (Time.time > tempTimeBeforeDissapearFade)
        {
            alpha = ((tempTimeBeforeDissapearFade - Time.time) + tempDissapearFadeTime) / tempDissapearFadeTime;
        }
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        if(alpha == 0f && Time.time > tempTimeBeforeDissapearFade)
        {
            Destroy(gameObject);
        }
    }
}
