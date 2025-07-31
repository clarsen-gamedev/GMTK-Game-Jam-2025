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
    public float accelerationForce = 3000f;     // The forward acceleration force
    public float maxSpeed = 30f;                // Maximum forward speed

    [Header("Turning Settings")]
    public float turnSpeed = 3f;                // Base turning force
    public float turnSmoothing = 10f;           // How quickly the car's turning snaps to the target rotation
    public float turnSpeedAtMaxSpeed = 1.5f;    // Multiplier for turnSpeed at maxSpeed

    [Header("Drag Settings")]
    public float groundDrag = 2f;               // The amount of drag applied when the car is on the ground
    public float airDrag = 0.1f;                // The amount of drag applied when the car is in the air

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 1.1f;    // Distance for the raycast to detect ground
    public LayerMask groundLayer;               // Layer(s) that are considered ground
    #endregion

    #region Privtae Variables
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private float currentSpeed;
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

        currentSpeed = rb.velocity.magnitude;   // Grab the current speed

        ApplyMovement();
        ApplyTurning();
        LimitSpeed();

        //if (isGrounded)
        //{
        //    rb.AddForce(transform.forward * moveInput * accelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        //}

        //float turn = turnInput * turnSpeed * Time.fixedDeltaTime;

        //// NEW TURNING
        //Vector3 targetVelocity = new Vector3(0, turnInput * turnSpeed, 0);
        //rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, targetVelocity, Time.fixedDeltaTime * turnSmoothing);

        //// OLD TURNING
        ////rb.AddTorque(transform.up * turn, ForceMode.Acceleration);
    }

    void ApplyMovement()
    {
        if (isGrounded)
        {
            // Forward movement
            if (moveInput > 0)
            {
                rb.AddForce(transform.forward * moveInput * accelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }
        else
        {
            // Minimal air control
            rb.AddForce(transform.forward * moveInput * (accelerationForce * 0.1f) * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void ApplyTurning()
    {
        float actualTurnSpeed = turnSpeed;

        if (isGrounded)
        {
            // Reduce turn speed at higher speeds for more controlled driving
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, currentSpeed);
            float turnMultiplier = Mathf.Lerp(1, turnSpeedAtMaxSpeed, speedFactor);

            //// Use animation curve for more control over turning response
            //turnMultiplier *= turnControlCurve.Evaluate(speedFactor);

            actualTurnSpeed = turnSpeed * turnMultiplier;

            // Applying turn force
            Vector3 targetAngularVelocity = new Vector3(0, turnInput * actualTurnSpeed, 0);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, transform.TransformDirection(targetAngularVelocity), Time.fixedDeltaTime * turnSmoothing);
        }
        else
        {
            // Some air rotation control
            Vector3 targetAngularVelocity = new Vector3(0, turnInput * (turnSpeed * 0.2f), 0); // Reduced turning in air
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, transform.TransformDirection(targetAngularVelocity), Time.fixedDeltaTime * (turnSmoothing * 0.5f));
        }
    }

    void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit forward speed
        if (moveInput > 0 && flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics.SphereCast(transform.position, 0.5f, -transform.up, out RaycastHit hit, groundCheckDistance, groundLayer);
    }
    #endregion
}