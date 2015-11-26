using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 8f;
    public float distanceToMaxSpeed = 2f;
    public float waitTime = 30f;

    float waitTill;

	// Use this for initialization
	void Start () {
        transform.position = startPosition;
        waitTill = waitTime;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 toPosition;
        Vector3 fromPosition;
        toPosition = endPosition;
        fromPosition = startPosition;

        if(Time.time > waitTill)
        {
            Debug.Log("Mhm");
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
            
            if(distanceTo < 0.001f)
            {
                this.enabled = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, toPosition, distanceToMove);
        }
	}
}
