using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    Rigidbody2D rb2D;
    AudioSource audioSource;
    bool _hasContactedWall = false;

    public static event System.Action<Ball> OnBallDeath;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (rb2D != null && rb2D.linearVelocity.sqrMagnitude > 0f)
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
        if (other.gameObject.CompareTag("SideWall") && !_hasContactedWall)
        {
            Vector2 contactNormal = other.contacts[0].normal;
            Vector2 incoming = rb2D.linearVelocity;
            float currentSpeed = Mathf.Max(0.01f, incoming.magnitude);

            // reflected direction from wall
            Vector2 reflectDir = Vector2.Reflect(incoming.normalized, contactNormal);
            Vector2 newDir = reflectDir;

            const float minVertical = 0.20f; // minimum absolute vertical component to avoid horizontal trapping

            // If incoming is nearly horizontal, rotate reflected direction slightly (modest deviation)
            if (Mathf.Abs(incoming.y) < 0.05f)
            {
                float angleDeg = Random.Range(12f, 25f) * (Random.value < 0.5f ? 1f : -1f);
                float rad = angleDeg * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                newDir = new Vector2(reflectDir.x * cos - reflectDir.y * sin,
                                     reflectDir.x * sin + reflectDir.y * cos).normalized;
            }

            // Ensure minimum vertical component (preserve sign if possible)
            if (Mathf.Abs(newDir.y) < minVertical)
            {
                float sign = 0f;
                if (Mathf.Abs(incoming.y) > 0.01f) sign = Mathf.Sign(incoming.y);    // preserve incoming sign when available
                else sign = (Random.value < 0.5f) ? 1f : -1f;                        // otherwise random up/down

                newDir.y = sign * minVertical;
                // keep x direction similar to reflectDir.x sign to avoid reversing horizontally
                newDir.x = Mathf.Sign(reflectDir.x) * Mathf.Sqrt(Mathf.Clamp01(1f - newDir.y * newDir.y));
                newDir = newDir.normalized;
            }

            // small jitter so we don't get axis-aligned bounces
            float jitterDeg = Random.Range(-2f, 2f);
            float jr = jitterDeg * Mathf.Deg2Rad;
            newDir = new Vector2(newDir.x * Mathf.Cos(jr) - newDir.y * Mathf.Sin(jr),
                                 newDir.x * Mathf.Sin(jr) + newDir.y * Mathf.Cos(jr)).normalized;

            rb2D.linearVelocity = newDir * currentSpeed;

            // nudge out of wall to avoid repeated contact
            transform.position += (Vector3)(contactNormal * 0.02f);

            _hasContactedWall = true;
        }
        Invoke(nameof(OnBallContactDelay), 0.5f);
    }

    private void OnBallContactDelay()
    {
        _hasContactedWall = false;
    }
    
    public void Die()
    {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
