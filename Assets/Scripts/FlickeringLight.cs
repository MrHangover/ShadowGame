using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour {
	
	public float speed = 0.1f;
	public Light lt;
	public float minIntensity = 0.23f;
	public float maxIntensity = 0.30f; 

	//float random;


	// Use this for initialization
	void Start () {
			lt = GetComponent<Light>();
			//random = Random.Range (minIntensity, maxIntensity);

	}
	
	// Update is called once per frame
	void Update () {

		lt.intensity = minIntensity + Mathf.PingPong(Time.time * speed, 
		                                             maxIntensity - minIntensity);
	
	}
}
