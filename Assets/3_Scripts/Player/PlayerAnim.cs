using System;
using Mirror;
using UnityEngine;

public class PlayerAnim : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private CharMove move;

    // Network variables for syncing animation states
    [SyncVar] private float moveSpeed;
    [SyncVar] private bool isFalling;
    [SyncVar] private bool isGrounded;
    [SyncVar(hook = nameof(OnJumpStartHook))] private bool jumpStartTriggered;
    [SyncVar(hook = nameof(OnJumpLandHook))] private bool jumpLandTriggered;

    private void Start()
    {
        if (!isOwned) return;

        move.OnWalk += () => SetMoveSpeed(1f);
        move.OnSprint += () => SetMoveSpeed(2f);
        move.OnIdle += () => SetMoveSpeed(0f);
        move.OnFalling += (falling) => CmdSetFalling(falling);
        move.OnLanding += (grounded) => CmdSetGrounded(grounded);
        move.OnJumpStart += () => CmdTriggerJumpStart();
        move.OnJumpLand += () => CmdTriggerJumpLand();
    }

    private void OnDisable()
    {
        if (!isOwned) return;

        move.OnWalk -= () => SetMoveSpeed(1f);
        move.OnSprint -= () => SetMoveSpeed(2f);
        move.OnIdle -= () => SetMoveSpeed(0f);
        move.OnFalling -= (falling) => CmdSetFalling(falling);
        move.OnLanding -= (grounded) => CmdSetGrounded(grounded);
        move.OnJumpStart -= () => CmdTriggerJumpStart();
        move.OnJumpLand -= () => CmdTriggerJumpLand();
    }

    // Local animation handling
    private void UpdateAnimations()
    {
        anim.SetFloat("Speed", moveSpeed);
        anim.SetBool("FreeFall", isFalling);
        anim.SetBool("Grounded", isGrounded);
    }

    private void SetMoveSpeed(float speed)
    {
        if (!isOwned) return;
        CmdSetMoveSpeed(speed);
    }

    // Commands and hooks to sync animation states
    [Command]
    private void CmdSetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    [Command]
    private void CmdSetFalling(bool falling)
    {
        isFalling = falling;
    }

    [Command]
    private void CmdSetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    [Command]
    private void CmdTriggerJumpStart()
    {
        jumpStartTriggered = true;
    }

    private void OnJumpStartHook(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            anim.SetTrigger("Jump_Start");
            jumpStartTriggered = false; // Reset trigger state
        }
    }

    [Command]
    private void CmdTriggerJumpLand()
    {
        jumpLandTriggered = true;
    }

    private void OnJumpLandHook(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            anim.SetTrigger("Jump_Land");
            anim.ResetTrigger("Jump_Start");
            jumpLandTriggered = false; // Reset trigger state
        }
    }

    private void Update()
    {
        if (!isOwned) return;

        // Update animations locally
        UpdateAnimations();
    }
}
