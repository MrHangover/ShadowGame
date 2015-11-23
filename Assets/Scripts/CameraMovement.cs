using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {
	
	public float movementSpeed = 30f;
	public float lookSensitivity = 10f;
	public float movementLimitXAxis = 19f;
    public float movementLimitYAxis = 19f;
    public float movementLimitZAxis = 19f;
    int sideOfWall;
	Light light;
    Vector3 respawnPosition;
    Quaternion respawnRotation;
    Animator animator;
    string onMac = "";

	// Use this for initialization
	void Start () {
        sideOfWall = (int)Mathf.Sign(transform.position.x);
		light = GetComponent<Light>();
        animator = GetComponent<Animator>();
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            onMac = "Mac";
        }
    }
	
	// Update is called once per frame
	void Update () {
		//Mouse look
		transform.Rotate(new Vector3(Input.GetAxis("Look Y" + onMac) * lookSensitivity, 0f, 0f));
		transform.Rotate(new Vector3(0f, -Input.GetAxis("Look X" + onMac) * lookSensitivity * transform.forward.x, 0f), Space.World);

		//Movement
		Vector3 input = new Vector3 (0f, Input.GetAxis("VerticalLight" + onMac), Input.GetAxis("HorizontalLight" + onMac));
		if (input.magnitude > 1f){
			input.Normalize ();
		}

        Vector3 move;
        float moveX = 0f, moveY = input.y, moveZ = input.z;
        //Debug.Log("LightForward: " + Input.GetAxis("LightForward").ToString() + "\tLightBackward: " + Input.GetAxis("LightBackward").ToString());
        if(transform.position.x * sideOfWall > movementLimitXAxis || Input.GetAxis("LightForward" + onMac) - Input.GetAxis("LightBackward" + onMac) < 0f)
		    moveX = (Input.GetAxis("LightForward" + onMac) - Input.GetAxis("LightBackward" + onMac)) * transform.forward.x;
        if (transform.position.y * Mathf.Sign(input.y) > movementLimitYAxis)
        {
            moveY = 0f;
        }
        if (transform.position.z * Mathf.Sign(input.z) > movementLimitZAxis)
        {
            moveZ = 0f;
        }

        move = new Vector3(moveX, moveY, moveZ);
        transform.position += move * movementSpeed * Time.deltaTime;
	}

    public void Flicker()
    {
        animator.SetBool("Flicker", true);
        Invoke("StopFlicker", 0.95f);
    }

    public void StopFlicker()
    {
        animator.SetBool("Flicker", false);
        transform.position = respawnPosition;
        Invoke("Respawn", 0.5f);
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
    }
}
