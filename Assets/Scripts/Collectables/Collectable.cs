using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.down * fallSpeed;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paddle"))
        {
            ApplyEffect();
        }
        if (collision.CompareTag("Paddle") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
    protected abstract void ApplyEffect();
}
