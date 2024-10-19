using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RatProjectile : MonoBehaviour
{
    public PlayerMovement player;
    Animator animator;
    public Action onReturnToPlayer;
    public float speed;
    public float runningSpeed;

    enum RatState
    {
        IN_HAND,
        FLYING,
        SPLAT,
        RUNNING
    }
    RatState ratState = RatState.IN_HAND;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (ratState == RatState.FLYING)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Time.deltaTime * speed))
            {
                if (hitInfo.collider.TryGetComponent(out EnemyMovement enemy))
                {

                }
                transform.position = hitInfo.point;
                transform.parent = hitInfo.collider.transform;
                ratState = RatState.SPLAT;
                Debug.Log($"Hit: {hitInfo.point} point, {hitInfo.collider.gameObject.name}");
                animator.Play("Splat");
                Invoke(nameof(StartRunning), 0.5f);
            }
            else
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
        else if (ratState == RatState.RUNNING)
        {
            Vector3 targetPosition = new(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, runningSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                ratState = RatState.IN_HAND;
                onReturnToPlayer?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }

    void StartRunning()
    {
        ratState = RatState.RUNNING;
        transform.position -= new Vector3(0, 0.25f, 0);
        animator.Play("Running");
    }
    public void Throw()
    {
        ratState = RatState.FLYING;
        if (!animator) animator = GetComponent<Animator>();
        animator.Play("Flying");
    }
}
