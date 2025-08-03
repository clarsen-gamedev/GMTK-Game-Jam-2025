/*=========================================================================/
 * Name: CycleCounter.cs
 * Author: Connor Larsen
 * Date: 08/03/2025
 * 
 * Place this script on an empty game object. This object will persist
 * across scene reloads and will keep track of how many times the game
 * has reset.
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleCounter : MonoBehaviour
{
    #region Static Instance
    public static CycleCounter CounterInstance { get; private set; }

    private void Awake()
    {
        if (CounterInstance == null)
        {
            CounterInstance = this;
            DontDestroyOnLoad(gameObject);  // Persist the manager across scenes (if necessary)
        }
        else
        {
            Destroy(gameObject);            // Makes sure only one GameManager exists at all times
        }
    }
    #endregion

    #region Public Variables
    public int cycleCount = 0;
    public int loopCount = 0;
    public int deathCount = 0;
    #endregion

    #region Functions
    public void IncreaseCycleCount()
    {
        cycleCount++;
    }
    #endregion
}