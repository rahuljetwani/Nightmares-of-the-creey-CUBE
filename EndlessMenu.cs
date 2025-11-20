using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class EndlessMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseMenuUI;
    public GameObject pauseButtonUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        Resume();
        gameOverUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        Debug.Log("Pause button pressed");
        pauseMenuUI.SetActive(true);
        pauseButtonUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        Debug.Log("Resume button pressed");
        pauseMenuUI.SetActive(false);
        pauseButtonUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ShowGameOver()
    {
        Debug.Log("Game Over triggered");
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        pauseButtonUI.SetActive(false);

        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        currentScoreText.text = "Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        if (EyeSelectionManager.OnEyeSelectionChanged != null)
            EyeSelectionManager.OnEyeSelectionChanged = null;

        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu pressed");
        Time.timeScale = 1f;

        if (EyeSelectionManager.OnEyeSelectionChanged != null)
            EyeSelectionManager.OnEyeSelectionChanged = null;

        SceneManager.LoadScene("MAIN MENU");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button pressed");
        Time.timeScale = 1f;



        if (EyeSelectionManager.OnEyeSelectionChanged != null)
            EyeSelectionManager.OnEyeSelectionChanged = null;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
