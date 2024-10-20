using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(AudioSource))]

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] RatProjectile ratProjectile;
    [SerializeField] Animator HandAnimator;
    [SerializeField] AudioClip[] ThrowClips;
    [SerializeField] AudioClip[] RandomChatterClips;
    [SerializeField] float randomChatterMinCooldown;
    [SerializeField] float randomChatterMaxCooldown;
    float randomChatterCooldown;
    bool ratInHand = true;

    PlayerMovement playerMovement;
    AudioSource audioSource;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        ratInHand = true;
        ratProjectile.onReturnToPlayer += OnRatReturn;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Shoot();
        }
        if (!audioSource.isPlaying)
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
        audioSource.clip = RandomChatterClips[Random.Range(0, RandomChatterClips.Length)];
        audioSource.Play();
    }
    void Shoot()
    {
        if (ratInHand)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(ThrowClips[Random.Range(0, ThrowClips.Length)]);
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
}
