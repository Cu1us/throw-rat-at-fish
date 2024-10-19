using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{

    [SerializeField] RatProjectile ratProjectile;
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
            ratInHand = false;
        }
    }

    void OnRatReturn()
    {
        ratInHand = true;
    }
}
