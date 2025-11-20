using UnityEngine;
using TMPro;
using System.Collections;

public class DeathHandler : MonoBehaviour
{
    [Header("Setup")]
    public CameraFollow cameraFollow;
    public Transform player;
    public PlayerController playerController;
    public GameObject deathEffect;

    [Header("Zoom Settings")]
    public float zoomedInSize = 3f;
    public float zoomSpeed = 2f;

    [Header("Game Over UI")]
    public GameObject gameOverMenuUI;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Other UI")]
    public GameObject pauseButton;

    private Camera mainCam;
    private bool isDying = false;

    void Start()
    {
        mainCam = Camera.main;

        if (gameOverMenuUI != null)
            gameOverMenuUI.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        // Stop player input
        if (playerController != null)
            playerController.Die();

        // Freeze camera
        if (cameraFollow != null)
            cameraFollow.freezeOffsetOnDeath = true;

        // Spawn effect
        if (deathEffect != null)
            Instantiate(deathEffect, player.position, Quaternion.identity);

        // Hide pause button
        if (pauseButton != null)
            pauseButton.SetActive(false);

        StartCoroutine(ZoomAndShowGameOver());
    }

    IEnumerator ZoomAndShowGameOver()
    {
        //zoomEffect
        while (Mathf.Abs(mainCam.orthographicSize - zoomedInSize) > 0.01f)
        {
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, zoomedInSize, zoomSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);


        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (currentScoreText != null)
            currentScoreText.text = "Score: " + currentScore;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;

        

        if (gameOverMenuUI != null)
            gameOverMenuUI.SetActive(true);


        Time.timeScale = 0f; 
    }
}
