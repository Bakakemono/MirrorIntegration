using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pressDepth = 0.1f;    // How far the plate moves down
    [SerializeField] private bool stayPressed = false;   // If true, stays pressed after first activation

    [Header("Events")]
    public UnityEvent onPressed;    // Called when plate is pressed
    public UnityEvent onReleased;   // Called when plate is released

    private bool isPressed = false;
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private Transform visualPlate;   // The visual part that moves down

    private void Start()
    {
        // Get or create the visual part
        visualPlate = transform.Find("Visual") ?? transform;
        originalPosition = visualPlate.localPosition;
        pressedPosition = originalPosition + (Vector3.down * pressDepth);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter by Tag: {other.tag}");
        Debug.Log($"Trigger Enter by Layer: {other.gameObject.layer.ToString()}");
        if (other.CompareTag("Player") || other.gameObject.layer == 6)
    {
        Debug.Log("Pressing plate");
        PressPlate();
    }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.gameObject.layer == 6) && !stayPressed)
        {
            ReleasePlate();
        }
    }

    private void PressPlate()
    {
        if (!isPressed)
        {
            isPressed = true;
            visualPlate.localPosition = pressedPosition;
            onPressed?.Invoke();
        }
    }

    private void ReleasePlate()
    {
        if (isPressed)
        {
            isPressed = false;
            visualPlate.localPosition = originalPosition;
            onReleased?.Invoke();
        }
    }
}