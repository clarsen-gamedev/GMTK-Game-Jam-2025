/*=========================================================================/
 * Name: GameManager.cs
 * Author: Connor Larsen
 * Date: 07/31/2025
 * 
 * Manages many game variables
/=========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Runtime.CompilerServices;

public enum GameState
{
    READY,      // Before game has started (show countdown?)
    PLAYING,    // The game is actively being played
    PAUSED,     // The game is paused
    GAMEOVER,   // The game has ended
    NONE
}

public class GameManager : MonoBehaviour
{
    #region Static Instance
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist the manager across scenes (if necessary)
        }
        else
        {
            Destroy(gameObject);            // Makes sure only one GameManager exists at all times
        }
    }
    #endregion

    #region Public and Serialized Variables
    // Public properties to read the private variables
    public GameState CurrentGameState { get { return currentGameState; } }
    public float ElapsedTime { get { return elapsedTime; } }
    public int LoopsCompleted { get { return loopsCompleted; } }
    public int Deaths {  get { return deaths; } }

    // Events to notify other scripts of changes
    public Action OnGameStart;
    public Action<int> OnLoopCompleted;
    public Action<int> OnDeath;
    public Action OnGameOver;

    // UI References
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI loopsText;
    public TextMeshProUGUI deathsText;
    #endregion

    #region Private Variables
    private GameState currentGameState;
    private float elapsedTime = 0f;
    private int loopsCompleted = 0;
    private int deaths = 0;
    #endregion

    #region Functions
    void Start()
    {
        // Set the initial game state
        currentGameState = GameState.READY;
        Debug.Log("GameManager initialized. State: " + currentGameState);

        // Start countdown coroutine
        StartCoroutine(StartCountdown(3));
    }

    void Update()
    {
        // The timer only runs when the game is in the 'Playing' state
        if (currentGameState == GameState.PLAYING)
        {
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }
    }

    public void StartGame()
    {
        if (currentGameState == GameState.READY)
        {
            currentGameState = GameState.PLAYING;
            Debug.Log("Game Started!");

            // Invoke the event to notify any listeners
            OnGameStart?.Invoke();
        }
    }

    public void AddLoop()
    {
        if (currentGameState == GameState.PLAYING)
        {
            loopsCompleted++;
            Debug.Log("Loop Completed! Total loops: " + loopsCompleted);

            // Invoke the event to notify any listeners
            OnLoopCompleted?.Invoke(loopsCompleted);
        }
    }

    public void AddDeath()
    {
        if (currentGameState == GameState.PLAYING)
        {
            deaths++;
            Debug.Log("Player Died. Total deaths: " + deaths);

            // Invoke the event to notify any listeners
            OnDeath?.Invoke(deaths);
        }
    }

    public void EndGame()
    {
        if (currentGameState == GameState.PLAYING)
        {
            currentGameState = GameState.GAMEOVER;
            Debug.Log("Game Over! Final Time: " + elapsedTime + ", Loops: " + loopsCompleted + ", Total Deaths: " + deaths);

            // Invoke the event to notify any listeners
            OnGameOver?.Invoke();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            // Calculate minutes, seconds, and milliseconds from the elapsed time
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            int milliseconds = (int)((elapsedTime * 1000) % 1000);

            // Format the string to MM:SS:MS, ensuring leading zeros
            string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

            timerText.text = formattedTime;
        }
        if (loopsText != null)
        {
            loopsText.text = "Loops: " + loopsCompleted.ToString();
        }
        if (deathsText != null)
        {
            deathsText.text = "Deaths: " + deaths.ToString();
        }
    }
    #endregion

    #region Coroutines
    IEnumerator StartCountdown(float duration)
    {
        Debug.Log("Countdown starting...");
        yield return new WaitForSeconds(duration);
        StartGame();
    }
    #endregion
}