/*=========================================================================/
 * Name: Crate.cs
 * Author: Connor Larsen
 * Date: 08/02/2025
 * 
 * Controls the EXPLOSION!
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Hazard Settings")]
    public GameObject billboardSprite;              // The visual sprite that will always face the camera
    public GameObject explosionPrefab;              // Public reference to the explosion animation prefab
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
            Debug.LogError("Crate script could not find the Main Camera! Please ensure a camera is tagged as 'MainCamera'");
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
            // Calculate the bounce-back direction
            Vector3 bounceDirection = collision.contacts[0].normal;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddBrokenCrate();
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            DestroyHazard();
        }
    }

    void DestroyHazard()
    {
        // Play explosion prefab
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
        // Destroy this GameObject
        Destroy(gameObject);
    }
    #endregion
}