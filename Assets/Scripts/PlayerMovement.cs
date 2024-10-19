using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : GridMovement
{
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

        base.Update();
    }
}
