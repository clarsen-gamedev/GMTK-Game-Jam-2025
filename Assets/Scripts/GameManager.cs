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
using UnityEngine.UI;
using System;
using TMPro;
using System.Runtime.CompilerServices;

public enum GameState
{
    READY,          // Before game has started (show countdown?)
    PLAYING,        // The game is actively being played
    RESPAWNING,     // The player has died and needs to respawn
    GATEOPENING,    // The player has opened a gate and cannot move
    PAUSED,         // The game is paused
    GAMEOVER,       // The game has ended
    NONE
}

public enum UIScreen
{
    GAME,
    PAUSE,
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
    public UIScreen CurrentScreen { get { return currentScreen; } }
    public float ElapsedTime { get { return elapsedTime; } }
    public int LoopsCompleted { get { return loopsCompleted; } }
    public int Deaths {  get { return deaths; } }
    public int CratesBroken { get { return currentCratesBroken; } }

    // Events to notify other scripts of changes
    public Action OnGameStart;
    public Action<int> OnLoopCompleted;
    public Action<int> OnDeath;
    public Action OnGameOver;
    public Action<int> OnGateOpened;
    public Action<int> OnCrateBroken;

    // UI References
    [Header("UI Panels")]
    public GameObject gameUI;
    public GameObject pauseUI;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI loopsText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI cratesBrokenText;
    public Slider healthSlider;

    // Gate Logic Settings
    [Header("Gate Logic")]
    public int[] cratesRequiredForGate;
    public float gateOpenDuration = 3.0f;
    #endregion

    // Game Sounds & Music
    [Header("Game Music")]
    public AudioClip backgroundMusic;
    public AudioSource musicSource;

    [Header("Game Sounds")]
    public AudioClip explosion;
    public AudioClip carDriving;
    public AudioClip crateBreak;
    public AudioClip gateOpen;
    public AudioClip boostPad;
    public AudioClip pickupSound;
    public AudioClip wallBump;
    public AudioClip loopComplete;

    #region Private Variables
    private GameState currentGameState;
    private UIScreen currentScreen;
    private bool isPaused = false;
    private float elapsedTime = 0f;
    private int loopsCompleted = 0;
    private int deaths = 0;
    private int currentGateIndex = 0;
    private int currentCratesBroken = 0;
    #endregion

    #region Functions
    void Start()
    {
        // Set the initial game state
        currentGameState = GameState.READY;
        Debug.Log("GameManager initialized. State: " + currentGameState);

        // Enable the game UI on startup
        currentScreen = UIScreen.GAME;
        UISwitch(UIScreen.GAME);

        SetupUI();
        UpdateCratesBrokenUI();

        // Start countdown coroutine
        StartCoroutine(StartCountdown(3));
    }

    void SetupUI()
    {
        CarController carController = FindObjectOfType<CarController>();
        if (carController != null )
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = carController.maxHealth;
            }

            carController.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(carController.maxHealth);
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void UpdateCratesBrokenUI()
    {
        if (cratesBrokenText != null && cratesRequiredForGate.Length > currentGateIndex)
        {
            cratesBrokenText.text = "Crates: " + CratesBroken + "/" + cratesRequiredForGate[currentGateIndex];
        }
        else if (cratesBrokenText != null)
        {
            cratesBrokenText.text = "Crates: " + CratesBroken + "/--";
        }
    }

    public void AddBrokenCrate()
    {
        if (currentGameState != GameState.PLAYING) return;

        currentCratesBroken++;
        OnCrateBroken?.Invoke(currentCratesBroken);
        UpdateCratesBrokenUI();

        if (currentGateIndex < cratesRequiredForGate.Length && currentCratesBroken >= cratesRequiredForGate[currentGateIndex])
        {
            Debug.Log("Gate " + currentGateIndex + " opened.");
            StartCoroutine(OpenGate(currentGateIndex));
        }
    }

    void Update()
    {
        // The timer only runs when the game is in the 'Playing' state
        if (currentGameState == GameState.PLAYING || currentGameState == GameState.RESPAWNING)
        {
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }

        #region Controls
        // Press ESC = Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Only function if in the GAME or PAUSE screen
            if (currentScreen == UIScreen.GAME || currentScreen == UIScreen.PAUSE)
            {
                // If game is paused, resume game
                if (isPaused)
                {
                    ResumeGame();
                }
                // If game is active, pause game
                else
                {
                    PauseGame();
                }
            }
        }
        #endregion
    }

    #region Menu Functions
    public void ResumeGame()
    {
        // Switch to the game screen
        UISwitch(UIScreen.GAME);

        // Resume game time
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        // Switch to the pause screen
        UISwitch(UIScreen.PAUSE);

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game time
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    public void StartGame()
    {
        if (currentGameState == GameState.READY)
        {
            currentGameState = GameState.PLAYING;
            HandleMusic(true);
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
        if (currentGameState == GameState.RESPAWNING)
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

    void UISwitch(UIScreen screen)
    {
        // Game UI
        if (screen == UIScreen.GAME)
        {
            // Disable all other screens
            pauseUI.SetActive(false);

            // Enable only the game screen
            gameUI.SetActive(true);
        }

        // Pause UI
        else if (screen == UIScreen.PAUSE)
        {
            // Disable all other screens
            gameUI.SetActive(false);

            // Enable only the pause screen
            pauseUI.SetActive(true);
        }
    }

    public void GameStateSwitch(GameState state)
    {
        currentGameState = state;
    }

    public void HandleMusic(bool isPlaying)
    {
        if (musicSource == null || backgroundMusic == null) return;

        if (isPlaying && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (!isPlaying && musicSource.isPlaying)
        {
            musicSource.Stop();
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

    public IEnumerator StartRespawn(float duration)
    {
        currentGameState = GameState.RESPAWNING;
        Debug.Log("Respawn countdown starting...");
        yield return new WaitForSeconds(duration);
        currentGameState = GameState.PLAYING;
        HandleMusic(true);
    }

    public IEnumerator OpenGate(int gateIndex)
    {
        currentGameState = GameState.GATEOPENING;
        OnGateOpened?.Invoke(currentGateIndex);

        yield return new WaitForSeconds(gateOpenDuration);

        currentCratesBroken = 0;
        currentGateIndex++;
        UpdateCratesBrokenUI();
        currentGameState = GameState.PLAYING;
        HandleMusic(true);
    }
    #endregion
}