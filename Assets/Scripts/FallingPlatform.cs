using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 8f;
    public float distanceToMaxSpeed = 2f;
    public float waitTime = 30f;
    //Sound stuff
    public AudioClip fall;
    private AudioSource audio;
    //
    float waitTill;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        transform.position = startPosition;
        waitTill = waitTime;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 toPosition;
        Vector3 fromPosition;
        toPosition = endPosition;
        fromPosition = startPosition;

        if (Time.time > waitTill)
        {
            audio.PlayOneShot(fall);
            float distanceToMove = speed * Time.deltaTime;
            float distanceFrom = Vector3.Distance(transform.position, fromPosition);
            float distanceTo = Vector3.Distance(transform.position, toPosition);
            if(distanceFrom < 0.001f)
            {
                distanceToMove = (speed / 20f) * Time.deltaTime;
            }
            else if(distanceFrom < distanceToMaxSpeed)
            {
                distanceToMove = (distanceFrom / distanceToMaxSpeed) * speed * Time.deltaTime + 0.05f * speed * Time.deltaTime;
            }

            transform.position = Vector3.MoveTowards(transform.position, toPosition, distanceToMove);
        }
	}

    public void Reset()
    {
        waitTill = Time.time + waitTime;
        transform.position = startPosition;
    }
}
