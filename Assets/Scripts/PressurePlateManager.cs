using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PressurePlateManager : MonoBehaviour
{
    [Header("Pressure Plates")]
    [SerializeField] private List<PressurePlate> pressurePlates = new List<PressurePlate>();
    [SerializeField] private bool requireAllPlates = true; // If true, needs all plates. If false, works with any plate

    [Header("Events")]
    public UnityEvent onAllPlatesPressed;
    public UnityEvent onPlatesReleased;

    private Dictionary<PressurePlate, bool> plateStates = new Dictionary<PressurePlate, bool>();

    private void Start()
    {
        // Initialize all plates state to false
        foreach (var plate in pressurePlates)
        {
            if (plate != null)
            {
                plateStates[plate] = false;
                plate.onPressed.AddListener(() => OnPlatePressedChanged(plate, true));
                plate.onReleased.AddListener(() => OnPlatePressedChanged(plate, false));
            }
        }
    }

    private void OnPlatePressedChanged(PressurePlate plate, bool isPressed)
    {
        // Update plate state
        plateStates[plate] = isPressed;

        CheckPlatesState();
    }

    private void CheckPlatesState()
    {
        if (requireAllPlates)
        {
            // Check if ALL plates are pressed
            bool allPressed = true;
            foreach (var state in plateStates.Values)
            {
                if (!state)
                {
                    allPressed = false;
                    break;
                }
            }

            if (allPressed)
            {
                onAllPlatesPressed?.Invoke();
            }
            else
            {
                onPlatesReleased?.Invoke();
            }
        }
        else
        {
            // Check if ANY plate is pressed
            bool anyPressed = false;
            foreach (var state in plateStates.Values)
            {
                if (state)
                {
                    anyPressed = true;
                    break;
                }
            }

            if (anyPressed)
            {
                onAllPlatesPressed?.Invoke();
            }
            else
            {
                onPlatesReleased?.Invoke();
            }
        }
    }
}