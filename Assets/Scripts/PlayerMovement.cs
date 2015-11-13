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
	[Range(0.0f, 1f)]
	public float jumpHoldTime = 0.2f;
	public float deathTriggerHeight = -17f;
    public int verticalRayPrecision = 4;
	public GameObject[] lights;
	public LayerMask shadowLayer;
    Rigidbody body;
	Collider playerCollider;
	bool isGrounded;
	Vector3 tempVelocity;
	Vector3 tempPosition;
	bool isFrozen;
	Vector3 respawnPosition;
	float jumpEnd;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		playerCollider = GetComponent<Collider>();
		isFrozen = true;
		tempVelocity = body.velocity;
		tempPosition = transform.position;
		respawnPosition = transform.position;
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

		if(transform.position.y < deathTriggerHeight){
			Die();
		}
	}

	void FixedUpdate(){
		if(!isFrozen){
			CheckCollisions();
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
				jumpEnd = Time.time + jumpHoldTime;
			}
			if(jumpEnd > Time.time && Input.GetButton("Jump")){
				body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
			}
		}
	}

	void Die(){
		for(int i = 0; i < lights.Length; i++){
			CameraMovement script = lights[i].GetComponent<CameraMovement>();
			script.Flicker();
            Invoke("Respawn", 0.95f);
		}
	}

    void Respawn()
    {
        transform.position = respawnPosition;
        playerCollider.isTrigger = false;
        playerCollider.enabled = true;
        isFrozen = true;
        Invoke("StopFreeze", 0.5f);
    }

    void StopFreeze()
    {
        isFrozen = false;
    }

	void CheckCollisions(){
		isGrounded = false;
		Vector3 rayOrigin;
		if(verticalRayPrecision < 2){
			rayOrigin = new Vector3(transform.position.x, transform.position.y - playerCollider.bounds.extents.y + 0.2f, transform.position.z);
			RaycastHit hit;
			Debug.DrawRay(rayOrigin, Vector3.down * 0.25f, Color.red);
			if(Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.25f, shadowLayer)){
				isGrounded = true;
			}
		}else{
			float spacing = playerCollider.bounds.size.z / (verticalRayPrecision - 1f);
			for(int i = 0; i < verticalRayPrecision; i++){
				rayOrigin = new Vector3(transform.position.x, transform.position.y - playerCollider.bounds.extents.y + 0.2f,
				                        transform.position.z - playerCollider.bounds.extents.z + spacing * i);
				RaycastHit hit;
				Debug.DrawRay(rayOrigin, Vector3.down * 0.25f, Color.red);
				if(Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.25f, shadowLayer)){
					isGrounded = true;
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag.ToString());
		if(other.gameObject.tag != "DeathLight")
        {
            playerCollider.isTrigger = false;
        }
        else
        {
            playerCollider.isTrigger = true;
        }
	}
}
