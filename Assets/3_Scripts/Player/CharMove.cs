using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class CharMove : MonoBehaviour
{
    public float walkSpeed = 4.0f;			// Default walking speed.
    public float sprintFactor = 2.0f;       // How much sprinting affects fly speed.
	public GameObject model;
	public GameObject thirdPersonCam;

	private float x, y;
	private bool isSprinting, changedFOV;
	private Vector3 velocity = Vector3.zero;

	private Rigidbody rb;
	private Animator anim;
	private NetworkAnimator nAnim;
	private ThirdPersonOrbitCamBasic cam_TPS;

	//grounded variables so anim will be in sync
	private bool isGrounded;
	public Transform groundCheck;
	public float groundDist = 0.28f;
	public LayerMask groundMask;

	//jump variables
	public float JumpHeight = 2.4f;

	private bool isUsingFPSCam;
	private bool isCursorLocked; //for initializing lock purposes

	private float currYSpeed;
	private bool isJumping;

	private void Awake()
    {
		isUsingFPSCam = false;
		isCursorLocked = false;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		nAnim = GetComponent<NetworkAnimator>();
		cam_TPS = thirdPersonCam.GetComponent<ThirdPersonOrbitCamBasic>();

		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;    // Color for not grounded state
		Gizmos.DrawWireSphere(groundCheck.position, groundDist);
	}

	void Update()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

		if (Cursor.lockState != CursorLockMode.Locked && !isCursorLocked)
		{
			LockCursor(true);
			Invoke(nameof(CheckMouseStat), .1f);
		}

		x = Input.GetAxisRaw("Horizontal");
		y = Input.GetAxisRaw("Vertical");

		isSprinting = Input.GetButton("Fire3");

		if (!isUsingFPSCam)
		{
			Rotating(x, y);
		}

		Vector3 direction = GetDir(x, y);
		Vector3 movement = direction * walkSpeed * (isSprinting ? sprintFactor : 1);

		HandleSprinting();
		HandleJump();
		Move(movement);
		anim.SetFloat("MotionSpeed", movement.magnitude/4);
		AnimatePlayer();
	}

	private void HandleSprinting()
	{
		if (isSprinting)
		{
			cam_TPS.SetFOV(100);
			changedFOV = true;
		}
		else if (changedFOV)
		{
			cam_TPS.ResetFOV();
			changedFOV = false;
		}
	}

	private void FixedUpdate()
	{
		if (velocity != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}
	}

	private void Move(Vector3 movement)
	{
		velocity = movement;
	}
	private void HandleJump()
	{
		currYSpeed = rb.linearVelocity.y;

		if (isGrounded)
		{
			// Jump
			if (Input.GetButtonDown("Jump"))
			{
				// Apply jump force to Rigidbody
				rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), rb.linearVelocity.z);
				isJumping = true;
			}
		}
	}

	private Vector3 GetDir(float horizontal, float vertical)
	{
		Vector3 forward = model.transform.TransformDirection(Vector3.forward);
		forward = forward.normalized;

		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		Vector3 targetDirection = forward * vertical + right * horizontal;

		return targetDirection;
	}

	private void Rotating(float horizontal, float vertical)
	{
		if (horizontal == 0 && vertical == 0)
			return;

		Vector3 forward = thirdPersonCam.transform.forward;
		forward.y = 0; 
		forward.Normalize();  

		Quaternion targetRotation = Quaternion.LookRotation(forward);

		model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, .06f);
	}

	private void AnimatePlayer()
	{
		// Check if the character is moving
		if (x != 0 || y != 0)
		{
			if (isSprinting)
			{
				anim.SetFloat("Speed", 2f);
			}
			else
			{
				anim.SetFloat("Speed", 1f);
			}
		}
		else
		{
			anim.SetFloat("Speed", 0f);
		}

		anim.SetBool("Grounded", isGrounded);
		anim.SetBool("FreeFall", !isGrounded);

		if (currYSpeed > 0 && isGrounded) // Starting a jump
		{
			anim.SetTrigger("Jump_Start");
			nAnim.SetTrigger("Jump_Start");
		}
		else if (!isGrounded) // In free fall (falling down)
		{
			anim.ResetTrigger("Jump_Start"); // Ensure no conflicting triggers
			nAnim.ResetTrigger("Jump_Start");
		}
		if (isGrounded && isJumping) // Landed after a jump or fall
		{
			isJumping = false;
			anim.SetTrigger("Jump_Land");
			nAnim.SetTrigger("Jump_Land");
			anim.ResetTrigger("Jump_Start"); // Ensure no conflicting triggers
			nAnim.ResetTrigger("Jump_Start"); // Ensure no conflicting triggers
		}

	}

	public void LockCursor(bool locking)
	{
        if (locking)
        {
			Cursor.lockState = CursorLockMode.Locked; 
			//Debug.Log("locked");
		}
        else
        {
			Cursor.lockState = CursorLockMode.None;
			//Debug.Log("unlocked");
		}
	}

	private void CheckMouseStat()
    {
		isCursorLocked = Cursor.visible;
	}

	private void OnFootstep(AnimationEvent animationEvent)
	{

	}
	private void OnLand(AnimationEvent animationEvent)
    {

    }
}
