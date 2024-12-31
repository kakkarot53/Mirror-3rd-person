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
	private ThirdPersonOrbitCamBasic cam_TPS;

	//action callbacks
	public event Action OnWalk;
	public event Action OnSprint;
	public event Action OnIdle;
	public event Action<bool> OnFalling;
	public event Action<bool> OnLanding;
	public event Action OnJumpStart;
	public event Action OnJumpLand;

	//grounded variables so anim will be in sync
	private bool isGrounded;
	public Transform groundCheck;
	public float groundDist = 0.28f;
	public LayerMask groundMask;

	//jump variables
	public float JumpHeight = 2.4f;

	private bool isCursorLocked; //for initializing lock purposes

	private float currYSpeed;
	private bool isJumping;

	private void Awake()
    {
		isCursorLocked = false;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
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


		Rotating(x, y);
		

		Vector3 direction = GetDir(x, y);
		Vector3 movement = direction * walkSpeed * (isSprinting ? sprintFactor : 1);

		HandleSprinting();
		HandleJump();
		Move(movement);
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
				OnJumpStart?.Invoke();
			}

			if (isJumping)
			{
				OnJumpLand?.Invoke();
				isJumping = false;
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
				//Debug.Log($"sprint invoked");
				OnSprint?.Invoke();
			}
			else
			{
				//Debug.Log($"walk invoked");
				OnWalk?.Invoke();
			}
		}
		else
		{
			//Debug.Log($"idle invoked");
			OnIdle?.Invoke();
		}

		OnFalling?.Invoke(!isGrounded);
		OnLanding?.Invoke(isGrounded);
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
