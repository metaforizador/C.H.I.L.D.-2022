using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Controls the moving of scroll rect with up and down buttons.
/// 
/// This script was downloaded from unity forums, but I modified it drastically.
/// </summary>
[RequireComponent(typeof(Button))]
public class ScrollbarIncrementer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Scrollbar target;
    private float step = 0.05f; // Amount of steps one click moves the rect

    private float holdFrequency = 0.05f; // Seconds to wait when holding
    public bool increment;

    void Start() {
        // Enable / disable button based on scrollbar value
        target.onValueChanged.AddListener((value) => {
            GetComponent<Button>().interactable = increment ? target.value < 1 : target.value > 0;
        });
    }

    /// <summary>
    /// Increment and decrement the scroll wheel value.
    /// </summary>
    public void MoveScroll() {
        if (target == null) throw new Exception("Setup ScrollbarIncrementer first!");
        float value = increment ? target.value + step : target.value - step;
        target.value = Mathf.Clamp(value, 0, 1);
    }

    /// <summary>
    /// Moves the scroll area in a given frequency.
    /// </summary>
    /// <param name="increment">whether to increment or decrement</param>
    /// <returns>IEnumerator</returns>
    IEnumerator IncrementDecrementSequence(bool increment) {
        MoveScroll();
        yield return new WaitForSecondsRealtime(holdFrequency);
        StartCoroutine("IncrementDecrementSequence", increment);
    }

    /// <summary>
    /// Starts moving the scroll area.
    /// </summary>
    /// <param name="eventData">mouse event data</param>
    public void OnPointerDown(PointerEventData eventData) {
        StartCoroutine("IncrementDecrementSequence", increment);
    }

    /// <summary>
    /// Stops moving the scroll area.
    /// </summary>
    /// <param name="eventData">mouse even data</param>
    public void OnPointerUp(PointerEventData eventData) {
        StopCoroutine("IncrementDecrementSequence");
    }
}