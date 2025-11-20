using UnityEngine;

public class EyeSpriteLoader : MonoBehaviour
{
    [Header("Eye Sprites")]
    public Sprite[] eyeSprites;

    void Start()
    {
        int savedEyeIndex = PlayerPrefs.GetInt("SelectedEyeIndex", 0);

        if (eyeSprites.Length > savedEyeIndex)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = eyeSprites[savedEyeIndex];
                Debug.Log($"Loaded eye index: {savedEyeIndex}");
            }
        }
    }
}