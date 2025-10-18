using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour
{
    [SerializeField] float extendShrinkDuration = 10f;
    [SerializeField] float paddleWidth = 2f;
    [SerializeField] float paddleHeight = 0.28f;

    public static Paddle Instance { get; private set; }

    public bool PaddleTransforming { get; private set; }
    private Camera mainCamera;
    private SpriteRenderer sr;
    private BoxCollider2D bc2D;
    private float defaultPaddleWidthInPixels = 200f;
    private float defaultLeftClamp = 135f;
    private float defaultRightClamp = 410f;

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
        mainCamera = FindObjectOfType<Camera>();
        sr = GetComponent<SpriteRenderer>();
        bc2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        PaddleMovement();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            float relativePosition = GetRelativePosition(other.transform);
            other.rigidbody.linearVelocity = new Vector2(relativePosition, 1).normalized * other.rigidbody.linearVelocity.magnitude;
        }

    }

    public float GetRelativePosition(Transform other)
    {
        return (other.position.x - transform.position.x) / bc2D.bounds.size.x;
    }

    private void PaddleMovement()
    {
        float paddleShift = (defaultPaddleWidthInPixels - (defaultPaddleWidthInPixels / 2 * sr.size.x)) / 2;
        float leftClamp = defaultLeftClamp - paddleShift;
        float rightClamp = defaultRightClamp + paddleShift;
        float mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, leftClamp, rightClamp);
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionPixels, 0, 0)).x;
        transform.position = new Vector3(mousePositionWorldX, transform.position.y, transform.position.z);
    }
    
    public void StartWidthAnimation(float newWidth)
    {
        StartCoroutine(AnimatePaddlewidth(newWidth));
    }

    public IEnumerator AnimatePaddlewidth(float width)
    {
        PaddleTransforming = true;
        StartCoroutine(ResetPaddleWitdhAfterTime(extendShrinkDuration));
        if(width > sr.size.x)
        {
            float currentWidth = sr.size.x;
            while (currentWidth < width)
            {
                currentWidth += Time.deltaTime * 2;
                sr.size = new Vector2(currentWidth, paddleHeight);
                bc2D.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }
        else if(width < sr.size.x)
        {
            float currentWidth = sr.size.x;
            while (currentWidth > width)
            {
                currentWidth -= Time.deltaTime * 2;
                sr.size = new Vector2(currentWidth, paddleHeight);
                bc2D.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }
        PaddleTransforming = false;
    }

    private IEnumerator ResetPaddleWitdhAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartWidthAnimation(paddleWidth);
    }
}
