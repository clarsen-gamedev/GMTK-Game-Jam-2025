/*=========================================================================/
 * Name: Gate.cs
 * Author: Connor Larsen
 * Date: 08/02/2025
 * 
 * Controls the EXPLOSION!
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int gateIndex = 0;
    public GameObject[] gatePieces;

    private Animator gateAnimator;

    void Awake()
    {
        gateAnimator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGateOpened += OpenGate;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGateOpened -= OpenGate;
        }
    }

    public void OpenGate(int openedGateIndex)
    {
        if (openedGateIndex == gateIndex)
        {
            Debug.Log("Gate " + gateIndex + " received open signal. Playing animation.");
            if (gateIndex == GameManager.Instance.CurrentGateIndex)
            {
                for (int i = 0; i < gatePieces.Length; i++)
                {
                    if (gatePieces[i].activeSelf)
                    {
                        gatePieces[i].SetActive(false);
                    }
                    else
                    {
                        gatePieces[i].SetActive(true);
                    }
                }
            }
        }
    }
}