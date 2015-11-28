using UnityEngine;
using System.Collections;

public class DropToPosition : MonoBehaviour {

    Vector3 desiredPosition;
    float delay;
    public float timeToPosition =1f;
    float tempTime = 0f;

    public bool isFalling = false;
    public bool isPoppingUp = false;

	// Use this for initialization
	void Start () {

        if (isFalling) { 
        desiredPosition = transform.position;
        transform.position = new Vector3(transform.position.x, 35f, transform.position.z);
        //timeToPosition = Random.value * 2f + 0.5f;
        delay = Random.value / 0.65f + 0.5f;
        }

        if (isPoppingUp)
        {
            desiredPosition = transform.position;
            transform.position = new Vector3(transform.position.x, -35f, transform.position.z);
            //timeToPosition = Random.value * 2f + 0.5f;
            delay = Random.value / 0.65f + 0.5f;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(tempTime < timeToPosition && Time.time > delay)
        {
            tempTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, tempTime / timeToPosition);
        }
	}
}
