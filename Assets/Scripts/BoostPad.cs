/*=========================================================================/
 * Name: BoostPad.cs
 * Author: Connor Larsen
 * Date: 07/30/2025
 * 
 * Attach to the boost pads on the track, when the car moves over one, they
 * will gain a momentary boost of speed before returning to their base
 * speed
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    #region Public and Serialized Variables
    [Header("Boost Settings")]
    public float boostMultiplier = 1.8f;    // Multiplier for the car's speed and acceleration during the boost
    public float boostDuration = 2.0f;      // Duration of the boost effect in seconds

    [Header("Visuals/Effects")]
    public ParticleSystem boostParticles;   // Particle system to play when the boost pad is activated
    public AudioClip boostSound;            // Audio clip to play when the boost pad is activated
    #endregion

    #region Private Variables
    private AudioSource audioSource;
    #endregion

    #region Functions
    void Awake()
    {
        // Check to see if the BoostPad has a Collider and set to Is Trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("BoostPad requires a Collider component.", this);
            enabled = false;
            return;
        }
        if (!col.isTrigger)
        {
            Debug.LogWarning("BoostPad's Collider is not set to 'Is Trigger'. Setting it now.", this);
            col.isTrigger = true;
        }

        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has a CarController component
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            // Apply the boost effect to the car
            car.ApplyBoost(boostMultiplier, boostDuration);

            // Play effect particles
            if (boostParticles != null)
            {
                boostParticles.Play();
            }

            // Play boost sound
            if (boostSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(boostSound);
            }
        }
    }
    #endregion
}