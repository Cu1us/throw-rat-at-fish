using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(AudioSource))]

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] RatProjectile ratProjectile;
    [SerializeField] Animator HandAnimator;
    [SerializeField] AudioClip[] RandomChatterClips;
    [SerializeField] AudioClip IntroMonologue;

    [SerializeField] float randomChatterMinCooldown;
    [SerializeField] float randomChatterMaxCooldown;
    GameManager gameManager;
    float randomChatterCooldown;
    bool ratInHand = true;

    PlayerMovement playerMovement;
    AudioSource audioSource;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindFirstObjectByType<GameManager>();
        ratInHand = true;
        HandAnimator.Play("Hidden");
        ratProjectile.onReturnToPlayer += OnRatReturn;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Shoot();
        }
        if (!audioSource.isPlaying && gameManager.RatPickedUp)
        {
            randomChatterCooldown -= Time.deltaTime;
            if (randomChatterCooldown <= 0)
            {
                randomChatterCooldown = Random.Range(randomChatterMinCooldown, randomChatterMaxCooldown);
                RandomChatter();
            }
        }
    }
    void RandomChatter()
    {
        if (!gameManager.RatPickedUp || !ratInHand) return;
        audioSource.clip = RandomChatterClips[Random.Range(0, RandomChatterClips.Length)];
        audioSource.Play();
    }
    void Shoot()
    {
        if (ratInHand && gameManager.RatPickedUp)
        {
            audioSource.Stop();
            ratProjectile.gameObject.SetActive(true);
            ratProjectile.transform.position = playerMovement.WorldPosition;
            ratProjectile.transform.rotation = GridMovement.DirectionToQuaternion(playerMovement.facingDirection);
            ratProjectile.Throw();
            HandAnimator.Play("Thrown");
            ratInHand = false;
            Invoke(nameof(HideHand), 0.25f);
        }
    }

    void OnRatReturn()
    {
        ratInHand = true;
        CancelInvoke(nameof(HideHand));
        HandAnimator.Play("Held");
        ratProjectile.transform.parent = null;
    }

    void HideHand()
    {
        if (!ratInHand) HandAnimator.Play("Hidden");
    }

    public void PlayIntroMonologue()
    {
        if (ratInHand) HandAnimator.Play("Held");
        audioSource.clip = IntroMonologue;
        audioSource.Play();
    }
}
