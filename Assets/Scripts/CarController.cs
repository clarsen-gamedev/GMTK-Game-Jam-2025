/*=========================================================================/
 * Name: CarController.cs
 * Author: Connor Larsen
 * Date: 07/30/2025
 * Description: Controls the car
/=========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Car Settings")]
    public float moveSpeed = 3000f;     // The forward acceleration force
    public float turnSpeed = 3f;        // The turning force
    public float turnSmoothing = 10f;   // How quickly the car's turning snaps to the target rotation
    public float groundDrag = 2f;       // The amount of drag applied when the car is on the ground
    public float airDrag = 0.1f;        // The amount of drag applied when the car is in the air
    #endregion

    #region Privtae Variables
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private bool isGrounded;
    #endregion

    #region Functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        CheckIfGrounded();
    }

    void FixedUpdate()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;    // Apply drag based on wehther the car is grounded

        if (isGrounded)
        {
            rb.AddForce(transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;

        // NEW TURNING
        Vector3 targetVelocity = new Vector3(0, turnInput * turnSpeed, 0);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, targetVelocity, Time.fixedDeltaTime * turnSmoothing);

        // OLD TURNING
        //rb.AddTorque(transform.up * turn, ForceMode.Acceleration);
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.1f);
    }
    #endregion
}