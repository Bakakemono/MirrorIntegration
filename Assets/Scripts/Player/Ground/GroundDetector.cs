using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : IGroundDetector
{
    private readonly CapsuleCollider _collider;
    private readonly LayerMask _groundLayer;
    private readonly LayerMask _beamLayer;
    private readonly float _extraHeight = 0.1f;
    private readonly float _sphereRadiusMultiplier = 0.9f;  // Pour éviter les accrochages sur les bords

    // Info about what we hit
    private bool _isOnBeam;
    public bool IsOnBeam => _isOnBeam;

    public GroundDetector(CapsuleCollider collider, LayerMask groundLayer,LayerMask beamLayer)
    {
        _collider = collider;
        _groundLayer = groundLayer;
        _beamLayer = beamLayer;
    }

    public bool CheckGround()
    {
        // Le rayon de la sphère de détection
        float radius = _collider.radius * _sphereRadiusMultiplier;

        // Point de départ du SphereCast (bas du collider)
        Vector3 origin = _collider.transform.position +
                        new Vector3(0, _collider.radius, 0);  // Juste au-dessus du bas du collider

        // Distance de détection
        float maxDistance = _extraHeight;

        bool isGrounded = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit hitInfo,
            maxDistance,
            _groundLayer
        );

        // Debug visuel
#if UNITY_EDITOR
        Debug.DrawLine(origin, origin + Vector3.down * maxDistance,
            isGrounded ? Color.green : Color.red);
#endif

        // Vérification supplémentaire si on est proche du sol
        if (!isGrounded)
        {
            // Utilise un Raycast plus précis pour les petites distances
            isGrounded = Physics.Raycast(
                origin,
                Vector3.down,
                maxDistance + radius,
                _groundLayer
            );
        }

        // Check beam
        bool isOnBeam = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit beamHit,
            maxDistance,
            _beamLayer
        );

        _isOnBeam = isOnBeam;


        // Return true if we hit either ground or beam
        return isGrounded || isOnBeam;
    }

    public void DrawDebugGizmos()
    {
        if (_collider == null) return;

        float radius = _collider.radius * _sphereRadiusMultiplier;
        Vector3 origin = _collider.transform.position +
                        new Vector3(0, _collider.radius, 0);
        float maxDistance = _extraHeight;

        // Sphère de départ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, radius);

        // Sphère d'arrivée
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin + Vector3.down * maxDistance, radius);

        // Ligne de connexion
        Gizmos.color = Color.white;
        Gizmos.DrawLine(origin, origin + Vector3.down * maxDistance);

        // Volume de détection
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        DrawCapsule(origin, origin + Vector3.down * maxDistance, radius);
    }

    private void DrawCapsule(Vector3 start, Vector3 end, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1, 1, 0, 0.2f);
        UnityEditor.Handles.DrawWireDisc(start, Vector3.up, radius);
        UnityEditor.Handles.DrawWireDisc(end, Vector3.up, radius);
        Gizmos.DrawLine(start + Vector3.right * radius, end + Vector3.right * radius);
        Gizmos.DrawLine(start - Vector3.right * radius, end - Vector3.right * radius);
        Gizmos.DrawLine(start + Vector3.forward * radius, end + Vector3.forward * radius);
        Gizmos.DrawLine(start - Vector3.forward * radius, end - Vector3.forward * radius);
#endif
    }
}
