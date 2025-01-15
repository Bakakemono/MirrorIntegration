using UnityEngine;

[CreateAssetMenu(fileName = "JumpConfig", menuName = "Player/Jump Config")]
public class JumpConfig : ScriptableObject
{
    [Header("Jump Parameters")]
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.5f;
    public float upwardMovementMultiplier = 1f;
    public float downwardMovementMultiplier = 4f;
    public float walkAirVelocityMultiplier = 0.8f; // Added
    public float runAirVelocityMultiplier = 1.2f;  // Added
    public float jumpCutOff = 2f;
    public float fallSpeedLimit = 20f;
    public int maxAirJumps = 0;
    public float walkAirVelocityMultiplier = 0.3f; // For walking jumps
    public float runAirVelocityMultiplier = 0.5f;  // For running jumps

    [Header("Jump Assists")]
    public bool enableCoyoteTime = true;
    public bool enableJumpBuffer = true;
    public float coyoteTimeDuration = 0.15f;
    public float jumpBufferDuration = 0.15f;
}
