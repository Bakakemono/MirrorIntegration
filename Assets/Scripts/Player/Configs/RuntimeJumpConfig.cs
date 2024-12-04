using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeJumpConfig
{
    [Header("Jump Parameters")]
    public float jumpHeight;
    public float timeToJumpApex;
    public float upwardMovementMultiplier;
    public float downwardMovementMultiplier;
    public float jumpCutOff;
    public float fallSpeedLimit;
    public int maxAirJumps;
    public float desiredJumpLengthWalk;
    public float desiredJumpLengthRun;

    [Header("Jump Assists")]
    public bool enableCoyoteTime;
    public float coyoteTimeDuration;
    public bool enableJumpBuffer;
    public float jumpBufferDuration;

    public RuntimeJumpConfig(JumpConfig config)
    {
        // Copy values from ScriptableObject
        desiredJumpLengthWalk = config.desiredJumpLengthWalk;
        desiredJumpLengthRun = config.desiredJumpLengthRun;
        jumpHeight = config.jumpHeight;
        timeToJumpApex = config.timeToJumpApex;
        upwardMovementMultiplier = config.upwardMovementMultiplier;
        downwardMovementMultiplier = config.downwardMovementMultiplier;
        jumpCutOff = config.jumpCutOff;
        fallSpeedLimit = config.fallSpeedLimit;
        maxAirJumps = config.maxAirJumps;
        enableCoyoteTime = config.enableCoyoteTime;
        coyoteTimeDuration = config.coyoteTimeDuration;
        enableJumpBuffer = config.enableJumpBuffer;
        jumpBufferDuration = config.jumpBufferDuration;
    }
}
