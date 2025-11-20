using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeDisplayController : MonoBehaviour
{
    [Header("Display Configuration")]
    [SerializeField] private Image eyeDisplayImage;
    [SerializeField] private SpriteRenderer eyeSpriteRenderer;

    [Header("Auto-Detection")]
    [SerializeField] private bool autoDetectComponents = true;

    void Start()
    {
        if (autoDetectComponents)
        {
            AutoDetectComponents();
        }

        // Subscribe to events
        EyeSelectionManager.OnEyeSelectionChanged += UpdateEyeDisplay;

        // Load with delay
        Invoke("LoadCurrentEyeSelection", 0.3f);
    }

    void AutoDetectComponents()
    {
        if (eyeDisplayImage == null)
            eyeDisplayImage = GetComponent<Image>();

        if (eyeSpriteRenderer == null)
            eyeSpriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Log($"EyeDisplayController: Image={eyeDisplayImage?.name}, SpriteRenderer={eyeSpriteRenderer?.name}");
    }

    void LoadCurrentEyeSelection()
    {
        // Try to get from manager first
        EyeSelectionManager manager = FindObjectOfType<EyeSelectionManager>();
        if (manager != null)
        {
            Sprite currentSprite = manager.GetCurrentEyeSprite();
            if (currentSprite != null)
            {
                UpdateEyeDisplay(currentSprite);
                return;
            }
        }

        // Fallback: load from PlayerPrefs
        int savedIndex = PlayerPrefs.GetInt("SelectedEyeIndex", 0);
        Debug.Log($"Loading eye display from saved index: {savedIndex}");
    }

    void UpdateEyeDisplay(Sprite newEyeSprite)
    {
        if (newEyeSprite == null)
        {
            Debug.LogWarning("Trying to update eye display with no sprite !!");
            return;
        }

        bool updated = false;

        if (eyeDisplayImage != null)
        {
            eyeDisplayImage.sprite = newEyeSprite;
            updated = true;
        }

        if (eyeSpriteRenderer != null)
        {
            eyeSpriteRenderer.sprite = newEyeSprite;
            updated = true;
        }

        Debug.Log($"Eye display updated: {newEyeSprite.name}, Success: {updated}");
    }

    void OnDestroy()
    {
        EyeSelectionManager.OnEyeSelectionChanged -= UpdateEyeDisplay;
    }
}