using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler:IPlayerInput
{
    private readonly PlayerControl _playerControl;
    private readonly InputAction _movementAction;
    private readonly InputAction _jumpAction;
    private readonly InputAction _runAction;
    private readonly InputAction _respawnAction;
    private bool _isEnabled;

    public bool IsEnabled => _isEnabled;

    public PlayerInputHandler()
    {
        _playerControl = new PlayerControl();

        _movementAction = _playerControl.Player.Movement;
        _jumpAction = _playerControl.Player.Jump;
        _runAction = _playerControl.Player.Run;
        _respawnAction = _playerControl.Player.Respawn;

        Enable(); // Enable inputs by default
    }

    public PlayerInputData GetInput()
    {
        var input = new PlayerInputData(
            _movementAction.ReadValue<Vector2>(),
            _jumpAction.WasPressedThisFrame(),
            _jumpAction.IsPressed(),
            _jumpAction.WasReleasedThisFrame(),
            _runAction.IsPressed(),
            _respawnAction.WasPressedThisFrame()
        );

        return input;
    }

    public void Enable()
    {
        _movementAction.Enable();
        _jumpAction.Enable();
        _runAction.Enable();
        _respawnAction.Enable();
        _isEnabled = true;
    }

    public void Disable()
    {
        _movementAction.Disable();
        _jumpAction.Disable();
        _runAction.Disable();
        _respawnAction.Disable();
        _isEnabled = false;
    }
}