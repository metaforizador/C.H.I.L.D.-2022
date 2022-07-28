using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

/// <summary>
/// Different states that the game can be in during gameplay.
/// </summary>
public enum GameState { Movement, Menu, Dialogue, Chest, Hotbar, ItemSelector, Dead };

/// <summary>
/// Controls the main aspects of the game.
/// </summary>
public class GameMaster : MonoBehaviour {
    
    // Make class static and destroy if script already exists
    private static GameMaster _instance; // **<- reference link to the class
    public static GameMaster Instance { get { return _instance; } }

    private void Awake() {
        // If instance not yet created
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private CanvasMaster cm;

    public const string SAVE_PATH = "/gamesave.save";
    public Save latestSave { get; private set; }

    // Handle game state
    public GameState gameState { get; private set; }
    /// <summary>
    /// Toggles different values depending of the game state.
    /// </summary>
    /// <param name="state">current game state</param>
    public void SetState(GameState state) {
        gameState = state;
        cm = CanvasMaster.Instance;

        // Show crosshair if state is movement
        cm.ShowCrosshair(gameState.Equals(GameState.Movement) ? true : false);

        // Pause time if gamestate is not movement
        Time.timeScale = gameState.Equals(GameState.Movement) ? 1 : 0;

        // Hide cursor only when the state is movement
        ShowCursor(gameState.Equals(GameState.Movement) ? false : true);

        switch (gameState) {
            case GameState.Movement:
                cm.ShowCanvasBackround(false);
                cm.ShowHUDCanvas(true);
                break;
            case GameState.Menu:
            case GameState.Hotbar:
                cm.ShowCanvasBackround(true);
                cm.ShowHUDCanvas(true);
                break;
            case GameState.Dialogue:
            case GameState.Chest:
                cm.ShowCanvasBackround(true);
                cm.ShowHUDCanvas(false);
                break;
            case GameState.Dead:
                cm.ShowHUDCanvas(false);
                break;
            case GameState.ItemSelector:
                cm.ShowCanvasBackround(true);
                break;
        }
    }

    // Handle cursor
    private bool cursorVisible;
    private void ShowCursor(bool show) {
        cursorVisible = show;
        Cursor.visible = cursorVisible;

        // Toggles the lock mode depending of the cursor visibility
        if (cursorVisible)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Start() {
        cm = CanvasMaster.Instance;
    }

    /// <summary>
    /// Creates a save game object which holds all the save variables.
    /// </summary>
    /// <returns>save object which has it's values changed</returns>
    private Save CreateSaveGameObject() {
        Save save = new Save();
        // Save values of other classes
        PlayerStats.Instance.SavePlayerStats(save);
        CanvasMaster.Instance.SaveCanvasValues(save);
        Inventory.Instance.SaveInventory(save);
        Storage.Instance.SaveStorage(save);

        // Save Scene name
        save.sceneName = SceneManager.GetActiveScene().name;

        return save;
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    public void SaveGame() {
        Save save = CreateSaveGameObject();

        // Format and save to a file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + SAVE_PATH);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void LoadGame() {
        if (File.Exists(Application.persistentDataPath + SAVE_PATH)) {
            // If file exists, deserialize the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + SAVE_PATH, FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            latestSave = save; // Player needs to access this when it spawns

            // Load saved scene (Needs to be called before loading values from other classes)
            SceneManager.LoadScene(save.sceneName);

            // Load values of other classes
            PlayerStats.Instance.LoadPlayerStats(save);
            CanvasMaster.Instance.LoadCanvasValues(save);
            Inventory.Instance.LoadInventory(save);
            Storage.Instance.LoadStorage(save);

            file.Close();

            Debug.Log("Game Loaded");
        } else {
            Debug.Log("No game saved!");
        }
    }

    public bool CheckIfSaveExists() {
        return File.Exists(Application.persistentDataPath + SAVE_PATH);
    }

    /// <summary>
    /// Moves back to the main menu, resetting singletons.
    /// </summary>
    public void BackToMainMenu() {
        // Destroy singletons when moving to main menu so necessary values
        // gets reset
        DestroyImmediate(GameMaster.Instance.gameObject);
        DestroyImmediate(CanvasMaster.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
