using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFeedbackUI : MonoBehaviour
{
    public GameObject full; // GameObject chứa Image + Text
    public float displayDuration = 1.5f;

    private Coroutine feedbackRoutine;

    public void ShowFeedback()
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        full.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        full.SetActive(false);
    }
}


