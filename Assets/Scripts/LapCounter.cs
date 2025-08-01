/*=========================================================================/
 * Name: LapCounter.cs
 * Author: Connor Larsen
 * Date: 07/31/2025
 * 
 * When the car passes through a collision trigger with this attached, the
 * lap counter in the game manager will increase by 1
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    #region Functions
    void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            GameManager.Instance.AddLoop();
        }
    }
    #endregion
}