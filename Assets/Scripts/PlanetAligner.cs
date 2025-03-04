using System.Collections.Generic;
using UnityEngine;

public class PlanetAligner : MonoBehaviour
{
    [SerializeField] private List<PlanetOrbit> controlledPlanets = new List<PlanetOrbit>();
    [SerializeField] private Transform alignmentPosition;
    [SerializeField] private bool startAligned = false;

    private bool allPlanetsAligned = false;
    private int alignedPlanetsCount = 0;

    private void Start()
    {
        // Set up planet event listeners
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.OnAlignmentComplete += OnPlanetAligned;
            }
        }

        // Initial alignment if needed
        if (startAligned && alignmentPosition != null)
        {
            AlignPlanets();
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.OnAlignmentComplete -= OnPlanetAligned;
            }
        }
    }

    private void OnPlanetAligned()
    {
        alignedPlanetsCount++;

        if (alignedPlanetsCount >= controlledPlanets.Count)
        {
            allPlanetsAligned = true;
            alignedPlanetsCount = 0;
            // You could fire an event here if needed
        }
    }

    public void AlignPlanets()
    {
        if (alignmentPosition != null)
        {
            allPlanetsAligned = false;
            alignedPlanetsCount = 0;

            foreach (var planet in controlledPlanets)
            {
                if (planet != null)
                {
                    planet.AlignToPosition(alignmentPosition.position);
                }
            }
        }
    }

    public void ResumeOrbits()
    {
        foreach (var planet in controlledPlanets)
        {
            if (planet != null)
            {
                planet.SetOrbiting(true);
            }
        }
    }
}