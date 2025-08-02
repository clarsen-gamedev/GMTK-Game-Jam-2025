/*=========================================================================/
 * Name: Hazard.cs
 * Author: Connor Larsen
 * Date: 07/31/2025
 * 
 * Place this script on a hazard object with a Collider with Is Trigger.
 * The player will take damage when hit by this object
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Hazard Settings")]
    public int damageAmount = 10;                   // The amount of damage this hazard will deal to the car
    public float respawnDelay = 5.0f;               // The time in seconds before the hazard respawns
    public GameObject explosionPrefab;              // Public reference to the explosion animation prefab
    #endregion

    #region Private Variables
    private Transform mainCameraTransform;
    private SpriteRenderer hazardRenderer;
    private Collider hazardCollider;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    #endregion

    #region Functions
    void Awake()
    {
        // Cache the components and initial transform values on awake
        hazardRenderer = GetComponent<SpriteRenderer>();
        hazardCollider = GetComponent<Collider>();
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

    void OnCollisionEnter(Collision collision)
    {
        CarController car = collision.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            car.OnHazardImpact(10, collision.contacts[0].normal);
        }

        StartRespawnTimer();
    }

    void StartRespawnTimer()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        if (hazardRenderer != null)
        {
            hazardRenderer.enabled = false;
        }
        if (hazardCollider != null)
        {
            hazardCollider.enabled = false;
        }

        StartCoroutine(RespawnCoroutine());
    }

    //void DestroyHazard()
    //{
    //    // Play explosion prefab
    //    if (explosionPrefab != null)
    //    {
    //        Instantiate(explosionPrefab, transform.position, transform.rotation);
    //    }
    //    // Destroy this GameObject
    //    Destroy(gameObject);
    //}
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
        if (hazardRenderer != null)
        {
            hazardRenderer.enabled = true;
        }
        if (hazardCollider != null)
        {
            hazardCollider.enabled = true;
        }

        Debug.Log(gameObject.name + " has respawned.");
    }
    #endregion
}