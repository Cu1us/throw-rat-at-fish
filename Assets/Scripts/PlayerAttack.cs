using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{

    [SerializeField] RatProjectile ratProjectile;
    [SerializeField] Animator HandAnimator;
    bool ratInHand = true;

    PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ratInHand = true;
        ratProjectile.onReturnToPlayer += OnRatReturn;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (ratInHand)
        {
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
