using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GridMovement ratProjectile;
    enum RatState
    {
        IN_HAND,
        FLYING,
        SPLAT,
        RUNNING
    }
    RatState ratState = RatState.IN_HAND;

    PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Shoot();
        }
        if (ratState == RatState.FLYING)
        {
            ratProjectile.MoveForward();
        }
    }

    void Shoot()
    {
        if (ratState != RatState.IN_HAND) return;

        ratProjectile.SetPosition(playerMovement.position);
        ratProjectile.SetRotation(playerMovement.facingDirection);
        ratProjectile.gameObject.SetActive(true);

        ratState = RatState.FLYING;
    }
}
