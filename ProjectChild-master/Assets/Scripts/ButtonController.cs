using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Controls all the clickable buttons in the game.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {

    private CanvasSounds sounds;
    private Button thisButton;
    private Navigation customNav = new Navigation();

    // Button specific
    [Header("Mute clicking sound completely")]
    public bool muteClickSound = false;
    [Header("Play back sound instead of select sound")]
    public bool playBackSound = false;

    void Awake() {
        sounds = CanvasMaster.Instance.canvasSounds;
        thisButton = GetComponent<Button>();

        // Disable navigation mode for button
        customNav.mode = Navigation.Mode.None;
        thisButton.navigation = customNav;

        // Change button colors
        ColorBlock colors = thisButton.colors;
        colors.highlightedColor = new Color32(221, 221, 221, 255);
        thisButton.colors = colors;
    }

    /// <summary>
    /// Play a sound when the button is clicked.
    /// </summary>
    /// <param name="eventData">mouse event data</param>
    public void OnPointerClick(PointerEventData eventData) {
        if (muteClickSound)
            return;

        // Play backsound instead of select sound if inspector says so
        PlaySound(playBackSound ? sounds.BUTTON_BACK : sounds.BUTTON_SELECT);
    }

    /// <summary>
    /// Play the move sound when cursor enters the button.
    /// </summary>
    /// <param name="eventData">mouse event data</param>
    public void OnPointerEnter(PointerEventData eventData) {
        PlaySound(sounds.BUTTON_ENTER);
    }

    /// <summary>
    /// Plays a provided sound if able.
    /// </summary>
    /// <param name="sound">sound to play</param>
    private void PlaySound(AudioClip sound) {
        // If button is not interactable, disable all sounds
        if (!thisButton.interactable)
            return;

        sounds.PlaySound(sound);
    }
}
