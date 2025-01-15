using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationShowCase : MonoBehaviour {
    Animator _animator;

    private void Start() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            _animator.SetBool("Jumping", true);
        }
        else {
            _animator.SetBool("Jumping", false);
        }
        bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if(isWalking) {
            _animator.SetBool("Walking", true);
            if(Input.GetKey(KeyCode.LeftShift)) {
                _animator.SetBool("Running", true);
            }
            else {
                _animator.SetBool("Running", false);
            }
        }
        else {
            _animator.SetBool("Walking", false);
            _animator.SetBool("Running", false);
        }
    }
}