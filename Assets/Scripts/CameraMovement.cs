using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {

	public float movementSpeed = 30f;
	public float lookSensitivity = 10f;
    public float movementLimitXAxis = 19f;
    int sideOfWall;

	// Use this for initialization
	void Start () {
        sideOfWall = (int)Mathf.Sign(transform.position.x);
	}
	
	// Update is called once per frame
	void Update () {
		//Mouse look
		transform.Rotate(new Vector3(Input.GetAxis("Look Y") * lookSensitivity, 0f, 0f));
		transform.Rotate(new Vector3(0f, -Input.GetAxis("Look X") * lookSensitivity * transform.forward.x, 0f), Space.World);

		//Movement
		Vector3 input = new Vector3 (0f, Input.GetAxis("VerticalLight"), Input.GetAxis("HorizontalLight"));
		if (input.magnitude > 1f) {
			input.Normalize ();
		}

        Vector3 move;
        if(transform.position.x * sideOfWall > movementLimitXAxis || Input.GetAxis("LightForward") - Input.GetAxis("LightBackward") < 0f)
		    move = new Vector3 ((Input.GetAxis("LightForward") - Input.GetAxis("LightBackward")) * transform.forward.x, input.y, input.z);
        else
            move = new Vector3(0f, input.y, input.z);

        transform.position += move * movementSpeed * Time.deltaTime;
	}
}
