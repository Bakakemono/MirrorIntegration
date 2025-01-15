using UnityEngine;

[CreateAssetMenu(fileName = "PushConfig", menuName = "Player/Push Config")]

public class PushConfig : ScriptableObject {
    [Header("Box Params")]
    [SerializeField] public Vector3 _grabBoxDetectionPostion;
    [SerializeField] public Vector3 _grabBoxDetectionDimension;
    [SerializeField] public LayerMask _pushableObjectLayerMask;
}