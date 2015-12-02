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
	//Sound stuff
	public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip deathSound;
    private AudioSource audioJump;
    private AudioSource audioLand;
    private AudioSource audioDeath;
    //
    public float lowPitch = .50f;
	public float highPitch = .80f;
    //Animation
    Animator anim;
    Rigidbody body;
	BoxCollider playerCollider;
	bool isGrounded = false;
    bool jumping;
	Vector3 tempVelocity;
	Vector3 tempPosition;
	bool isFrozen;
	Vector3 respawnPosition;
	float jumpEnd;
    string onMac = "";
    bool isColliding = false;
    bool oldIsColliding = false;
    bool isJumping = false;
    FallingPlatform[] scriptsToReset;
    ParticleSystem[] particleSystems;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        scriptsToReset = FindObjectsOfType<FallingPlatform>();
		body = GetComponent<Rigidbody>();
		playerCollider = GetComponent<BoxCollider>();
		isFrozen = false;
		tempVelocity = body.velocity;
		tempPosition = transform.position;
		respawnPosition = transform.position;
        if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            onMac = "Mac";
        }
		audioJump = GetComponent<AudioSource>();
        audioLand = GetComponent<AudioSource>();
        audioDeath = GetComponent<AudioSource>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        //Pausing/stopping movement
        bool oldGround = isGrounded;

		if (isFrozen){
			transform.position = tempPosition;
        }
        else
        {
            CheckCollisions();
        }

        if (isGrounded)
        {
            audioLand.PlayOneShot(landSound);
        }

        anim.SetBool("Grounded", isGrounded);

        anim.SetFloat("VelocityY", body.velocity.y);

        if (!oldGround && isGrounded)
        {
            isJumping = false;
            Debug.Log("Touched ground!");
            foreach(ParticleSystem sys in particleSystems)
            {
                sys.Play();
            }
        }

        if (transform.position.y < deathTriggerHeight){
			Die();
		}

        if(body.velocity.z < 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
        }
        else if(body.velocity.z > 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -1f);
        }
	}

	void FixedUpdate(){
        if (!isFrozen){
			//Movement
			if(body.velocity.z < maxSpeed && Input.GetAxis("HorizontalPlatform" + onMac) < 0f){
				if(body.velocity.z - Input.GetAxis("HorizontalPlatform" + onMac) * acceleration < maxSpeed){
					body.velocity += Vector3.forward * -Input.GetAxis("HorizontalPlatform" + onMac) * acceleration;
				}
				else{
					body.velocity = new Vector3(body.velocity.x, body.velocity.y, maxSpeed);
				}
			}
			if(body.velocity.z > -maxSpeed && Input.GetAxis("HorizontalPlatform" + onMac) > 0f){
				if(body.velocity.z - Input.GetAxis("HorizontalPlatform" + onMac) * acceleration > -maxSpeed){
					body.velocity += Vector3.forward * -Input.GetAxis("HorizontalPlatform" + onMac) * acceleration;
				}
				else{
					body.velocity = new Vector3(body.velocity.x, body.velocity.y, -maxSpeed);
				}
			}

			//Stopping movement if no input is given
			if(Input.GetAxis("HorizontalPlatform" + onMac) == 0f){
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
			if(isGrounded && Input.GetButton("Jump" + onMac) && !isJumping){
                isJumping = true;
                anim.SetTrigger("StartJump");
                audioJump.pitch = Random.Range(lowPitch, highPitch);
                audioJump.PlayOneShot(jumpSound);
                Invoke("Jump", 0.20f);
			}

			if(jumpEnd > Time.time && Input.GetButton("Jump" + onMac)){
				body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
			}
		}

        if (!isColliding)
        {
            transform.parent = null;
        }

        oldIsColliding = isColliding;
        isColliding = false;
	}

    void Jump()
    {
        jumpEnd = Time.time + jumpHoldTime;
        body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
        isGrounded = false;
        anim.SetTrigger("Jump");
    }

	void Die(){
		for(int i = 0; i < lights.Length; i++){
			CameraMovement script = lights[i].GetComponent<CameraMovement>();
			script.Flicker();
            audioDeath.PlayOneShot(deathSound);
            Invoke("Respawn", 0.95f);
		}
	}

    void Respawn()
    {
        transform.position = respawnPosition;
        playerCollider.isTrigger = false;
        playerCollider.enabled = true;
        isFrozen = true;
        body.velocity = new Vector3(0f, 0f, 0f);
        tempPosition = transform.position;
        body.useGravity = false;
        Invoke("StopFreeze", 1f);
    }

    void StopFreeze()
    {
        body.useGravity = true;
        isFrozen = false;
        foreach(FallingPlatform plat in scriptsToReset)
        {
            plat.Reset();
        }
    }

	void CheckCollisions(){
        bool oldGround = isGrounded;
		isGrounded = false;
		Vector3 rayOrigin;
		if(verticalRayPrecision < 2){
			rayOrigin = new Vector3(transform.position.x, transform.position.y - playerCollider.bounds.extents.y + 0.2f + playerCollider.center.y, transform.position.z);
			RaycastHit hit;
			if(Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.25f, shadowLayer)){
				isGrounded = true;
			}
		}else{
			float spacing = playerCollider.bounds.size.z / (verticalRayPrecision - 1f);
			for(int i = 0; i < verticalRayPrecision; i++){
				rayOrigin = new Vector3(transform.position.x, transform.position.y - playerCollider.bounds.extents.y + 0.2f + playerCollider.center.y,
				                        transform.position.z - playerCollider.bounds.extents.z + spacing * i);
				RaycastHit hit;
				//Debug.DrawRay(rayOrigin + Vector3.right * 0.7f, Vector3.down * -body.velocity.y * Time.fixedDeltaTime + Vector3.down * 0.21f, Color.red);
				if(Physics.Raycast(rayOrigin, Vector3.down, out hit, -body.velocity.y * Time.fixedDeltaTime + 0.21f, shadowLayer)){
                    isGrounded = true;
				}
			}
		}

        //Sticking to platforms
        if (oldGround && playerCollider.isTrigger == false)
        {
            float highestGround = -99999f;
            float spacing = playerCollider.bounds.size.z / (verticalRayPrecision - 1f);
            for (int i = 0; i < verticalRayPrecision; i++)
            {
                rayOrigin = new Vector3(transform.position.x, transform.position.y + playerCollider.center.y,
                                        transform.position.z - playerCollider.bounds.extents.z + spacing * i);
                RaycastHit hit;

                //Debug.DrawRay(rayOrigin + Vector3.right, Vector3.down * 1.2f, Color.green);
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1.2f, shadowLayer))
                {
                    isGrounded = true;
                    if (hit.point.y > highestGround)
                        highestGround = hit.point.y;
                }
            }
            if (isGrounded)
            {
                transform.position = new Vector3(transform.position.x, highestGround + playerCollider.bounds.extents.y - playerCollider.center.y, transform.position.z);
                body.velocity = new Vector3(body.velocity.x, 0f, body.velocity.z);
            }
        }
        if (oldGround && !isGrounded)
        {
            anim.SetTrigger("Jump");
            Debug.Log("FUCKING WORK YOU PIECE OF SHIT");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Entered: " + other.gameObject.tag.ToString());
        isColliding = true;
        transform.parent = other.transform;
        if(other.gameObject.tag == "ShadowCol")
        {

        }
        if(other.gameObject.tag == "Enemy")
        {
            Die();
        }
    }

    void OnCollision(Collision other)
    {
        Debug.Log("Stayed: " + other.gameObject.tag.ToString());
    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log("Exited: " + other.gameObject.tag.ToString());
    }
}