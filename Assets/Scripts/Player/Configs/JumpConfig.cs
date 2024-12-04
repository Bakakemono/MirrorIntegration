using UnityEngine;

[CreateAssetMenu(fileName = "JumpConfig", menuName = "Player/Jump Config")]
public class JumpConfig : ScriptableObject
{
    [Header("Jump Parameters")]
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.5f;
    public float desiredJumpLengthWalk = 5f;
    public float desiredJumpLengthRun = 7f;
    public float upwardMovementMultiplier = 1f;
    public float downwardMovementMultiplier = 4f;
    public float jumpCutOff = 2f;
    public float fallSpeedLimit = 20f;
    public int maxAirJumps = 0;

    [Header("Jump Assists")]
    public bool enableCoyoteTime = true;
    public bool enableJumpBuffer = true;
    public float coyoteTimeDuration = 0.15f;
    public float jumpBufferDuration = 0.15f;
}
