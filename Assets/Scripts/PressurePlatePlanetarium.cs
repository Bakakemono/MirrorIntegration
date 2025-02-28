using UnityEngine;
using UnityEngine.Events;

public class PressurePlatePlanetarium : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private bool stayPressed = false;
    [SerializeField] private PlanetGroupController planetGroup;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private bool isPressed = false;
    private bool canUse = true;
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private Transform visualPlate;

    private void Start()
    {
        visualPlate = transform.Find("Visual") ?? transform;
        originalPosition = visualPlate.localPosition;
        pressedPosition = originalPosition + (Vector3.down * pressDepth);

        if (planetGroup != null)
        {
            planetGroup.OnAllPlanetsFullSpeed += () => canUse = true;
            planetGroup.OnAllPlanetsStopped += () => canUse = stayPressed; // Can only use again if stayPressed
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter by Tag: {other.tag}");
        Debug.Log($"Trigger Enter by Layer: {other.gameObject.layer.ToString()}");

        // Only work if planets are at full speed
        if (canUse && (other.CompareTag("Player") || other.gameObject.layer == 6))
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
            canUse = false; // Cannot use again until planets are fully stopped or started
            visualPlate.localPosition = pressedPosition;
            onPressed?.Invoke();
        }
    }

    private void ReleasePlate()
    {
        if (isPressed)
        {
            isPressed = false;
            canUse = false; // Cannot use again until planets are fully started
            visualPlate.localPosition = originalPosition;
            onReleased?.Invoke();
        }
    }
}