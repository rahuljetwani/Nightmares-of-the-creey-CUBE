using UnityEngine;

public class EyeSelectionPersistence : MonoBehaviour
{
    [Header("Persistence Settings")]
    //[SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private string saveKey = "SelectedEyeIndex";

    private static EyeSelectionPersistence instance;

    public static EyeSelectionPersistence Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EyeSelectionPersistence>();
            }
            return instance;
        }
    }
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //if (dontDestroyOnLoad)
            //{
            //    DontDestroyOnLoad(gameObject);
            //}
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SaveEyeSelection(int eyeIndex)
    {
        PlayerPrefs.SetInt(saveKey, eyeIndex);
        PlayerPrefs.Save();
        Debug.Log($"Persistence: Saved eye index {eyeIndex}");
    }

    public int LoadEyeSelection()
    {
        int savedIndex = PlayerPrefs.GetInt(saveKey, 0);
        Debug.Log($"Persistence: Loaded eye index {savedIndex}");
        return savedIndex;
    }

    public bool HasSavedSelection()
    {
        return PlayerPrefs.HasKey(saveKey);
    }

    public void ClearSavedSelection()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
            Debug.Log("Persistence: Cleared saved eye selection");
        }
    }
}
