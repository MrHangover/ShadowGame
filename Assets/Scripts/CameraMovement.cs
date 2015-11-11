﻿using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {
	
	public float movementSpeed = 30f;
	public float lookSensitivity = 10f;
	public float movementLimitXAxis = 19f;
    int sideOfWall;
	Light light;
	float startSpotAngle;
	float startIntensity;
    Vector3 respawnPosition;
    Animator animator;

	// Use this for initialization
	void Start () {
        sideOfWall = (int)Mathf.Sign(transform.position.x);
		light = GetComponent<Light>();
		startSpotAngle = light.spotAngle;
		startIntensity = light.intensity;
        animator = GetComponent<Animator>();
        respawnPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//Mouse look
		transform.Rotate(new Vector3(Input.GetAxis("Look Y") * lookSensitivity, 0f, 0f));
		transform.Rotate(new Vector3(0f, -Input.GetAxis("Look X") * lookSensitivity * transform.forward.x, 0f), Space.World);

		//Movement
		Vector3 input = new Vector3 (0f, Input.GetAxis("VerticalLight"), Input.GetAxis("HorizontalLight"));
		if (input.magnitude > 1f){
			input.Normalize ();
		}

        Vector3 move;
        if(transform.position.x * sideOfWall > movementLimitXAxis || Input.GetAxis("LightForward") - Input.GetAxis("LightBackward") < 0f)
		    move = new Vector3 ((Input.GetAxis("LightForward") - Input.GetAxis("LightBackward")) * transform.forward.x, input.y, input.z);
        else
            move = new Vector3(0f, input.y, input.z);

        transform.position += move * movementSpeed * Time.deltaTime;

		if(light.spotAngle != startSpotAngle){
			light.spotAngle = Mathf.Lerp(light.spotAngle, startSpotAngle, 0.05f);
			if(Mathf.Abs(light.spotAngle - startSpotAngle) < 0.01f){
				light.spotAngle = startSpotAngle;
			}
		}
		if(light.intensity != startIntensity){
			light.intensity = Mathf.Lerp(light.intensity, startIntensity, 0.05f);
			if(Mathf.Abs(light.intensity - startIntensity) < 0.01f){
				light.intensity = startIntensity;
			}
		}
	}

	public void Flash(){
		light.spotAngle = 179f;
		light.intensity = 8f;
	}

    public void Flicker()
    {
        animator.SetBool("Flicker", true);
        Invoke("StopFlicker", 1f);
    }

    public void StopFlicker()
    {
        animator.SetBool("Flicker", false);
        transform.position = respawnPosition;
    }
}
