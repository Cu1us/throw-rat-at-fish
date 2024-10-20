using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Animation[] animations;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        Animation fishAnimation = animations[Random.Range(0, animations.Length)];
        animator.Play(fishAnimation.ToString());
    }

    private enum Animation
    {
        Big,
        Medium,
        Small
    }
}
