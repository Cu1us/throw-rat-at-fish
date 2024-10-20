using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement : GridMovement
{
    public byte Keys = 0;
    public float MaxHealth;
    public float Health;
    [SerializeField] float HealthRegenRate;
    [SerializeField] Volume RedVignette;
    [SerializeField] Image KeycardHandUI;
    [SerializeField] Vector3Int gameEndPosition;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    protected override void Update()
    {
        if (position == gameEndPosition)
        {
            gameManager.GameEnd();
            enabled = false;
            return;
        }
        int movementInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int rotationInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        if (movementInput == 1)
            Move(DirectionToMovement(facingDirection));
        else if (movementInput == -1)
            Move(-DirectionToMovement(facingDirection));

        if (rotationInput != 0)
            Rotate(clockwise: rotationInput == -1);
        KeycardHandUI.enabled = Keys > 0;

        base.Update();
        Health = Mathf.Clamp(Health + HealthRegenRate * Time.deltaTime, 0, MaxHealth);
        UpdateHealth();
        if (movementInput == 0)
        {
            moveCooldown = false;
        }
    }
    protected override TryWalkOnTileResult TryWalkOnTile(Vector3Int position)
    {
        if (moveCooldown) return TryWalkOnTileResult.StopButCanPassNextTime;
        GameObject tileObject = tilemap.GetInstantiatedObject(position);
        if (tileObject.TryGetComponent(out InteractableTile interactableTile))
        {
            return interactableTile.OnPlayerMoveOnto(this);
        }
        List<EnemyMovement> enemies = FindFirstObjectByType<GameManager>().enemies;
        foreach (EnemyMovement enemy in enemies)
        {
            if (enemy.position == position)
                return TryWalkOnTileResult.StopButCanPassNextTime;
        }
        return base.TryWalkOnTile(position);
    }

    void UpdateHealth()
    {
        RedVignette.weight = 1 - (Health / MaxHealth);
    }

    public void AttackedByEnemy(EnemyMovement enemy)
    {
        Health--;
        if (Health <= 0)
        {
            Debug.Log("Player died!");
            gameManager.GameOver();
        }
        else
        {
            UpdateHealth();
        }
    }
}
