using UnityEngine;
using System.Collections;

public class FinishPoint : MonoBehaviour {
	
	public Material mat;
    //Sound stuff
    public AudioClip finish;
    private AudioSource audio;

    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		mat.SetTextureOffset("_MainTex", new Vector2(Mathf.Cos(Time.time) * 1f, Mathf.Sin(Time.time) * 1f));
	}
	
	IEnumerator OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player"){
            audio.PlayOneShot(finish);
            if (Application.levelCount > Application.loadedLevel + 1){
                float fadeTime = GameObject.Find("Player").GetComponent<FadingScenes>().BeginFade(1);
                yield return new WaitForSeconds(fadeTime);
                Application.LoadLevel(Application.loadedLevel + 1);
			}
			else{
                float fadeTime = GameObject.Find("Player").GetComponent<FadingScenes>().BeginFade(1);
                yield return new WaitForSeconds(fadeTime);
                Application.LoadLevel(0);
			}
			Debug.Log("We have liftoff!");
		}
	}
}
