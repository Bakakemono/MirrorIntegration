using UnityEngine;

public class PlayerClimb {
    ClimbConfig _config = null;

    private Transform _playerTransform;

    private Vector3 _targetPos;
    private Vector3 _positionWhenStartClimbing;
    private float _timeWhenStartClimbing = 0f;
    private float _timeWhenStopClimbing = 0f;

    bool isClimbing = false;

    public bool _isClimbing {
        get {
            return isClimbing;
        }
    }

    private bool _lowClimb = false;

    public PlayerClimb(ClimbConfig config, Transform transform) {
        _config = config;
        _playerTransform = transform;
    }
    
    public void ClimbCheck() {
        if(isClimbing)
            return;

        if(Time.time < _timeWhenStopClimbing + _config._cooldown)
            return;

        Collider[] climbableObject =
            Physics.OverlapBox(
                _playerTransform.position +
                    _playerTransform.forward * ((_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                    Vector3.up * (_config._lowHeightClimbDetection + _config._lowDepthClimbDetection) / 2f,
                new Vector3(
                    _config._lowWidthClimbDetection / 2f,
                    (_config._lowHeightClimbDetection - _config._lowDepthClimbDetection) / 2f,
                    (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f
                ),
                _playerTransform.rotation,
                _config._climbableLayers
            );

        Collider[] notClimbableObject =
            Physics.OverlapBox(
                _playerTransform.position +
                    _playerTransform.forward * ((_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                    Vector3.up * (_config._lowHeightRestrictionClimbDetection + _config._lowHeightClimbDetection) / 2f,
                new Vector3(
                    _config._lowWidthClimbDetection / 2f,
                    (_config._lowHeightRestrictionClimbDetection - _config._lowHeightClimbDetection) / 2f,
                    (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f
                ),
                _playerTransform.rotation
            );

        if(climbableObject.Length > 0) {
            if(notClimbableObject.Length == 0) {
                RaycastHit hit;

                Collider[] detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            _playerTransform.forward * ((_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._lowHeightClimbDetection + _config._lowDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._lowHeightClimbDetection / 2f,
                            (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                        _playerTransform.forward * _config._lowLengthClimbDetection +
                        Vector3.up * _config._lowHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._lowHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );
                    _targetPos = hit.point;
                    isClimbing = true;
                    _lowClimb = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }

                detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            -_playerTransform.right * _config._lowWidthClimbDetection / 2f +
                            _playerTransform.forward * ((_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._lowHeightClimbDetection + _config._lowDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._lowHeightClimbDetection / 2f,
                            (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                            _playerTransform.forward * _config._lowLengthClimbDetection +
                            -_playerTransform.right * _config._lowWidthClimbDetection / 2f +
                            Vector3.up * _config._lowHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._lowHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );
                    _targetPos = hit.point;
                    isClimbing = true;
                    _lowClimb = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }

                detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            _playerTransform.right * _config._lowWidthClimbDetection / 2f +
                            _playerTransform.forward * ((_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._lowHeightClimbDetection + _config._lowDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._lowHeightClimbDetection / 2f,
                            (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                            _playerTransform.forward * _config._lowLengthClimbDetection +
                            _playerTransform.right * _config._lowWidthClimbDetection / 2f +
                            Vector3.up * _config._lowHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._lowHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );

                    _targetPos = hit.point;
                    isClimbing = true;
                    _lowClimb = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }
            }
        }

        climbableObject =
            Physics.OverlapBox(
                _playerTransform.position +
                    _playerTransform.forward * ((_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                    Vector3.up * (_config._highHeightClimbDetection + _config._highDepthClimbDetection) / 2f,
                new Vector3(
                    _config._highWidthClimbDetection / 2f,
                    (_config._highHeightClimbDetection - _config._highDepthClimbDetection) / 2f,
                    (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f
                ),
                _playerTransform.rotation,
                _config._climbableLayers
            );

        notClimbableObject =
            Physics.OverlapBox(
                _playerTransform.position +
                    _playerTransform.forward * ((_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                    Vector3.up * (_config._highHeightRestrictionClimbDetection + _config._highHeightClimbDetection) / 2f,
                new Vector3(
                    _config._highWidthClimbDetection / 2f,
                    (_config._highHeightRestrictionClimbDetection - _config._highHeightClimbDetection) / 2f,
                    (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f
                ),
                _playerTransform.rotation
            );

        if(climbableObject.Length > 0) {
            if(notClimbableObject.Length == 0) {
                RaycastHit hit;

                Collider[] detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            _playerTransform.forward * ((_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._highHeightClimbDetection + _config._highDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._highHeightClimbDetection / 2f,
                            (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                            _playerTransform.forward * _config._highLengthClimbDetection +
                            Vector3.up * _config._highHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._highHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );
                    _targetPos = hit.point;
                    isClimbing = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }

                detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            -_playerTransform.right * _config._highWidthClimbDetection / 2f +
                            _playerTransform.forward * ((_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._highHeightClimbDetection + _config._highDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._highHeightClimbDetection / 2f,
                            (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                            _playerTransform.forward * _config._highLengthClimbDetection +
                            -_playerTransform.right * _config._highWidthClimbDetection / 2f +
                            Vector3.up * _config._highHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._highHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );
                    _targetPos = hit.point;
                    isClimbing = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }

                detectedObjects =
                    Physics.OverlapBox(
                        _playerTransform.position +
                            _playerTransform.right * _config._highWidthClimbDetection / 2f +
                            _playerTransform.forward * ((_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f + _config._playerWidth / 2f) +
                            Vector3.up * (_config._highHeightClimbDetection + _config._highDepthClimbDetection) / 2f,
                        new Vector3(
                            0.01f,
                            _config._highHeightClimbDetection / 2f,
                            (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f
                        ),
                        _playerTransform.rotation,
                        _config._climbableLayers
                    );

                if(detectedObjects.Length > 0) {
                    Physics.Raycast(
                        _playerTransform.position +
                            _playerTransform.forward * _config._highLengthClimbDetection +
                            _playerTransform.right * _config._highWidthClimbDetection / 2f +
                            Vector3.up * _config._highHeightRestrictionClimbDetection,
                        Vector3.down,
                        out hit,
                        _config._highHeightRestrictionClimbDetection * 1.1f,
                        _config._climbableLayers
                    );

                    _targetPos = hit.point;
                    isClimbing = true;
                    _timeWhenStartClimbing = Time.time;
                    _positionWhenStartClimbing = _playerTransform.position;
                    return;
                }
            }
        }
    }

    public void Climb() {
        if(_targetPos == Vector3.zero) {
            isClimbing = false;
            return;
        }
        Vector3 newPos =
            Vector3.Lerp(
                _positionWhenStartClimbing,
                _targetPos,
                _lowClimb ?
                    _config._lowClimbinigHorizontalCurve.Evaluate((Time.time - _timeWhenStartClimbing) / _config._lowClimbingTime) :
                    _config._highClimbinigHorizontalCurve.Evaluate((Time.time - _timeWhenStartClimbing) / _config._highClimbingTime)
            );

        float newHight =
            Mathf.Lerp(
                _positionWhenStartClimbing.y,
                _targetPos.y + (_config._isPivotPointCentered ? _config._playerHeight / 2f : 0f),
                _lowClimb ?
                    _config._lowClimbinigVerticalCurve.Evaluate((Time.time - _timeWhenStartClimbing) / _config._lowClimbingTime) :
                    _config._highClimbinigVerticalCurve.Evaluate((Time.time - _timeWhenStartClimbing) / _config._highClimbingTime)
            );

        _playerTransform.position = new Vector3(newPos.x, newHight, newPos.z);

        if(Time.time - _timeWhenStartClimbing >= (_lowClimb ? _config._lowClimbingTime : _config._highClimbingTime)) {
            _playerTransform.position = _targetPos + Vector3.up * (_config._isPivotPointCentered ? _config._playerHeight / 2f : 0f);
            isClimbing = false;
            _lowClimb = false;
            _timeWhenStopClimbing = Time.time;
        }
    }

    public void DrawGizmos() {
        if(_playerTransform == null) 
            return;

        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = _playerTransform.localToWorldMatrix;

        if(_config.DEBUG_showLowDetection) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                Vector3.forward * _config._playerWidth / 2f + Vector3.forward * (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f +
                    Vector3.up * (_config._lowHeightClimbDetection + _config._lowDepthClimbDetection) / 2f,
                new Vector3(
                    _config._lowWidthClimbDetection,
                    _config._lowHeightClimbDetection - _config._lowDepthClimbDetection,
                    _config._lowLengthClimbDetection - _config._playerWidth / 2f
                )
            );

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                Vector3.forward * _config._playerWidth / 2f + Vector3.forward * (_config._lowLengthClimbDetection - _config._playerWidth / 2f) / 2f +
                    Vector3.up * (_config._lowHeightRestrictionClimbDetection + _config._lowHeightClimbDetection) / 2f,
                new Vector3(
                    _config._lowWidthClimbDetection,
                    _config._lowHeightRestrictionClimbDetection - _config._lowHeightClimbDetection,
                    _config._lowLengthClimbDetection - _config._playerWidth / 2f
                )
            );
        }


        if(_config.DEBUG_showHighDetection) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                Vector3.forward * _config._playerWidth / 2f + Vector3.forward * (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f +
                    Vector3.up * (_config._highHeightClimbDetection + _config._highDepthClimbDetection) / 2f,
                new Vector3(
                    _config._highWidthClimbDetection,
                    _config._highHeightClimbDetection - _config._highDepthClimbDetection,
                    _config._highLengthClimbDetection - _config._playerWidth / 2f
                )
            );

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                Vector3.forward * _config._playerWidth / 2f + Vector3.forward * (_config._highLengthClimbDetection - _config._playerWidth / 2f) / 2f +
                    Vector3.up * (_config._highHeightRestrictionClimbDetection + _config._highHeightClimbDetection) / 2f,
                new Vector3(
                    _config._highWidthClimbDetection,
                    _config._highHeightRestrictionClimbDetection - _config._highHeightClimbDetection,
                    _config._highLengthClimbDetection - _config._playerWidth / 2f
                )
            );
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            Vector3.up * (_config._isPivotPointCentered ? 0f : _config._playerHeight / 2f),
            new Vector3(
                _config._playerWidth,
                _config._playerHeight,
                _config._playerWidth
            )
        );

        Gizmos.matrix = defaultMatrix;
    }
}
