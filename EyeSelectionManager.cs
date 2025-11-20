using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EyeSelectionManager : MonoBehaviour
{

    [Header("Eye Selection Configuration")]
    [SerializeField] private List<GameObject> eyeButtonObjects = new List<GameObject>();
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color deselectedColor = Color.white;

    [Header("Database Settings")]
    [SerializeField] private string saveKey = "SelectedEyeIndex";

    private int currentSelectedIndex = 0;
    private List<EyeSelectionButton> eyeButtons = new List<EyeSelectionButton>();

    public static System.Action<Sprite> OnEyeSelectionChanged;


    void Start()
    {
        RefreshButtonReferences();
        CompleteSetup();
    }

    void RefreshButtonReferences()
    {
        eyeButtonObjects.Clear();

        // NAMING PATTERN IS IMP------

        string[] buttonNames = {
        "EyePanelButton_01", "EyePanelButton_02", "EyePanelButton_03", "EyePanelButton_04",
        "EyePanelButton_05", "EyePanelButton_06", "EyePanelButton_07", "EyePanelButton_08"
    };

        foreach (string buttonName in buttonNames)
        {
            GameObject buttonObj = GameObject.Find(buttonName);
            if (buttonObj != null)
            {
                eyeButtonObjects.Add(buttonObj);
                Debug.Log($"Found: {buttonName}");
            }
            else
            {
                Debug.LogWarning($"Missing: {buttonName}");
            }
        }

        Debug.Log($"Total buttons found: {eyeButtonObjects.Count}");
    }

    void CompleteSetup()
    {

        SetupEyeButtons();
        InitializeEyeSelection();
        LoadSelectedEye();

        Debug.Log($"Eye Selection Manager initialized with {eyeButtons.Count} buttons");
    }

    void SetupEyeButtons()
    {
        eyeButtons.Clear();

        foreach (GameObject buttonObj in eyeButtonObjects)
        {
            if (buttonObj != null)
            {
                EyeSelectionButton eyeButton = buttonObj.GetComponent<EyeSelectionButton>();
                if (eyeButton == null)
                {
                    eyeButton = buttonObj.AddComponent<EyeSelectionButton>();
                }
                eyeButtons.Add(eyeButton);
            }
        }
    }

    void InitializeEyeSelection()
    {
        for (int i = 0; i < eyeButtons.Count; i++)
        {
            int buttonIndex = i;
            eyeButtons[i].Initialize(buttonIndex, OnEyeButtonClicked);
        }
    }

    void OnEyeButtonClicked(int eyeIndex)
    {
        if (eyeIndex < 0 || eyeIndex >= eyeButtons.Count) return;

        currentSelectedIndex = eyeIndex;
        UpdateButtonStates();
        SaveSelectedEye();

        Sprite selectedSprite = eyeButtons[currentSelectedIndex].GetEyeSprite();
        OnEyeSelectionChanged?.Invoke(selectedSprite);

        Debug.Log($"Eye selected: {eyeIndex}");
    }

    void UpdateButtonStates()
    {
        for (int i = 0; i < eyeButtons.Count; i++)
        {
            bool isSelected = (i == currentSelectedIndex);
            Color bgColor = isSelected ? selectedColor : deselectedColor;
            eyeButtons[i].SetBackgroundColor(bgColor);
        }
    }

    void SaveSelectedEye()
    {
        PlayerPrefs.SetInt(saveKey, currentSelectedIndex);
        PlayerPrefs.Save();
    }

    void LoadSelectedEye()
    {
        currentSelectedIndex = PlayerPrefs.GetInt(saveKey, 0);

        if (currentSelectedIndex >= eyeButtons.Count)
            currentSelectedIndex = 0;

        UpdateButtonStates();

        if (eyeButtons.Count > 0 && eyeButtons[currentSelectedIndex] != null)
        {
            Sprite currentSprite = eyeButtons[currentSelectedIndex].GetEyeSprite();
            OnEyeSelectionChanged?.Invoke(currentSprite);
        }
    }

    public Sprite GetCurrentEyeSprite()
    {
        if (currentSelectedIndex >= 0 && currentSelectedIndex < eyeButtons.Count)
        {
            return eyeButtons[currentSelectedIndex].GetEyeSprite();
        }
        return null;
    }

    public int GetCurrentEyeIndex()
    {
        return currentSelectedIndex;
    }

    void OnEnable()
    {
        
        if (Application.isPlaying)
        {
            Invoke("RefreshAfterSceneLoad", 0.2f);
        }
    }

    void RefreshAfterSceneLoad()
    {
        RefreshButtonReferences();
        if (eyeButtonObjects.Count > 0)
        {
            CompleteSetup();
        }
    }
}