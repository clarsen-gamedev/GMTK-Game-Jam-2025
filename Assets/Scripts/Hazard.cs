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
    public GameObject billboardSprite;              // The visual sprite that will always face the camera
    public ParticleSystem destructionParticles;     // The particle system to play when this hazard is destroyed
    #endregion

    #region Private Variables
    private Transform mainCameraTransform;
    #endregion

    #region Functions
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
        if (mainCameraTransform != null && billboardSprite != null)
        {
            // Calculate the direction from the hazard to the camera
            Vector3 directionToCamera = mainCameraTransform.position - billboardSprite.transform.position;
            directionToCamera.y = 0;

            // Look in the direction of the camera
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
                billboardSprite.transform.rotation = targetRotation;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CarController car = collision.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            car.TakeDamage(damageAmount);
            DestroyHazard();
        }
    }

    void DestroyHazard()
    {
        // Play destruction particle effects if assigned
        if (destructionParticles != null)
        {
            // Instantiate particles at the hazard's position and play them
            ParticleSystem particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            particles.Play();

            // Destroy the particle effect after its duration
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Destroy this GameObject
        Destroy(gameObject);
    }
    #endregion
}