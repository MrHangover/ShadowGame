using UnityEngine;
using System.Collections;

public class PositionFinish : MonoBehaviour {

    public Vector3 finishPosition;
    public float accuracy = 1f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if(Vector3.Distance(transform.position, finishPosition) < accuracy)
        {
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
}
