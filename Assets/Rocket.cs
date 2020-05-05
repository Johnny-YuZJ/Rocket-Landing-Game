using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    // Start is called before the first frame update

    Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    private void ProcessInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            rigidbody.AddRelativeForce(Vector3.up);
            print("space/up pressed");
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.forward);
            print("a/left pressed");
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.forward);
            print("d/right pressed");
        }
    }
}
