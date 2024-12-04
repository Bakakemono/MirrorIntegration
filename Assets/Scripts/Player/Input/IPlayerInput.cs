using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for input handling
public interface IPlayerInput
{
    PlayerInputData GetInput();
    void Enable();
    void Disable();
}
