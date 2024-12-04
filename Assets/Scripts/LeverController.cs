using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [SerializeField] private float activationDelay = 0.5f;
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject[] planets;
    private bool _isActivated = false;
    private bool playerInZone = false;

    PlayerControl _playerControl;
    InputAction _interactAction;

    void Start()
    {
  
        _interactAction = _playerControl.Player.Grab;
        _interactAction.Enable();
    }

    void Update()
    {
        if (playerInZone && _interactAction.triggered)
        {
            StartCoroutine(ActivateLever());
        }
    }

    private IEnumerator ActivateLever()
    {
        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);

        _isActivated = !_isActivated; // Toggle activation state

        // Toggle planets' movement, emissive materials, and point lights
        foreach (GameObject planet in planets)
        {
            // Stop or start the orbiting
            PlanetOrbit planetOrbit = planet.GetComponent<PlanetOrbit>();
            if (planetOrbit != null)
            {
                planetOrbit.SetOrbiting(!_isActivated);
            }

            // Control the emissive property
            Renderer renderer = planet.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Get the material instance
                Material material = renderer.material;

                if (_isActivated)
                {
                    // Disable emission
                    material.DisableKeyword("_EMISSION");
                }
                else
                {
                    // Enable emission
                    material.EnableKeyword("_EMISSION");
                }
            }

            // Control the point light
            Light pointLight = planet.GetComponentInChildren<Light>();
            if (pointLight != null)
            {
                pointLight.enabled = !_isActivated;
            }
        }

        // Sun's emission and point light remain enabled
        Renderer sunRenderer = sun.GetComponent<Renderer>();
        if (sunRenderer != null)
        {
            Material sunMaterial = sunRenderer.material;
            sunMaterial.EnableKeyword("_EMISSION");
        }

        // Ensure the sun's point light is enabled
        Light sunLight = sun.GetComponentInChildren<Light>();
        if (sunLight != null)
        {
            sunLight.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}
