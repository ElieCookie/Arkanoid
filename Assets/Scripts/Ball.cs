using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    Rigidbody2D rb2D;
    AudioSource audioSource;

    public static event System.Action<Ball> OnBallDeath;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        rb2D.linearVelocity = rb2D.linearVelocity.normalized * speed;
    }

    public void Launch(Vector2 direction)
    {
        transform.parent = null;
        rb2D.simulated = true;
        rb2D.linearVelocity = direction.normalized * speed;
    }

    public void Catch(Transform parent)
    {
        transform.parent = parent;
        rb2D.simulated = false;
        rb2D.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        audioSource.Play();
    }
    
    public void Die()
    {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
