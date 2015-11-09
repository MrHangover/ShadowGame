using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {

	public float speed = 30f;
	public float sensibility = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Mouse look
		transform.Rotate(new Vector3(Input.GetAxis("Look Y") * sensibility, 0f, 0f));
		transform.Rotate(new Vector3(0f, -Input.GetAxis("Look X") * sensibility * transform.forward.x, 0f), Space.World);

		//Movement
		Vector3 input = new Vector3 (0f, Input.GetAxis("VerticalLight"), Input.GetAxis("HorizontalLight"));
		if (input.magnitude > 1f) {
			input.Normalize ();
		}

        Debug.Log(transform.right.ToString());

		Vector3 move = new Vector3 ((Input.GetAxis("LightForward") - Input.GetAxis("LightBackward")) * transform.forward.x, input.y, input.z);

		transform.position += move * speed * Time.deltaTime;
	}
}
