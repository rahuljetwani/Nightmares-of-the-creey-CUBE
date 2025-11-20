using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public Graphic[] mainMenuGraphics;    
    public Graphic[] playMenuGraphics;    
    public Graphic[] storeMenuGraphics;   
    public Graphic[] optionsMenuGraphics; 

    public Transform movingObjects;       

    [Header("Sound")]
    public AudioClip soundName;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;


    [Header("Animation")]
    public float moveDistance = 7f;
    public float moveSpeed = 3f;
    public float fadeSpeed = 2f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        originalPosition = movingObjects.position;
        targetPosition = originalPosition + Vector3.up * moveDistance;

        // Hide all submenus 
        SetGraphicsState(playMenuGraphics, 0);
        SetGraphicsState(storeMenuGraphics, 0);
        SetGraphicsState(optionsMenuGraphics, 0);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
    }

    public void OnPlayPressed()
    {
        StartCoroutine(MenuSequence(mainMenuGraphics, playMenuGraphics));
        if (soundName != null) audioSource.PlayOneShot(soundName);
    }

    public void OnStorePressed()
    {
        StartCoroutine(MenuSequence(mainMenuGraphics, storeMenuGraphics));
        if (soundName != null) audioSource.PlayOneShot(soundName);
    }

    public void OnOptionsPressed()
    {
        StartCoroutine(MenuSequence(mainMenuGraphics, optionsMenuGraphics));
        if (soundName != null) audioSource.PlayOneShot(soundName);
    }

    public void OnBackPressed()
    {
        if (IsGraphicsVisible(playMenuGraphics))
            StartCoroutine(MenuSequence(playMenuGraphics, mainMenuGraphics, reverse: true));
        else if (IsGraphicsVisible(storeMenuGraphics))
            StartCoroutine(MenuSequence(storeMenuGraphics, mainMenuGraphics, reverse: true));
        else if (IsGraphicsVisible(optionsMenuGraphics))
            StartCoroutine(MenuSequence(optionsMenuGraphics, mainMenuGraphics, reverse: true));
        if (soundName != null) audioSource.PlayOneShot(soundName);
    }

    public void OnEndlessPressed()
    {
        Debug.Log("Starting SampleScene...");
        SceneManager.LoadScene("SampleScene");
        if (soundName != null) audioSource.PlayOneShot(soundName);
    }

    //W GPT 
    IEnumerator MenuSequence(Graphic[] fromMenu, Graphic[] toMenu, bool reverse = false)
    {
        yield return StartCoroutine(FadeGraphics(fromMenu, 0)); 
        if (reverse)
            yield return StartCoroutine(MoveObjects(originalPosition));
        else
            yield return StartCoroutine(MoveObjects(targetPosition));

        yield return StartCoroutine(FadeGraphics(toMenu, 1)); 
    }

    IEnumerator FadeGraphics(Graphic[] graphics, float targetAlpha)
    {
        bool enabling = targetAlpha > 0.5f;

        if (enabling)
        {
            foreach (var g in graphics)
            {
                g.gameObject.SetActive(true);
                g.raycastTarget = false;
            }
        }
        else
        {
            //disable interaction
            foreach (var g in graphics)
                g.raycastTarget = false;
        }

        while (!IsGraphicsAtTarget(graphics, targetAlpha))
        {
            foreach (var g in graphics)
            {
                Color c = g.color;
                c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
                g.color = c;
            }
            yield return null;
        }

        if (enabling)
        {
            foreach (var g in graphics)
                g.raycastTarget = true;
        }
        else
        {
            
            foreach (var g in graphics)
                g.gameObject.SetActive(false);
        }
    }

    IEnumerator MoveObjects(Vector3 destination)
    {
        while ((movingObjects.position - destination).sqrMagnitude > 0.01f)
        {
            movingObjects.position = Vector3.MoveTowards(movingObjects.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void SetGraphicsState(Graphic[] graphics, float alpha)
    {
        bool active = alpha > 0.01f;

        foreach (var g in graphics)
        {
            Color c = g.color;
            c.a = alpha;
            g.color = c;
            g.raycastTarget = alpha > 0.5f;
            g.gameObject.SetActive(active);
        }
    }

    bool IsGraphicsAtTarget(Graphic[] graphics, float targetAlpha)
    {
        foreach (var g in graphics)
        {
            if (!Mathf.Approximately(g.color.a, targetAlpha))
                return false;
        }
        return true;
    }

    bool IsGraphicsVisible(Graphic[] graphics)
    {
        foreach (var g in graphics)
        {
            if (g.color.a > 0.5f) return true;
        }
        return false;
    }
}
