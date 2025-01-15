using UnityEngine;

public class OrbitButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private PlanetOrbit[] controlledPlanets;  // Array of planets this button controls
    [SerializeField] private bool startActive = true;

    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            isPressed = true;
            TogglePlanets();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPressed)
        {
            isPressed = false;
        }
    }

    private void TogglePlanets()
    {
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(!planet.IsOrbiting);  // Toggle the orbit state
            }
        }
    }

    private void Start()
    {
        // Set initial state
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(startActive);
            }
        }
    }
}