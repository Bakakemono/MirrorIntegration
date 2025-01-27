using UnityEngine;

public class Drawbridge : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform pivotPoint;

    private float currentAngle = 0f;
    private float targetAngle = 0f;

    private void Update()
    {
        if (currentAngle != targetAngle)
        {
            // Smooth rotation around pivot point
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, moveSpeed * Time.deltaTime);

            // Calculate rotation around pivot
            transform.RotateAround(
                pivotPoint.position,  // Point to rotate around
                transform.right,      // Axis of rotation (X axis for drawbridge)
                moveSpeed * Time.deltaTime * (targetAngle > currentAngle ? 1 : -1)
            );
        }
    }

    public void OpenBridge()
    {
        targetAngle = openAngle;
    }

    public void CloseBridge()
    {
        targetAngle = 0f;
    }

    private void OnDrawGizmos()
    {
        if (pivotPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pivotPoint.position, 0.5f);
            // Draw rotation axis
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pivotPoint.position - transform.right, pivotPoint.position + transform.right);
        }
    }
}