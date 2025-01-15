[System.Serializable]
public class RuntimeRopeConfig
{
    public float ropeLength;
    public float swingForce;
    public float damping;
    public float springForce;
    public float massScale;
    public float detachmentUpwardBoost;
    public float minimumDetachmentVelocity;

    public RuntimeRopeConfig(RopeConfig config)
    {
        ropeLength = config.ropeLength;
        swingForce = config.swingForce;
        damping = config.damping;
        springForce = config.springForce;
        massScale = config.massScale;
        detachmentUpwardBoost = config.detachmentUpwardBoost;
        minimumDetachmentVelocity = config.minimumDetachmentVelocity;
    }
}