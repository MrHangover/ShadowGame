using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    [Range(0.0f, 50.0f)]
	public float maxSpeed = 5f;
    [Range(0.0f, 50.0f)]
    public float acceleration = 5f;
    [Range(0.0f, 50.0f)]
    public float jumpSpeed = 2f;
    [Range(0.0f, 50.0f)]
    public float friction = 1f;
	Rigidbody body;
	Collider playerCollider;
	bool isGrounded;
	Vector3 tempVelocity;
	Vector3 tempPosition;
	bool isFrozen;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		playerCollider = GetComponent<Collider>();
		isFrozen = true;
		tempVelocity = body.velocity;
		tempPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//Pausing/stopping movement
		if(Input.GetButtonDown("Fire1") && !isFrozen){
			tempVelocity = body.velocity;
			tempPosition = transform.position;
			isFrozen = true;
		}else if(Input.GetButtonDown("Fire1") && isFrozen){
			body.velocity = tempVelocity;
			transform.position = tempPosition;
			isFrozen = false;
		}
		if(isFrozen){
			transform.position = tempPosition;
		}
	}

	void FixedUpdate(){
		if(!isFrozen){
			//Movement
			if(body.velocity.z < maxSpeed && Input.GetAxis("HorizontalPlatform") < 0f){
				if(body.velocity.z - Input.GetAxis("HorizontalPlatform") * acceleration < maxSpeed){
					body.velocity += Vector3.forward * -Input.GetAxis("HorizontalPlatform") * acceleration;
				}
				else{
					body.velocity = new Vector3(body.velocity.x, body.velocity.y, maxSpeed);
				}
			}
			if(body.velocity.z > -maxSpeed && Input.GetAxis("HorizontalPlatform") > 0f){
				if(body.velocity.z - Input.GetAxis("HorizontalPlatform") * acceleration > -maxSpeed){
					body.velocity += Vector3.forward * -Input.GetAxis("HorizontalPlatform") * acceleration;
				}
				else{
					body.velocity = new Vector3(body.velocity.x, body.velocity.y, -maxSpeed);
				}
			}

			//Stopping movement if no input is given
			if(Input.GetAxis("HorizontalPlatform") == 0f){
				if(body.velocity.z > 0f){
					if(body.velocity.z - friction >= 0f){
						body.velocity -= Vector3.forward * friction;
					}else{
						body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0f);
					}
				}
				if(body.velocity.z < 0f){
					if(body.velocity.z + friction <= 0f){
						body.velocity += Vector3.forward * friction;
					}else{
						body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0f);
					}
				}
			}

			//Jumping
			if(isGrounded && Input.GetButton("Jump")){
				body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		if(other.collider.isTrigger == false){
			for(int i = 0; i < other.contacts.Length; i++){
				if(other.contacts[i].point.y <= transform.position.y - transform.localScale.y/2f){
					isGrounded = true;
				}
			}
		}
	}

	void OnCollision(Collision other) {
		if(other.collider.isTrigger == false){
			for(int i = 0; i < other.contacts.Length; i++){
				if(other.contacts[i].point.y <= transform.position.y - transform.localScale.y/2f){
					isGrounded = true;
				}
			}
		}
	}

	void OnCollisionExit(Collision other) {
		Debug.Log(other.gameObject.ToString());
		isGrounded = false;
	}
}
