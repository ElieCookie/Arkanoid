using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameStarted { get; set; }
    [SerializeField] public int AvailableLives = 3;
    [SerializeField] public GameObject gameOverScreen;
    [SerializeField] public GameObject victoryScreen;
    [SerializeField] public GameObject tryAgainButton;
    [SerializeField] public GameObject startOverButton;
    [SerializeField] public GameObject pauseGameScreen;
    [SerializeField] public GameObject resumeButton;
    [SerializeField] public GameObject pauseButton;
    public int Lives { get; set; }
    public static event System.Action<int> OnLifeLost;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        Lives = AvailableLives;
        Screen.SetResolution(540, 960, false);
        Ball.OnBallDeath += HandleBallDeath;
        Block.OnBlockDestruction += HandleBlockDestroyed;
        startOverButton.GetComponent<Button>().onClick.AddListener(RestartGame);
        tryAgainButton.GetComponent<Button>().onClick.AddListener(RestartGame);
        pauseButton.GetComponent<Button>().onClick.AddListener(PauseGameScreen);
    }

    private void HandleBlockDestroyed(Block block)
    {
        if (BlocksManager.Instance.RemainingBlocks.Count <= 0)
        {
            Debug.Log("All blocks destroyed, loading next level");
            BallsManager.Instance.ResetBalls();
            IsGameStarted = false;
            BlocksManager.Instance.LoadNextLevel();
        }
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0f;
        startOverButton.GetComponent<Button>().onClick.AddListener(RestartGame);
    }

    public void PauseGameScreen()
    {
        Time.timeScale = 0f;
        pauseGameScreen.SetActive(true);
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
    }

    public void RestartGame()
    {
        Camera camera = FindObjectOfType<Camera>();
        DontDestroyOnLoad(camera.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        pauseGameScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    private void HandleBallDeath(Ball ball)
    {
        Debug.Log("Ball died, handling in GameManager");
        if (BallsManager.Instance.Balls.Count <= 0)
        {
            Debug.Log("No balls left, decreasing life");
            Lives--;

            if (Lives <= 0)
            {
                Time.timeScale = 0f;
                gameOverScreen.SetActive(true);
                tryAgainButton.GetComponent<Button>().onClick.AddListener(RestartGame);
            }
            else
            {
                OnLifeLost?.Invoke(Lives);
                BallsManager.Instance.ResetBalls();
                IsGameStarted = false;
                //BlocksManager.Instance.LoadLevel(BlocksManager.Instance.CurrentLevel);
            }
        }
    }

    private void OnDisable()
    {
        Ball.OnBallDeath -= HandleBallDeath;
        Block.OnBlockDestruction -= HandleBlockDestroyed;
    }
}
