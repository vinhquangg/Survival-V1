﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FeedbackType
{
    None,
    Full,
    EatMeat,
    RawMeat,
    InventoryFull,
    CannotDrink
}

public class PlayerFeedbackUI : MonoBehaviour
{
    public GameObject full; // GameObject chứa Image + Text
    public float displayDuration = 1.5f;
    [System.Serializable]
    public class FeedbackEntry
    {
        public FeedbackType type;
        public GameObject feedbackObject;
    }

    public List<FeedbackEntry> feedbackEntries = new();
    private Dictionary<FeedbackType, GameObject> feedbackDict;

    private Coroutine feedbackRoutine;

    private void Awake()
    {

        feedbackDict = new Dictionary<FeedbackType, GameObject>();
        foreach (var entry in feedbackEntries)
        {
            if (!feedbackDict.ContainsKey(entry.type) && entry.feedbackObject != null)
                feedbackDict.Add(entry.type, entry.feedbackObject);
        }
    }

    public void ShowFeedback(FeedbackType type)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        if (!feedbackDict.ContainsKey(type))
        {
            Debug.LogWarning($"⚠ Feedback type {type} chưa được setup trong Inspector.");
            return;
        }

        feedbackRoutine = StartCoroutine(ShowRoutine(type));
    }

    private IEnumerator ShowRoutine(FeedbackType type)
    {
        full.SetActive(true);
        GameObject obj = feedbackDict[type];
        obj.SetActive(true);

        yield return new WaitForSeconds(displayDuration);
        full.SetActive(false);

        obj.SetActive(false);
    }
}

