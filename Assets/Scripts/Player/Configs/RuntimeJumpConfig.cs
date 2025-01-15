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
<<<<<<< Updated upstream
    public float desiredJumpLengthWalk;
    public float desiredJumpLengthRun;
    public float walkAirVelocityMultiplier = 0.8f; // Added
    public float runAirVelocityMultiplier = 1.2f;  // Added
=======
    public float airVelocityMultiplier;
    public float walkAirVelocityMultiplier;
    public float runAirVelocityMultiplier;
>>>>>>> Stashed changes

    [Header("Jump Assists")]
    public bool enableCoyoteTime;
    public float coyoteTimeDuration;
    public bool enableJumpBuffer;
    public float jumpBufferDuration;

    public RuntimeJumpConfig(JumpConfig config)
    {
        // Copy values from ScriptableObject
        jumpHeight = config.jumpHeight;
        timeToJumpApex = config.timeToJumpApex;
        upwardMovementMultiplier = config.upwardMovementMultiplier;
        downwardMovementMultiplier = config.downwardMovementMultiplier;
        walkAirVelocityMultiplier = config.walkAirVelocityMultiplier;
        runAirVelocityMultiplier = config.runAirVelocityMultiplier;
        jumpCutOff = config.jumpCutOff;
        fallSpeedLimit = config.fallSpeedLimit;
        maxAirJumps = config.maxAirJumps;
        enableCoyoteTime = config.enableCoyoteTime;
        coyoteTimeDuration = config.coyoteTimeDuration;
        enableJumpBuffer = config.enableJumpBuffer;
        jumpBufferDuration = config.jumpBufferDuration;
        walkAirVelocityMultiplier = config.walkAirVelocityMultiplier;
        runAirVelocityMultiplier = config.runAirVelocityMultiplier;
    }
}
