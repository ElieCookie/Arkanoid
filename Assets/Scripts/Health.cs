using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 1;
    [SerializeField] int pointsPerBlock = 10;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            health--;
            if (health <= 0)
            {
                FindObjectOfType<GameSession>().AddToScore(pointsPerBlock);
                Destroy(gameObject);
            }
        }
    }
}
