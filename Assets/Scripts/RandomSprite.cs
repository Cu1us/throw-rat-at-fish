using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Animation[] animations;
    [SerializeField] private Animation setAnimation;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        
        if (setAnimation != Animation.None)
        {
            animator.Play(setAnimation.ToString());
            return;
        }
            
        Animation fishAnimation = animations[Random.Range(0, animations.Length)];
        animator.Play(fishAnimation.ToString());
    }

    private enum Animation
    {
        None,
        Big,
        Medium,
        Small
    }
}
