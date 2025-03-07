using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    [SerializeField] bool _isKey = false;
    [SerializeField] bool _isPlanet = false;
    [SerializeField] bool _isPlanetarium = false;
    [SerializeField] bool _isDoor = false;

    public bool IsKey() {
        return _isKey;
    }

    public bool IsPlanet() {
        return _isPlanet;
    }

    public bool IsPlanetarium() {
        return _isPlanetarium;
    }

    public bool IsDoor() {
        return _isDoor;
    }
}
