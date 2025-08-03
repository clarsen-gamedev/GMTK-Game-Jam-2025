using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate1Disappear : MonoBehaviour
{
    #region Public Variables
    public GameObject gate1;
    #endregion

    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.gameObject.GetComponent<CarController>();
        if (car != null)
        {
            gate1.SetActive(false);
        }
    }
    #endregion
}
