using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClimbConfig", menuName = "Player/Climb Config")]
public class ClimbConfig : ScriptableObject {
    [Header("General Params")]
    [SerializeField] public LayerMask _climbableLayers;

    [Header("player Info")]
    [SerializeField] public float _playerHeight;
    [SerializeField] public float _playerWidth;
    [SerializeField] public bool _isPivotPointCentered = false;

    [Header("Low Climb Detection Params")]
    [SerializeField] public float _lowLengthClimbDetection;
    [SerializeField] public float _lowHeightClimbDetection;
    [SerializeField] public float _lowDepthClimbDetection;
    [SerializeField] public float _lowWidthClimbDetection;
    [SerializeField] public float _lowHeightRestrictionClimbDetection;

    [Header("High Climb Detection Params")]
    [SerializeField] public float _highLengthClimbDetection;
    [SerializeField] public float _highHeightClimbDetection;
    [SerializeField] public float _highDepthClimbDetection;
    [SerializeField] public float _highWidthClimbDetection;
    [SerializeField] public float _highHeightRestrictionClimbDetection;

    [Header("Climb Sequence Params")]
    [SerializeField] public AnimationCurve _lowClimbinigHorizontalCurve;
    [SerializeField] public AnimationCurve _lowClimbinigVerticalCurve;
    [SerializeField] public float _lowClimbingTime;

    [SerializeField] public AnimationCurve _highClimbinigHorizontalCurve;
    [SerializeField] public AnimationCurve _highClimbinigVerticalCurve;
    [SerializeField] public float _highClimbingTime;

    [SerializeField] public float _cooldown = 1f;

    [SerializeField] public bool DEBUG_showLowDetection;
    [SerializeField] public bool DEBUG_showHighDetection;
}