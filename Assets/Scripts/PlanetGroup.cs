using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetGroupController : MonoBehaviour
{
    [SerializeField] private List<PlanetOrbit> controlledPlanets = new List<PlanetOrbit>();

    // Track if all planets are stopped or at full speed
    public bool AllPlanetsAtFullSpeed { get; private set; } = true;
    public bool AllPlanetsStopped { get; private set; } = false;

    // Events
    public event Action OnAllPlanetsFullSpeed;
    public event Action OnAllPlanetsStopped;

    private void Start()
    {
        // Subscribe to planet events
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.OnFullSpeedReached += CheckAllPlanetsFullSpeed;
                planet.OnFullStop += CheckAllPlanetsStopped;
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.OnFullSpeedReached -= CheckAllPlanetsFullSpeed;
                planet.OnFullStop -= CheckAllPlanetsStopped;
            }
        }
    }

    private void CheckAllPlanetsFullSpeed()
    {
        foreach (var planet in controlledPlanets)
        {
            if (planet != null && !planet.IsAtFullSpeed)
                return;
        }

        // All planets are at full speed
        AllPlanetsAtFullSpeed = true;
        OnAllPlanetsFullSpeed?.Invoke();
    }

    private void CheckAllPlanetsStopped()
    {
        foreach (var planet in controlledPlanets)
        {
            if (planet != null && !planet.IsStopped)
                return;
        }

        // All planets are stopped
        AllPlanetsStopped = true;
        OnAllPlanetsStopped?.Invoke();
    }

    public void StopPlanets()
    {
        AllPlanetsAtFullSpeed = false;
        AllPlanetsStopped = false;

        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(false);
            }
        }
    }

    public void StartPlanets()
    {
        AllPlanetsAtFullSpeed = false;
        AllPlanetsStopped = false;

        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(true);
            }
        }
    }
}