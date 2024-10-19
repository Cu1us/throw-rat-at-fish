using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableTile : MonoBehaviour
{
    public abstract bool OnPlayerMoveOnto(PlayerMovement player);
}
