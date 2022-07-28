using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays the main menu elements.
/// </summary>
public class MainMenuCanvas : MonoBehaviour {

    [SerializeField]
    private Button continueButton, loadGameButton;

    // Speech
    [SerializeField]
    private AudioClip[] speeches;

    void Start() {
        // Disable continue and load game buttons if save does not exist
        bool saveExists = GameMaster.Instance.CheckIfSaveExists();
        continueButton.interactable = saveExists;
        loadGameButton.interactable = saveExists;

        Invoke("PlayRandomAudio", 0.5f);
    }

    /// <summary>
    /// Plays a random speech when the main menu opens.
    /// </summary>
    private void PlayRandomAudio() {
        AudioClip sound = speeches[Random.Range(0, speeches.Length)];
        CanvasMaster.Instance.canvasSounds.PlaySound(sound);
    }

    /// <summary>
    /// Starts a new game.
    /// </summary>
    public void NewGame() {
        // Hide this canvas when changing scene
        gameObject.SetActive(false);

        // Number 1 should be the first scene in game after the main menu
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the latest saved game.
    /// </summary>
    public void Continue() {
        // Hide this canvas when changing scene
        gameObject.SetActive(false);

        GameMaster.Instance.LoadGame();
    }

    /// <summary>
    /// Closes the game.
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }
}
