using UnityEngine;

public class RopeSystem
{
    private readonly Rigidbody _rigidbody;
    private readonly RuntimeRopeConfig _config;
    private readonly Transform _ropeAnchor;
    private readonly PlayerMovement _movement;
    private readonly PlayerJump _jump;

    private bool _isAttached;
    private SpringJoint _joint;
    private Vector3 _attachPoint;
    private float _currentRopeLength;

    public bool IsAttached => _isAttached;

    public RopeSystem(
        Rigidbody rigidbody,
        RuntimeRopeConfig config,
        Transform ropeAnchor,
        PlayerMovement movement,
        PlayerJump jump)
    {
        _rigidbody = rigidbody;
        _config = config;
        _ropeAnchor = ropeAnchor;
        _movement = movement;
        _jump = jump;
    }

    public void UpdateRope(PlayerInputData input, bool canAttach)
    {
        if (!_isAttached && canAttach && input.IsGrabPressed)
        {
            AttachToRope();
        }
        else if (_isAttached)
        {
            if (input.IsJumpPressed)
            {
                DetachFromRope();
                return;
            }

            HandleSwinging(input);
        }
    }

    private void AttachToRope()
    {
        _isAttached = true;

        // Create spring joint
        _joint = _rigidbody.gameObject.AddComponent<SpringJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = _ropeAnchor.position;

        // Configure joint
        _currentRopeLength = Vector3.Distance(_ropeAnchor.position, _rigidbody.position);
        _joint.minDistance = 0;
        _joint.maxDistance = _currentRopeLength;
        _joint.spring = _config.springForce;
        _joint.damper = _config.damping;
        _joint.massScale = _config.massScale;

        _attachPoint = _ropeAnchor.position;

        // Disable gravity (we'll handle it)
        _rigidbody.useGravity = false;
    }

    private void HandleSwinging(PlayerInputData input)
    {
        Vector3 inputDirection = input.MovementDirection;

        if (inputDirection.magnitude > 0.1f)
        {
            // Calculate swing direction perpendicular to rope
            Vector3 ropeDirection = (_attachPoint - _rigidbody.position).normalized;
            Vector3 perpendicularDirection = Vector3.Cross(ropeDirection, Vector3.up);

            // Apply force
            Vector3 swingForce = perpendicularDirection * _config.swingForce;
            _rigidbody.AddForce(swingForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    private void DetachFromRope()
    {
        if (_joint != null)
        {
            // Store velocity for preservation
            Vector3 releaseVelocity = _rigidbody.velocity;

            // Clean up joint
            Object.Destroy(_joint);
            _joint = null;

            // Restore gravity
            _rigidbody.useGravity = true;

            // Apply release velocity with upward boost
            _rigidbody.velocity = releaseVelocity + (Vector3.up * _config.detachmentUpwardBoost);
        }

        _isAttached = false;
    }

    public void Reset()
    {
        if (_isAttached)
        {
            DetachFromRope();
        }
    }
}