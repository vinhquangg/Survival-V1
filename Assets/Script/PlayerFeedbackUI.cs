using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeedbackType
{
    None,
    Full,
    EatMeat,
    RawMeat,
    InventoryFull,
    CannotDrink,
    Cook,
    NeedAxe,
    OpenInventory,
    OpenCrafting,
    CollectItemFirstTime,
}

public class PlayerFeedbackUI : MonoBehaviour
{
    [System.Serializable]
    public class FeedbackEntry
    {
        public FeedbackType type;
        public GameObject feedbackObject;
        public float duration;
    }

    public List<FeedbackEntry> feedbackEntries = new();   
    private Dictionary<FeedbackType, FeedbackEntry> feedbackDict;
    private Coroutine feedbackRoutine;

    public bool firstInventoryFeedbackShown = false;
    public bool firstCraftFeedbackShown = false;
    public bool firstItemPicked = false;
    public bool waitingForInventoryOpen = false;
    //public float displayDuration = 1.5f;

    private void Awake()
    {

        feedbackDict = new Dictionary<FeedbackType, FeedbackEntry>();
        foreach (var entry in feedbackEntries)
        {
            if (!feedbackDict.ContainsKey(entry.type) && entry.feedbackObject != null)
                feedbackDict.Add(entry.type, entry);
        }
    }

    public void ShowFeedbackUntilKeyPress(FeedbackType type, KeyCode key)
    {
        StopAllCoroutines(); // dừng feedback cũ
        StartCoroutine(ShowUntilKeyPressRoutine(type, key));
    }

    private IEnumerator ShowUntilKeyPressRoutine(FeedbackType type, KeyCode key)
    {
        if (!feedbackDict.ContainsKey(type))
        {
            Debug.LogWarning($"⚠ Feedback type {type} chưa được setup trong Inspector.");
            yield break;
        }

        var entry = feedbackDict[type];
        entry.feedbackObject.SetActive(true);

        // chờ nhấn phím
        while (!Input.GetKeyDown(key))
        {
            yield return null;
        }

        entry.feedbackObject.SetActive(false);
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
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.noitifySound);
    }

    public void HideFeedback(FeedbackType type)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        if (feedbackDict.ContainsKey(type))
            feedbackDict[type].feedbackObject.SetActive(false);
    }


    private IEnumerator ShowRoutine(FeedbackType type)
    {
        var entry = feedbackDict[type];
        entry.feedbackObject.SetActive(true);

        yield return new WaitForSeconds(entry.duration);

        entry.feedbackObject.SetActive(false);
    }
}
