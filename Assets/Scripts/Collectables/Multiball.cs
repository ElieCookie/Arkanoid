using UnityEngine;
using System.Collections.Generic;

public class Multiball : Collectable
{
    public GameObject ballPrefab;

    protected override void ApplyEffect()
    {
        // take a snapshot so we don't modify the collection while iterating (avoids infinite loop / exception)
        List<Ball> existingBalls = new List<Ball>(BallsManager.Instance.Balls);

        foreach (Ball ball in existingBalls)
        {
            if (BallsManager.Instance.Balls.Count >= BallsManager.Instance.maxBalls)
                break;
            Rigidbody2D originalRb = ball.GetComponent<Rigidbody2D>();

            for (int i = 0; i < 2; i++)
            {
                float randomAngle = Random.Range(0f, 360f);
                Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;

                GameObject newBallObj = Instantiate(ballPrefab, ball.transform.position, Quaternion.identity);
                Ball newBall = newBallObj.GetComponent<Ball>();
                newBall.Launch(direction);

                // add the newly created ball to the manager's list
                BallsManager.Instance.Balls.Add(newBall);
            }
        }
    }
}
