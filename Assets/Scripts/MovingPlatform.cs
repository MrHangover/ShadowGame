using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 1f;
    public float distanceToMaxSpeed = 1f;
    public float pauseTime = 1f;

    float waitTill;
    int direction;

	// Use this for initialization
	void Start () {
        transform.position = startPosition;
        waitTill = pauseTime;
        direction = 1;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 toPosition;
        Vector3 fromPosition;
        if(direction == 1)
        {
            toPosition = endPosition;
            fromPosition = startPosition;
        }
        else
        {
            toPosition = startPosition;
            fromPosition = endPosition;
        }

        if(Time.time > waitTill)
        {
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
                direction = -direction;
                waitTill = Time.time + pauseTime;
            }
            else if(distanceTo < distanceToMaxSpeed)
            {
                distanceToMove = (distanceTo / distanceToMaxSpeed) * speed * Time.deltaTime + 0.05f * speed * Time.deltaTime;
            }

            transform.position = Vector3.MoveTowards(transform.position, toPosition, distanceToMove);
        }
	}
}
