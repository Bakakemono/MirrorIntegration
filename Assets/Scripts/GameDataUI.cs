using UnityEngine;
using TMPro;  // For TextMeshPro - remove this if using regular Text components

public class GameDataUI : MonoBehaviour
{
    [Header("Text UI Elements")]
    [SerializeField] private TextMeshProUGUI groundedStatusText;
    [SerializeField] private TextMeshProUGUI jumpStatusText;
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI pushPullStatusText;

    [Header("Player Data")]
    [SerializeField] private HLPlayerController playerController;
    [SerializeField] private PushableObject pushObject;

    private Rigidbody playerRigidbody;

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in GameDataUI!");
            return;
        }

        // Get the player's Rigidbody for velocity data
        playerRigidbody = playerController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerController == null || playerRigidbody == null) return;

        // Update each UI element with the current data
        //groundedStatusText.text = "Grounded: " + playerController.IsGrounded;
        velocityText.text = "Velocity: " + playerRigidbody.velocity.ToString("F2");
        //pushPullStatusText.text = "Push/Pull Mode: " + (playerController.IsPushPulling ? "Active" : "Inactive");
    }
}
