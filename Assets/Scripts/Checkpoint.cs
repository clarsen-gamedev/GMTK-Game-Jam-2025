/*=========================================================================/
 * Name: Checkpoint.cs
 * Author: Connor Larsen
 * Date: 08/03/2025
 * 
 * Place this script on a checkpoint object with a Collider with Is
 * Trigger. The player will save the transform attached to this script as
 * it's new spawn point.
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    #region Public and Serialized Variables
    public Transform checkpoint;
    #endregion

    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            car.SetNewCheckpoint(checkpoint);
        }
    }
    #endregion
}
