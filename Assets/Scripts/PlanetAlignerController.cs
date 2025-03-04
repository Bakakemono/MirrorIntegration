using UnityEngine;

public class PlanetAlignerController : MonoBehaviour
{
    public PlanetAligner planetAligner;

    public void AlignPlanets()
    {
        if (planetAligner != null)
        {
            Debug.Log("Calling align planets");
            planetAligner.AlignPlanets();
        }
    }

    public void ResumeOrbits()
    {
        if (planetAligner != null)
        {
            Debug.Log("Calling resume orbits");
            planetAligner.ResumeOrbits();
        }
    }
}