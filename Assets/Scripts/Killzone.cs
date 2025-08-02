/*=========================================================================/
 * Name: Killzone.cs
 * Author: Connor Larsen
 * Date: 07/31/2025
 * 
 * Place this script on an invisible killzone under the track. When the
 * player hits this killzone, it will count as a death and respawn the
 * car.
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    #region Functions
    void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.Die();
        }
    }
    #endregion
}