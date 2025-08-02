/*=========================================================================/
 * Name: ExplosionManager.cs
 * Author: Connor Larsen
 * Date: 08/01/2025
 * 
 * Controls the EXPLOSION!
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public float animationDuration = 0.7f;  // Public variable to set the duration of the explosion animation

    private Transform mainCameraTransform;

    private void Start()
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

        // Calculate the direction from the hazard to the camera
        Vector3 directionToCamera = mainCameraTransform.position - transform.position;
        directionToCamera.y = 0;

        // Look in the direction of the camera
        if (directionToCamera != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
            transform.rotation = targetRotation;
        }

        Destroy(gameObject, animationDuration);
    }
}