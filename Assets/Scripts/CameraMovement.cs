using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float speed = 5f;
	public float sensibility = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Mouse look
		transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * sensibility, 0f, 0f));
		transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * sensibility, 0f), Space.World);

		//Movement
		Vector3 input = new Vector3 (Input.GetAxis("HorizontalCam"), 0f, Input.GetAxis ("VerticalCam"));
		if (input.magnitude > 1f) {
			input.Normalize ();
		}

		Vector3 move = new Vector3 (Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z).normalized * input.z;
		move += new Vector3 (Camera.main.transform.right.x, Camera.main.transform.right.y, Camera.main.transform.right.z) * input.x;
		move += Vector3.up * Input.GetAxis("UpDownCam");

		transform.position += move * speed * Time.deltaTime;
	}
}
