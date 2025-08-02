/*=========================================================================/
 * Name: CarController.cs
 * Author: Connor Larsen
 * Date: 07/30/2025
 * 
 * Controls the car
/=========================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Car Settings")]
    public float accelerationForce = 3000f;     // The forward acceleration force
    public float maxSpeed = 30f;                // Maximum forward speed
    public Transform spawnPosition;             // The transform for the car's starting position
    public GameObject carModel;                 // Public reference to the car model, to disappear when dead

    [Header("Turning Settings")]
    public float turnSpeed = 3f;                // Base turning force
    public float turnSmoothing = 10f;           // How quickly the car's turning snaps to the target rotation
    public float turnSpeedAtMaxSpeed = 1.5f;    // Multiplier for turnSpeed at maxSpeed

    [Header("Drag Settings")]
    public float groundDrag = 2f;               // The amount of drag applied when the car is on the ground
    public float airDrag = 0.1f;                // The amount of drag applied when the car is in the air

    [Header("Hazard Settings")]
    public float bounceForce = 50f;             // The force applied to the car on impact with a hazard
    public GameObject explosionPrefab;          // Public reference to the explosion animation prefab

    [Header("Stuck/Correction Settings")]
    public float uprightCorrectionSpeed = 5f;   // How fast the car corrects its upright orientation
    public float uprightTiltAngle = 45f;        // The angle at which the car starts to auto-correct
    public float stuckTime = 3f;                // Time in seconds before the car is considered stuck and respawns

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 1.1f;    // Distance for the raycast to detect ground
    public LayerMask groundLayer;               // Layer(s) that are considered ground

    [Header("Health Settings")]
    public int maxHealth = 100;                 // The maximum health of the player's car
    public Action<int> OnHealthChanged;         // Event to notify UI or other systems of health changes

    [Header("Audio Settings")]
    public AudioSource engineSource;
    public AudioSource impactSource;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private float currentSpeed;
    private bool isGrounded;

    // Boost pad settings
    private float originalMaxSpeed;
    private float originalAccelerationForce;
    private float currentBoostTimer;
    private bool isBoosting = false;

    // Health settings
    private int currentHealth;

    // Stuck timer
    private float stuckTimer = 0f;
    #endregion

    #region Functions
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalMaxSpeed = maxSpeed;
        originalAccelerationForce = accelerationForce;
    }

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth); // Notify listeners of the initial health
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.PLAYING)
        {
            moveInput = 1;
        }
        else
        {
            moveInput = 0;
        }
        turnInput = Input.GetAxis("Horizontal");

        CheckIfGrounded();

        // Handle boost timer
        if (isBoosting)
        {
            currentBoostTimer -= Time.deltaTime;
            if (currentBoostTimer <= 0)
            {
                EndBoost();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState == GameState.PLAYING)
        {
            rb.drag = isGrounded ? groundDrag : airDrag;    // Apply drag based on wehther the car is grounded

            currentSpeed = rb.velocity.magnitude;   // Grab the current speed

            ApplyMovement();
            ApplyTurning();
            LimitSpeed();
            CorrectCarOrientation();
            CheckForStuck();
        }
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

    void CorrectCarOrientation()
    {
        // Calculate the angle between the car's "up" vector and the world's "up" vector
        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);

        if (tiltAngle > uprightTiltAngle)
        {
            // Create a target roation that keeps the current forward direction but corrects the up vector
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

            // Smoothly interpolate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * uprightCorrectionSpeed);
        }
    }

    void CheckForStuck()
    {
        if (rb.velocity.magnitude < 1f)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckTime)
        {
            Debug.Log("Car is stuck! Forcing a respawn");
            Die();
        }
    }

    void Respawn()
    {
        // Re-enable the car model
        carModel.SetActive(true);

        // Placeholder respawn logic
        transform.position = spawnPosition.position;
        transform.rotation = Quaternion.identity;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset stuck timer
        stuckTimer = 0f;
    }

    public void ApplyBoost(float multiplier, float duration)
    {
        // If already boosting, reset the timer to extend the boost
        if (isBoosting)
        {
            currentBoostTimer = duration;
            return;
        }

        isBoosting = true;
        currentBoostTimer = duration;

        // Apply boost to to max speed and acceleration force
        maxSpeed = originalMaxSpeed * multiplier;
        accelerationForce = originalAccelerationForce * multiplier;
        Debug.Log("Boost Activated! New Max Speed: " + maxSpeed + ", New Acceleration: " + accelerationForce);
    }

    public void EndBoost()
    {
        isBoosting = false;
        maxSpeed = originalMaxSpeed;
        accelerationForce = originalAccelerationForce;
        Debug.Log("Boost Ended. Restored Max Speed: " + maxSpeed + ", Restored Acceleration: " + accelerationForce);
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // Prevent taking damage after dying

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);    // Clamp health to a minimum of 0
        Debug.Log("Player took " + damageAmount + " damage. Current Health: " + currentHealth);

        // Notify listeners of the health changes
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealDamage(int healAmount)
    {
        if (currentHealth <= 0) return; // Prevent healing damage after dying

        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed " + healAmount + " damage. Current Health: " + currentHealth);

        // Notify listeners of the health changes
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void OnHazardImpact(int damage, Vector3 bounceDirection)
    {
        // Take damage from the hazard
        TakeDamage(damage);

        // Play impact sound
        impactSource.clip = GameManager.Instance.wallBump;
        impactSource.Play();
    }

    public void Die()
    {
        Debug.Log("Player had died!");

        // Instantiate the explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            engineSource.clip = GameManager.Instance.explosion;
            engineSource.loop = false;
            engineSource.Play();
        }

        // Hide the car while the explosion animation plays
        carModel.SetActive(false);

        GameManager.Instance.GameStateSwitch(GameState.RESPAWNING);
        GameManager.Instance.HandleMusic(false);

        StartCoroutine(WaitForExplosion(1f));
    }
    #endregion

    #region Coroutines
    IEnumerator WaitForExplosion(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.Instance.AddDeath();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth);
        Respawn();
        GameManager.Instance.StartCoroutine(GameManager.Instance.StartRespawn(1.5f));
    }
    #endregion
}