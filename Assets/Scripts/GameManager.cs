using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gameplay")]
    public int startingLives = 3;
    public int score = 0;
    public int lives = 3;
    public int pointsPerExtraLife = 1000;

    [Header("UI (se reasignan en OnSceneLoaded)")]
    public TMP_Text ScoreText;
    public TMP_Text LivesText;
    public GameObject gameOverPanel;

    [Header("Scenes")]
    public int mainMenuSceneIndex = 0;
    public int gameSceneIndex = 1;

    private int nextLifeThreshold;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // Solo ocultamos panel aquí, las referencias vendrán ya asignadas  
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateUI();
    }

    // Se dispara al cargar cualquier escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == gameSceneIndex)
        {
            // 1) Reasignar referencias UI de la nueva escena
            ScoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
            LivesText = GameObject.Find("LivesText")?.GetComponent<TMP_Text>();
            gameOverPanel = GameObject.Find("GameOverPanel");

            // 2) Resetear variables de partida
            ResetGame();
        }
    }

    void ResetGame()
    {
        score = 0;
        lives = startingLives;
        nextLifeThreshold = pointsPerExtraLife;
        Time.timeScale = 1;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
        while (score >= nextLifeThreshold)
        {
            lives++;
            nextLifeThreshold += pointsPerExtraLife;
        }
        UpdateUI();
    }

    public void LoseLife()
    {
        if (lives <= 0) return;
        lives--;
        UpdateUI();
        if (lives <= 0)
            StartCoroutine(HandleGameOver());
    }

    void UpdateUI()
    {
        if (ScoreText != null) ScoreText.text = "Puntos: " + score;
        if (LivesText != null) LivesText.text = "Vidas: " + Mathf.Max(0, lives);
    }

    IEnumerator HandleGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

