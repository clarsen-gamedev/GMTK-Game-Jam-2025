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
    public GameObject explosionPrefab;              // Public reference to the explosion animation prefab
    #endregion

    #region Private Variables
    private Transform mainCameraTransform;
    private AudioSource boxBreakSource;
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

        boxBreakSource = GetComponent<AudioSource>();
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
        if (car != null)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddBrokenCrate();
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                boxBreakSource.clip = GameManager.Instance.crateBreak;
                boxBreakSource.Play();
            }

            StartCoroutine(DestroyHazard());
        }
    }

    IEnumerator DestroyHazard()
    {
        // Play explosion prefab
        if (explosionPrefab != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        yield return new WaitForSeconds(1f);

        // Destroy this GameObject
        Destroy(gameObject);
    }
    #endregion
}