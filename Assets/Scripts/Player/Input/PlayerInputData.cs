using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This struct holds all possible input states we might need
public struct PlayerInputData
{
    public Vector3 MovementDirection { get; }
    public float MovementMagnitude { get; }
    public bool IsJumpPressed { get; }
    public bool IsJumpHeld { get; }
    public bool WasJumpReleased { get; }
    public bool IsRunning { get; }
    public bool IsRespawnPressed { get; }
    public bool IsGrabPressed { get; }  // Added for rope interaction

    public PlayerInputData(
        Vector2 movementInput,
        bool jumpPressed,
        bool jumpHeld,
        bool jumpReleased,
        bool running,
        bool respawnPressed,
        bool grabPressed)
    {
        MovementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        MovementMagnitude = movementInput.magnitude;
        IsJumpPressed = jumpPressed;
        IsJumpHeld = jumpHeld;
        WasJumpReleased = jumpReleased;
        IsRunning = running;
        IsRespawnPressed = respawnPressed;
        IsGrabPressed = grabPressed;
    }
}