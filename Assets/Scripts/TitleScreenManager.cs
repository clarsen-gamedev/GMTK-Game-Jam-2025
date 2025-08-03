/*=========================================================================/
 * Name: TitleScreenManager.cs
 * Author: Connor Larsen
 * Date: 08/19/2024
 * 
 * Controls the function of the game's title screen
/=========================================================================*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("UI Screens")]
    [SerializeField] GameObject titleUI;        // Reference to the screen to play and quit the game
    [SerializeField] GameObject tutorialUI;     // Reference to the screen which shows the wizard's introduction
    [SerializeField] GameObject creditsUI;      // Reference to the screen which shows the credits
    #endregion

    #region Functions
    // Start is called before the first frame update
    void Start()
    {
        // Resume game time
        Time.timeScale = 1f;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region Button Functions
    // Start Game Button
    public void StartGame()
    {
        GetComponent<AudioSource>().Play();         // Play the button click sound effect
        SceneManager.LoadScene("Connor Test");      // Load the first level of the game
    }

    // Quit Game Button
    public void QuitGame()
    {
        GetComponent<AudioSource>().Play();         // Play the button click sound effect
        Application.Quit();                         // Quit the game
    }

    public void ButtonTest()
    {
        Debug.Log("Button Pressed");
    }
    #endregion
}