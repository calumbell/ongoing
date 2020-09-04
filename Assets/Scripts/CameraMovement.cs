﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform target;
    public float smoothing;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void LateUpdate() {

        if (transform.position.x != target.position.x | transform.position.y != target.position.y) {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
