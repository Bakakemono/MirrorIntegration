using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionExample : MonoBehaviour {

    PlayerControl _playerControl;

    InputAction _interactAction;

    Transform _detectionBoxTransform;
    Vector3 _detectionBoxSize = Vector3.one;

    [SerializeField] LayerMask _interactableObjectLayer;

    [SerializeField] bool _showGizmos = false;
    [SerializeField] Color _gizmosColor = Color.red;


    bool _haveKey = false;
    bool _havePlanet = false;

    List<InteractableObject> _interactableObjectList = new List<InteractableObject>();

    private void Start() {
        _interactAction = _playerControl.Player.Interact;
        _interactAction.performed += Interact;
        _interactAction.Enable();
    }
   
    void Interact(InputAction.CallbackContext context) {
        if(_detectionBoxTransform != null)
            return;
        
        Collider[] detectedObjects = 
            Physics.OverlapBox(
                _detectionBoxTransform.position,
                _detectionBoxSize / 2f,
                transform.rotation, 
                _interactableObjectLayer
            );

        if(detectedObjects.Length > 0) {
            InteractableObject pickableObject = detectedObjects[0].GetComponent<InteractableObject>();
            if(pickableObject != null) {
                if(pickableObject.IsKey()) {
                    _haveKey = true;
                    Destroy(pickableObject.gameObject);
                }
                else if(pickableObject.IsPlanet()) {
                    _havePlanet = true;
                    _interactableObjectList.Add(pickableObject);
                    pickableObject.transform.position += Vector3.up * 1000f;
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if(!_showGizmos)
            return;

        Gizmos.color = _gizmosColor;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(transform.TransformPoint(_detectionBoxTransform.position), _detectionBoxSize);

        Gizmos.matrix = transform.worldToLocalMatrix;
    }
}