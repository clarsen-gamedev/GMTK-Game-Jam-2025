/*=========================================================================/
 * Name: HealthPickup.cs
 * Author: Connor Larsen
 * Date: 08/02/2025
 * 
 * Place this script on a pickup object with a Collider with Is Trigger.
 * The player will heal damage when collecting this object.
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Hazard Settings")]
    public int healAmount = 10;             // The amount of damage this hazard will deal to the car
    public float respawnDelay = 5.0f;       // The time in seconds before the hazard respawns
    public GameObject pickupEffect;         // Public reference to the pickup effect animation prefab
    #endregion

    #region Private Variables
    private AudioSource pickupSound;
    private Transform mainCameraTransform;
    private SpriteRenderer pickupRenderer;
    private Collider pickupCollider;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    #endregion

    #region Functions
    void Awake()
    {
        // Cache the components and initial transform values on awake
        pickupSound = GetComponent<AudioSource>();
        pickupRenderer = GetComponent<SpriteRenderer>();
        pickupCollider = GetComponent<Collider>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Start()
    {
        // Find the main camera
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Hazard script could not find the Main Camera! Please ensure a camera is tagged as 'MainCamera'");
        }
    }

    void Update()
    {
        // Make the sprite always face the camera
        if (mainCameraTransform != null)
        {
            // Calculate the direction from the hazard to the camera
            Vector3 directionToCamera = mainCameraTransform.position - transform.position;
            directionToCamera.y = 0;

            // Look in the direction of the camera
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
                transform.rotation = targetRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.gameObject.GetComponent<CarController>();
        if (car != null && other.tag == "Player")
        {
            car.HealDamage(healAmount);
        }

        StartRespawnTimer();
    }

    void StartRespawnTimer()
    {
        if (pickupEffect != null)
        {
            pickupSound.clip = GameManager.Instance.pickupSound;
            pickupSound.Play();
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }

        if (pickupRenderer != null)
        {
            pickupRenderer.enabled = false;
        }
        if (pickupCollider != null)
        {
            pickupCollider.enabled = false;
        }

        StartCoroutine(RespawnCoroutine());
    }
    #endregion

    #region Coroutines
    IEnumerator RespawnCoroutine()
    {
        // Wait for the specified respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Reset the hazard's position and rotation
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Re-enable the hazard's renderer and collider
        if (pickupRenderer != null)
        {
            pickupRenderer.enabled = true;
        }
        if (pickupCollider != null)
        {
            pickupCollider.enabled = true;
        }

        Debug.Log(gameObject.name + " has respawned.");
    }
    #endregion
}