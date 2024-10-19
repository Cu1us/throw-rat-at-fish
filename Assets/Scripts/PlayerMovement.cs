using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : GridMovement
{
    public byte Keys = 0;
    [SerializeField] Image KeycardHandUI;
    protected override void Update()
    {
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
    }
    protected override bool TryWalkOnTile(Vector3Int position)
    {
        GameObject tileObject = tilemap.GetInstantiatedObject(position);
        if (tileObject.TryGetComponent(out InteractableTile interactableTile))
        {
            return interactableTile.OnPlayerMoveOnto(this);
        }
        return base.TryWalkOnTile(position);
    }
}
