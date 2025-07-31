/*=========================================================================/
 * Name: CarController.cs
 * Author: Connor Larsen
 * Date: 07/30/2025
 * Description: Controls the car
/=========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Car Settings")]
    public float moveSpeed = 15f;
    public float turnSpeed = 15f;
    #endregion

    #region Privtae Variables
    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private bool isGrounded;
    #endregion

    #region Functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}