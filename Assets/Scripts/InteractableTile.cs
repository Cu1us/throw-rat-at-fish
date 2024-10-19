using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableTile : MonoBehaviour
{
    public abstract GridMovement.TryWalkOnTileResult OnPlayerMoveOnto(PlayerMovement player);
}
