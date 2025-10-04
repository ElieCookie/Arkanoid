using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 30f;
    [SerializeField] GameObject ballPrefab;
    Rigidbody2D rb2D;
    CustomBounce customBounce;
    Vector3 ballOffset;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        customBounce = GetComponent<CustomBounce>();
    }

    private void Start()
    {
        Ball ball = GetComponentInChildren<Ball>();
        ballOffset = ball.transform.position - transform.position;
    }
    void Update()
    {
        rb2D.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);

        if (transform.childCount > 0 && Input.GetButtonDown("Jump"))
        {
            Ball ball = GetComponentInChildren<Ball>();
            float relativePosition = customBounce.GetRelativePosition(ball.transform);
            ball.Launch(new Vector2(relativePosition, 1));
        }

        if(Input.GetButtonDown("Fire1"))
        {
            ResetBall();
        }

    }

    public void ResetBall()
    {
        Ball ball = Instantiate(ballPrefab).GetComponent<Ball>();
        ball.transform.parent = transform;
        ball.transform.position = transform.position + ballOffset;
    }
}
