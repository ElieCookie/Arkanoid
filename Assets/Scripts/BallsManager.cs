using UnityEngine;
using System.Collections.Generic;

public class BallsManager : MonoBehaviour
{
    [SerializeField] Ball ballPrefab;
    [SerializeField] public float initialBallSpeed = 100f;
    public static BallsManager Instance { get; private set; }
    public List<Ball> Balls { get; set; }

    private Ball initialBall;
    private Rigidbody2D initBallRb2D;

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
        InitBall();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameStarted)
        {
            // Align ball with paddle
            Vector3 paddlePos = Paddle.Instance.gameObject.transform.position;
            Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 0.27f, 0);
            initialBall.transform.position = ballPos;

            if (Input.GetMouseButtonDown(0))
            {
                initBallRb2D.isKinematic = false;
                initBallRb2D.AddForce(new Vector2(0, initialBallSpeed));
                GameManager.Instance.IsGameStarted = true;
            }
        }
    }
    private void InitBall()
    {
        Vector3 paddlePos = Paddle.Instance.gameObject.transform.position;
        Vector3 ballStartPos = new Vector3(paddlePos.x, paddlePos.y + 0.27f, 0);
        initialBall = Instantiate(ballPrefab, ballStartPos, Quaternion.identity);
        initBallRb2D = initialBall.GetComponent<Rigidbody2D>();

        Balls = new List<Ball> { initialBall };
    }
    
    public void ResetBalls()
    {
        foreach (var ball in Balls)
        {
            Destroy(ball.gameObject);
            Debug.Log("Destroyed ball");
        }
        InitBall();
    }
}
