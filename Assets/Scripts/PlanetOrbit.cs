using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    [SerializeField] private Transform sun;
    [SerializeField] private float orbitSpeed = 10f;
    private bool isOrbiting = true;

    void Update()
    {
        if (isOrbiting && sun != null)
        {
            // Rotate around the sun
            transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }

    public void SetOrbiting(bool value)
    {
        isOrbiting = value;
    }
}
