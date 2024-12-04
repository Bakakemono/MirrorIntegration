using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeMovementConfig
{
    [Header("Ground Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float accelerationTime;
    public float decelerationTime;

    [Header("Air Movement")]
    public float airWalkSpeed;
    public float airRunSpeed;
    public float airAccelerationTime;
    public float airDecelerationTime;

    public RuntimeMovementConfig(MovementConfig config)
    {
        // Copy values from ScriptableObject
        walkSpeed = config.walkSpeed;
        runSpeed = config.runSpeed;
        accelerationTime = config.accelerationTime;
        decelerationTime = config.decelerationTime;
        airWalkSpeed = config.airWalkSpeed;
        airRunSpeed = config.airRunSpeed;
        airAccelerationTime = config.airAccelerationTime;
        airDecelerationTime = config.airDecelerationTime;
    }
}
