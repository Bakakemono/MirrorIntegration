using UnityEngine;

// Store character model configurations
[CreateAssetMenu(fileName = "CharacterConfig", menuName = "Player/Character Config")]
public class CharacterConfig:ScriptableObject
{
    [Header("Model Selection")]
    [Tooltip("When checked, uses girl model. When unchecked, uses boy model")]
    public bool useGirlModel = true;

    [Tooltip("Size for girl character (x: radius, y: height, z: unused)")]
    public Vector3 girlSize = new Vector3(0.5f, 2f, 0.5f);

    [Tooltip("Size for boy character (x: radius, y: height, z: unused)")]
    public Vector3 boySize = new Vector3(0.5f, 2f, 0.5f);

    // Validation
    public void Validate()
    {
        if (girlSize.x <= 0 || girlSize.y <= 0)
            Debug.LogError("Invalid girl size in CharacterConfig!");
        if (boySize.x <= 0 || boySize.y <= 0)
            Debug.LogError("Invalid boy size in CharacterConfig!");
    }
}