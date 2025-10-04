using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] Text livesText;
    [SerializeField] Text scoreText;
    [SerializeField] Text highScoreText;
    [SerializeField] int initialLives = 5;
    [SerializeField] GameObject blackScreen;
    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject gameWinText;
    [SerializeField] GameObject restartButton;
    int currentScore = 0;
    int currentLives = 0;
    int currentHighScore = 0;
    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        currentLives = initialLives;
        restartButton.GetComponent<Button>().onClick.AddListener(ResetGame);
        currentHighScore = PlayerPrefs.GetInt("High Score", 0);
    }

    void Update()
    {
        CountBalls();
        CountBlocks();
        UpdateUI();
    }

    public void AddToScore(int points)
    {
        currentScore += points;
    }

    public void DecreaseLives()
    {
        currentLives--;
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void Win()
    {
        var scenesNum = SceneManager.sceneCountInBuildSettings;
        var currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentBuildIndex == scenesNum - 1)
        {
            ShowWinScreen();
        }
        else
        {
            SceneManager.LoadScene(currentBuildIndex + 1);
        }
    }

    public void GameOver()
    {
        blackScreen.SetActive(true);
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
        SetHighScore();
        PauseGame();
    }

    public void ResetGame()
    {
        ResetScore();
        ResetScreen();
        ResetLevel1();
    }

    private void ResetScreen()
    {
        blackScreen.SetActive(false);
        gameOverText.SetActive(false);
        gameWinText.SetActive(false);
        restartButton.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ResetScore()
    {
        currentLives = initialLives;
        currentScore = 0;
    }

    private void ResetLevel1()
    {
        SceneManager.LoadScene(0);
        ResetScore();
    }

    private void SetHighScore()
    {
        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            PlayerPrefs.SetInt("High Score", currentHighScore);
            PlayerPrefs.Save();
        }
    }
    private void UpdateUI()
    {
        livesText.text = currentLives.ToString();
        scoreText.text = currentScore.ToString();
        highScoreText.text = currentHighScore.ToString();
    }

    private void CountBlocks()
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        if (blocks.Length == 0)
        {
            Win();
        }
    }

    private void CountBalls()
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length == 0)
        {
            DecreaseLives();
            GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().ResetBall();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    private void ShowWinScreen()
    {
        blackScreen.SetActive(true);
        gameWinText.SetActive(true);
        restartButton.SetActive(true);
        SetHighScore();
        PauseGame();
    }
}
