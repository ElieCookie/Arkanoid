using UnityEngine;

public class CustomBounce : MonoBehaviour
{
    BoxCollider2D bc2D;

    private void Awake()
    {
        bc2D = GetComponent<BoxCollider2D>();
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
}
