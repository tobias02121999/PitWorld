using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Initialize the public variables
    public Vector2 lookSensitivity;

    // Initialize the private variables
    Rigidbody rb;
    Transform camTrans;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camTrans = GetComponentInChildren<Camera>().transform;

        Initialize(); // Initialize the entity object
    }

    // Update is called once per frame
    private void Update()
    {
        Look(transform, camTrans, lookSensitivity, controls.lookAxisHor, controls.lookAxisVer); // Look around by input
        Sprint(Input.GetButton(controls.sprintButton)); // Increase the entities speed
    }

    // FixedUpdate is called once per fixed frame
    void FixedUpdate()
    {
        Move(rb, controls.moveAxisHor, controls.moveAxisVer); // Move on input
    }
}
