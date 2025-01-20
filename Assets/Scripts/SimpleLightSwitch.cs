using UnityEngine;

public class SimpleLight : MonoBehaviour
{
    [SerializeField] private Light targetLight;

    private void Start()
    {
        // Make sure light starts off
        if (targetLight != null)
        {
            targetLight.enabled = false;
        }
    }

    public void TurnOn()
    {
        if (targetLight != null)
        {
            Debug.Log("Light Turned On");
            targetLight.enabled = true;
        }
    }

    public void TurnOff()
    {
        if (targetLight != null)
        {
            Debug.Log("Light Turned Off");
            targetLight.enabled = false;
        }
    }
}