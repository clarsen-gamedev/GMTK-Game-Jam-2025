using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCar : MonoBehaviour
{
    #region Public Variables
    public Transform targetDestination;
    #endregion

    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            car.transform.position = targetDestination.position;
            car.transform.rotation = targetDestination.rotation;
            car.SetNewCheckpoint(targetDestination);
        }
    }
    #endregion
}