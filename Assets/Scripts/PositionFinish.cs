using UnityEngine;
using System.Collections;

public class PositionFinish : MonoBehaviour {

    public Vector3 finishPosition;
    public float accuracy = 1f;
    public float waitTime = 2f;
    float waitTill;
    bool isWaiting = false;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (!isWaiting)
        {
            waitTill = Time.time + waitTime;
        }
        if (Vector3.Distance(transform.position, finishPosition) < accuracy)
        {
            isWaiting = true;
            Debug.Log("Actually... I changed my mind!");
            if (Time.time > waitTill) { 
                if (Application.levelCount > Application.loadedLevel + 1)
                {
                    Application.LoadLevel(Application.loadedLevel + 1);
                }
                else
                {
                    Application.LoadLevel(0);
                }
            }
        }
        else
        {
            isWaiting = false;
        }
    }
}
