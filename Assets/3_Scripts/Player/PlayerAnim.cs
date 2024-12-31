using System;
using Mirror;
using UnityEngine;

public class PlayerAnim : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private CharMove move;

    private void Start()
    {
        anim = GetComponent<Animator>();
        move = GetComponent<CharMove>();

        if (!isOwned)
            return;

        move.OnWalk += HandleWalkAnimation;
        move.OnSprint += HandleSprintAnimation;
        move.OnIdle += HandleIdleAnimation;
        move.OnFalling += HandleFallingAnimation;
        move.OnLanding += HandleLandingAnimation;
        move.OnJumpStart += HandleJumpStartAnimation;
        move.OnJumpLand += HandleJumpLandAnimation;
    }

    private void OnDisable()
    {
        // Unsubscribe from events when the object is disabled
        if (!isOwned) return;

        move.OnWalk -= HandleWalkAnimation;
        move.OnSprint -= HandleSprintAnimation;
        move.OnIdle -= HandleIdleAnimation;
        move.OnFalling -= HandleFallingAnimation;
        move.OnLanding -= HandleLandingAnimation;
        move.OnJumpStart -= HandleJumpStartAnimation;
        move.OnJumpLand -= HandleJumpLandAnimation;
    }

    private void HandleWalkAnimation()
    {
        if (!isOwned) return;
        //Debug.Log($"walk request caught");
        CmdSetWalkAnimation();
    }

    [Command]
    private void CmdSetWalkAnimation()
    {
        RpcSetWalkAnimation();
    }

    [ClientRpc]
    private void RpcSetWalkAnimation()
    {
        anim.SetFloat("Speed", 1f);
    }

    private void HandleSprintAnimation()
    {
        if (!isOwned) return;
        //Debug.Log($"sprint request caught");
        CmdSetSprintAnimation();
    }

    [Command]
    private void CmdSetSprintAnimation()
    {
        RpcSetSprintAnimation();
    }

    [ClientRpc]
    private void RpcSetSprintAnimation()
    {
        anim.SetFloat("Speed", 2f);
    }

    private void HandleIdleAnimation()
    {
        if (!isOwned) return;
        //Debug.Log($"idle request caught");
        CmdSetIdleAnimation();
    }

    [Command]
    private void CmdSetIdleAnimation()
    {
        RpcSetIdleAnimation();
    }

    [ClientRpc]
    private void RpcSetIdleAnimation()
    {
        anim.SetFloat("Speed", 0f);
    }

    private void HandleFallingAnimation(bool isFalling)
    {
        if (!isOwned) return;
        CmdSetFallingAnimation(isFalling);
    }

    [Command]
    private void CmdSetFallingAnimation(bool isFalling)
    {
        RpcSetFallingAnimation(isFalling);
    }

    [ClientRpc]
    private void RpcSetFallingAnimation(bool isFalling)
    {
        anim.SetBool("FreeFall", isFalling);
    }

    private void HandleLandingAnimation(bool isLanding)
    {
        if (!isOwned) return;
        CmdSetLandingAnimation(isLanding);
    }

    [Command]
    private void CmdSetLandingAnimation(bool isLanding)
    {
        RpcSetLandingAnimation(isLanding);
    }

    [ClientRpc]
    private void RpcSetLandingAnimation(bool isLanding)
    {
        anim.SetBool("Grounded", isLanding);
    }

    private void HandleJumpStartAnimation()
    {
        if (!isOwned) return;
        CmdSetJumpStartAnimation();
    }

    [Command]
    private void CmdSetJumpStartAnimation()
    {
        RpcSetJumpStartAnimation();
    }

    [ClientRpc]
    private void RpcSetJumpStartAnimation()
    {
        anim.SetTrigger("Jump_Start");
    }

    private void HandleJumpLandAnimation()
    {
        if (!isOwned) return;
        CmdSetJumpLandAnimation();
    }

    [Command]
    private void CmdSetJumpLandAnimation()
    {
        RpcSetJumpLandAnimation();
    }

    [ClientRpc]
    private void RpcSetJumpLandAnimation()
    {
        anim.SetTrigger("Jump_Land");
        anim.ResetTrigger("Jump_Start");
    }
}
