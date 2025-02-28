using System.Collections.Generic;
using UnityEngine;

public class OrbitButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private List<PlanetOrbit> controlledPlanets = new List<PlanetOrbit>();
    [SerializeField] private bool startActive = true;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void HandleInteract(bool isInteractPressed)
    {
        if (playerInRange && isInteractPressed)
        {
            TogglePlanets();
        }
    }

    private void TogglePlanets()
    {
        // Check the first planet to determine the current state
        if (controlledPlanets.Count > 0 && controlledPlanets[0] != null)
        {
            bool shouldOrbit = !controlledPlanets[0].IsAtFullSpeed;

            foreach (var planet in controlledPlanets)
            {
                if (planet != null)
                {
                    planet.SetOrbiting(shouldOrbit);
                }
            }
        }
    }

    private void Start()
    {
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(startActive);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualization for the interaction area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}