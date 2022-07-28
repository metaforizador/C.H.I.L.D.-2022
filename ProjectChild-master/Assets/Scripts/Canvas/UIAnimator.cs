using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates the UI elements.
/// 
/// Sets an ease for all the animations and 
/// plays them even when Timescale is set to 0.
/// </summary>
public class UIAnimator : MonoBehaviour {

    public LTDescr MoveX(GameObject obj, float to, float time, LeanTweenType ease) {
        return LeanTween.moveLocalX(obj, to, time).setEase(ease).setIgnoreTimeScale(true);
    }

    public LTDescr MoveY(GameObject obj, float to, float time, LeanTweenType ease) {
        return LeanTween.moveLocalY(obj, to, time).setEase(ease).setIgnoreTimeScale(true);
    }

    public LTDescr Scale(GameObject obj, Vector3 to, float time, LeanTweenType ease) {
        return LeanTween.scale(obj, to, time).setEase(ease).setIgnoreTimeScale(true);
    }
}
