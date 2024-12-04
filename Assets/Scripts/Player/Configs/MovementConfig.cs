using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Player/Movement Config")]
public class MovementConfig : ScriptableObject
{
    [Header("Ground Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;

    [Header("Air Movement")]
    public float airWalkSpeed = 2f;
    public float airRunSpeed = 5f;
    public float airAccelerationTime = 0.2f;
    public float airDecelerationTime = 0.2f;
}
