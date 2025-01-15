using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimePushConfig {
    public Vector3 _grabBoxDetectionPostion;
    public Vector3 _grabBoxDetectionDimension;
    public LayerMask _pushableObjectLayerMask;

    public RuntimePushConfig(PushConfig pushConfig) {
        _grabBoxDetectionPostion = pushConfig._grabBoxDetectionPostion;
        _grabBoxDetectionDimension = pushConfig._grabBoxDetectionDimension;
        _pushableObjectLayerMask = pushConfig._pushableObjectLayerMask;
    }
}
