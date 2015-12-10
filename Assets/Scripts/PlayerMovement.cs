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
    [Range(5f, 80f)]
    public float maxClimbAngle = 70f;
    [Range(5f, 80f)]
    public float maxDescendAngle = 50f;
	//Sound stuff
	public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip deathSound;
    public AudioClip enemySound;
    private AudioSource audioJump;
    private AudioSource audioLand;
    private AudioSource audioDeath;
    private AudioSource audioEnemy;
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
    Vector3 spriteStartPosition;
    Vector3 spriteStartRotation;
    Transform[] sprites;
    FallingPlatform[] scriptsToReset;
    ParticleSystem[] particleSystems;

	// Use this for initialization
	void Start () {
        sprites = new Transform[2];
        sprites[0] = transform.FindChild("SpriteFront");
        sprites[1] = transform.FindChild("SpriteBack");
        spriteStartPosition = sprites[0].localPosition;
        spriteStartRotation = sprites[0].localEulerAngles;
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
        audioEnemy = GetComponent<AudioSource>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        //Pausing/stopping movement
        bool oldGround = isGrounded;

        if (isFrozen)
        {
            transform.position = tempPosition;
        }

        anim.SetBool("Grounded", isGrounded);

        anim.SetFloat("VelocityY", body.velocity.y);

        if (!oldGround && isGrounded)
        {
            Debug.Log("Touched ground!");
            foreach(ParticleSystem sys in particleSystems)
            {
                sys.Play();
            }
            audioLand.PlayOneShot(landSound, 0.7f);
        }

        if (transform.position.y < deathTriggerHeight){
			Die();
			audioDeath.PlayOneShot(deathSound);
		}

        if(body.velocity.z < -0.05f)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
        }
        else if(body.velocity.z > 0.05f)
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
                Invoke("Jump", 0.15f);
                Debug.Log("JUMP!");
			}

			if(jumpEnd > Time.time && Input.GetButton("Jump" + onMac)){
				body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
			}

            CheckCollisions();
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
        isJumping = false;
        anim.SetTrigger("Jump");
    }

	void Die(){
        for (int i = 0; i < lights.Length; i++){
			CameraMovement script = lights[i].GetComponent<CameraMovement>();
			transform.position += Vector3.up * 9999f;
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
        foreach (Transform sprite in sprites)
        {
            sprite.localPosition = new Vector3(sprite.localPosition.x, spriteStartPosition.y, spriteStartPosition.z);
            sprite.localEulerAngles = spriteStartRotation;
        }

        bool oldGround = isGrounded;
		isGrounded = false;
		Vector3 rayOrigin;
		if(verticalRayPrecision < 2){
			rayOrigin = new Vector3(transform.position.x, transform.position.y - playerCollider.bounds.extents.y + 0.2f + playerCollider.center.y, transform.position.z);
			RaycastHit hit;
			if(Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.25f, shadowLayer)){
				isGrounded = true;
                //transform.parent = hit.transform;
            }
        }
        else{
            float skinWidth = 0.05f;
			float spacing = playerCollider.bounds.size.z / (verticalRayPrecision - 1f) - skinWidth * 2 / (verticalRayPrecision - 1f);
			for(int i = 0; i < verticalRayPrecision; i++){
				rayOrigin = new Vector3(transform.position.x, transform.position.y + playerCollider.center.y,
				                        transform.position.z - playerCollider.bounds.extents.z + spacing * i + skinWidth);
				RaycastHit hit;
                //Debug.DrawRay(rayOrigin + Vector3.right * 0.75f, Vector3.down * (-body.velocity.y * Time.fixedDeltaTime + 0.21f + playerCollider.bounds.extents.y), Color.green);
				if(Physics.Raycast(rayOrigin, Vector3.down, out hit, -body.velocity.y * Time.fixedDeltaTime + 0.1f + playerCollider.bounds.extents.y, shadowLayer)){
                    isGrounded = true;
                    //transform.parent = hit.transform;
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
                    //1isGrounded = true;
                    if (hit.point.y > highestGround)
                        highestGround = hit.point.y;
                }
            }
            if (isGrounded && highestGround < transform.position.y + playerCollider.bounds.extents.y)
            {
                transform.position = new Vector3(transform.position.x, highestGround + playerCollider.bounds.extents.y - playerCollider.center.y, transform.position.z);
                body.velocity = new Vector3(body.velocity.x, 0f, body.velocity.z);
            }
        }

        //Slope climbing DO NOT TOUCH BITTE PLZ
        //if (Input.GetAxis("HorizontalPlatform" + onMac) != 0f)
        //{
            float directionZ = -Mathf.Sign(transform.localScale.z);
            float rayLength = Mathf.Abs(body.velocity.z) * Time.fixedDeltaTime + 0.02f;

            rayOrigin = (directionZ < 0f) ? new Vector3(transform.position.x, transform.position.y + playerCollider.center.y - playerCollider.bounds.extents.y + 0.02f, transform.position.z - playerCollider.bounds.extents.z) : new Vector3(
                transform.position.x, transform.position.y + playerCollider.center.y - playerCollider.bounds.extents.y + 0.02f, transform.position.z + playerCollider.bounds.extents.z);

            RaycastHit hitHorizontal;
            Debug.DrawRay(rayOrigin + Vector3.right, Vector3.forward * directionZ * rayLength, Color.red);

            if (Physics.Raycast(rayOrigin, Vector3.forward * directionZ, out hitHorizontal, rayLength, shadowLayer))
            {
                Vector2 normal2D = new Vector2(hitHorizontal.normal.z, hitHorizontal.normal.y);
                float slopeAngle = Vector2.Angle(normal2D, Vector2.up);
                if (slopeAngle <= maxClimbAngle)
                {
                    isGrounded = true;
                    float moveDistance = Mathf.Abs(body.velocity.z);
                    float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                    float climbVelocityZ = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(body.velocity.z);
                    body.velocity = new Vector3(body.velocity.x, climbVelocityY, climbVelocityZ);
                    foreach (Transform sprite in sprites)
                    {
                        //sprite.RotateAround(sprite.position + Vector3.up * 1.5f * -directionZ - Vector3.forward * -directionZ, Vector3.right, slopeAngle);
                        sprite.RotateAround(sprite.position + Vector3.down * 1f * directionZ + Vector3.forward * 0.5f * directionZ, Vector3.right, slopeAngle);
                    }
                }
            }

            rayLength = 0.05f + Mathf.Abs(body.velocity.y) * Time.fixedDeltaTime;

            rayOrigin = (directionZ < 0f) ? new Vector3(transform.position.x, transform.position.y + playerCollider.center.y - playerCollider.bounds.extents.y + 0.02f, transform.position.z + playerCollider.bounds.extents.z) : new Vector3(
                transform.position.x, transform.position.y + playerCollider.center.y - playerCollider.bounds.extents.y + 0.02f, transform.position.z - playerCollider.bounds.extents.z);

            RaycastHit hitVertical;

            Debug.DrawRay(rayOrigin + Vector3.right, Vector3.down * rayLength, Color.green);
            if (Physics.Raycast(rayOrigin, Vector3.down, out hitVertical, rayLength, shadowLayer) && !isJumping)
            {
                Vector2 normal2D = new Vector2(hitVertical.normal.z, hitVertical.normal.y);
                float slopeAngle = Vector2.Angle(normal2D, Vector2.up);
                if (slopeAngle <= maxDescendAngle && slopeAngle > 5f)
                {
                    Debug.Log(slopeAngle);
                    isGrounded = true;
                    float moveDistance = Mathf.Abs(body.velocity.z);
                    float climbVelocityY = -Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                    float climbVelocityZ = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(body.velocity.z);
                    body.velocity = new Vector3(body.velocity.x, climbVelocityY, climbVelocityZ);
                    foreach (Transform sprite in sprites)
                    {
                        sprite.RotateAround(sprite.position + Vector3.down * 1f * directionZ - Vector3.forward * 0.5f * directionZ, Vector3.right, -slopeAngle);
                    }
                }
            }
        //}

        if (oldGround && !isGrounded)
        {
            anim.SetTrigger("Jump");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        isColliding = true;
        //transform.parent = other.transform;
        if (isGrounded)
        {
            transform.parent = other.transform;
        }
        if(other.gameObject.tag == "ShadowCol")
        {

        }
        if(other.gameObject.tag == "Enemy")
        {
            audioEnemy.PlayOneShot(enemySound);
            Die();
        }
    }
}