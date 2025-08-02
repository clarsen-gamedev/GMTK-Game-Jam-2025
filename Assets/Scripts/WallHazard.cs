/*=========================================================================/
 * Name: WallHazard.cs
 * Author: Connor Larsen
 * Date: 08/01/2025
 * 
 * Place this script on a wall object with a Collider. The player will take
 * damage when hit by this object. Works similarly to Hazard.cs, except
 * doesn't destroy itself on impact.
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHazard : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Hazard Settings")]
    public int damageAmount = 10;   // The amount of damage this hazard will deal to the car
    #endregion

    #region Functions
    void OnCollisionEnter(Collision collision)
    {
        CarController car = collision.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            // Calculate the bounce-back direction
            Vector3 bounceDirection = collision.contacts[0].normal;

            // Take damage
            car.OnHazardImpact(damageAmount, bounceDirection);
        }
    }
    #endregion
}
