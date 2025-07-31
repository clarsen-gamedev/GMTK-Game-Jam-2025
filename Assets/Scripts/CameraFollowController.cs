/*=========================================================================/
 * Name: CameraFollowController.cs
 * Author: Connor Larsen
 * Date: 07/30/2025
 * 
 * Attach this script to the Main Camera game object. It will follow a
 * target Transform (the player)
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Target Settings")]
    public Transform target;                    // The player object for the camera to follow
    public float lookAtHeightOffset = 1.0f;     // The vertical offset for the point the camera looks at

    [Header("Camera Positioning")]
    public float distance = 5.0f;               // The default distance the camera tries to stay from the player
    public float height = 2.0f;                 // The height of the camera above the player
    public float positionDampening = 5.0f;      // How quickly the camera moves to its target position (higher is faster)

    [Header("Camera Rotation")]
    public float rotationDampening = 4.0f;      // How quickly the camera rotates to align behind the player (higher is faster)

    [Header("Collision Handling")]
    public LayerMask collisionLayers;           // The layer that the camera will collide with (environment, level geometry, etc)
    public float collisionOffset = 0.3f;        // Small buffer distance to keep the camera from being flush with a wall
    #endregion

    #region Functions
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if a target has been assigned
        if (target == null)
        {
            Debug.LogError("CameraController: No target assigned! Assign the player's transform to the 'Target' field in the inspector");
            enabled = false;
        }
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        // Stop update if no target is selected
        if (!target)
        {
            return;
        }

        #region Calculate Desired Position and Rotation
        // Calculate the desired position and rotation (behind player at specified distance and height)
        Vector3 wantedPosition = target.position - (target.forward * distance) + (Vector3.up * height);

        // Calculate the desired rotation from the camera (look at a point slightly above player's head)
        Vector3 lookAtPoint = target.position + (Vector3.up * lookAtHeightOffset);
        Quaternion wantedRotation = Quaternion.LookRotation(lookAtPoint - wantedPosition, Vector3.up);
        #endregion


        #region Handle Camera Collisions
        // Cast a ray from the look-at point towards the desired camera position to check for obstacles like walls
        RaycastHit hit;
        if (Physics.Raycast(lookAtPoint, wantedPosition - lookAtPoint, out hit, distance, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            // Adjust the 'wantedPosition' to be at the collision point if the ray hits an obstacle
            wantedPosition = hit.point + (hit.normal * collisionOffset);
        }
        #endregion

        #region Smoothly Move and Rotate Camera
        // Interpolate camera's current position to the wanted position
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDampening * Time.deltaTime);

        // Interpolate camera's current rotation towards the rotation that looks at the player
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDampening * Time.deltaTime);
        #endregion
    }
    #endregion
}