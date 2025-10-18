using UnityEngine;

public class ExtendOrShrink : Collectable
{
    [SerializeField] float NewWidth = 1.5f;
    [SerializeField] float effectDuration = 5f;

    protected override void ApplyEffect()
    {
        Paddle paddle = FindObjectOfType<Paddle>();
        if (paddle != null && !paddle.PaddleTransforming)
        {
            paddle.StartWidthAnimation(NewWidth);
        }
    }
}
