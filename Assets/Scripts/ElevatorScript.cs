using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;
    private bool isAtTop = false;
    private bool isMoving = false;  // Add this to track movement state

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector3.up * moveDistance;
        targetPosition = startPosition;
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > 0.01f)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isMoving = false;  // We've reached our destination
        }
    }

    public void ToggleElevator()
    {
        // Only allow toggle if not currently moving
        if (!isMoving)
        {
            if (!isAtTop)
            {
                targetPosition = endPosition;
                isAtTop = true;
            }
            else
            {
                targetPosition = startPosition;
                isAtTop = false;
            }
        }
    }

    // Optional: Add method to check if elevator can be toggled
    public bool CanToggle()
    {
        return !isMoving;
    }
}