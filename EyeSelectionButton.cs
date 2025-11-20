using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EyeSelectionButton : MonoBehaviour
{
    [Header("Button Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image eyeImage;

    private int buttonIndex;
    private System.Action<int> onClickCallback;

    void Awake()
    {
        // Auto-assign components if not set 
        if (button == null) button = GetComponent<Button>();
        if (backgroundImage == null) backgroundImage = GetComponent<Image>();
    }

    public void Initialize(int index, System.Action<int> clickCallback)
    {
        buttonIndex = index;
        onClickCallback = clickCallback;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        onClickCallback?.Invoke(buttonIndex);
    }

    public void SetBackgroundColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }

    public Sprite GetEyeSprite()
    {
        if (eyeImage != null)
        {
            return eyeImage.sprite;
        }
        return null;
    }

    public void SetEyeSprite(Sprite sprite)
    {
        if (eyeImage != null)
        {
            eyeImage.sprite = sprite;
        }
    }
}
