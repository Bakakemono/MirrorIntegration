using UnityEngine;

[CreateAssetMenu(fileName = "RopeConfig", menuName = "Player/Rope Config")]
public class RopeConfig : ScriptableObject
{
    [Header("Rope Physics")]
    public float ropeLength = 5f;
    public float swingForce = 10f;
    public float damping = 0.5f;
    public float springForce = 100f;
    public float massScale = 4.5f;

    [Header("Detachment")]
    public float detachmentUpwardBoost = 2f;
    public float minimumDetachmentVelocity = 2f;
}