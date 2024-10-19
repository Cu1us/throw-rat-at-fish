using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] AnimationCurve movementInterpolationCurve;
    [SerializeField] float movementInterpolationDuration;
    [SerializeField] float rotationInterpolationDuration;

    [SerializeField] Vector3 positionOffset;

    Vector3Int position;
    float positionInterpolationStart;
    float rotationInterpolationStart;

    Vector3 posInterpolationStartValue;
    Vector3 posInterpolationTargetValue;
    Quaternion rotInterpolationStartValue;
    Quaternion rotInterpolationTargetValue;
    bool moving = false;
    bool rotating = false;
    Direction facingDirection;

    #region Direction manipulation
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
    Vector3Int DirectionToMovement(Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector3Int.up,
            Direction.East => Vector3Int.left,
            Direction.West => Vector3Int.right,
            Direction.South => Vector3Int.down,
            _ => Vector3Int.zero
        };
    }
    Quaternion DirectionToQuaternion(Direction direction)
    {
        return direction switch
        {
            Direction.North => Quaternion.Euler(0, 0, 0),
            Direction.East => Quaternion.Euler(0, 270, 0),
            Direction.West => Quaternion.Euler(0, 90, 0),
            Direction.South => Quaternion.Euler(0, 180, 0),
            _ => Quaternion.identity
        };
    }
    /// <summary>
    /// Rotates a direction either clockwise or counterclockwise
    /// </summary>
    /// <param name="direction">The starting direction</param>
    /// <param name="clockwise">If false, rotates counterclockwise</param>
    /// <returns></returns>
    Direction RotateDirection(Direction direction, bool clockwise = true)
    {
        return direction switch
        {
            Direction.North => clockwise ? Direction.East : Direction.West,
            Direction.East => clockwise ? Direction.South : Direction.North,
            Direction.West => clockwise ? Direction.North : Direction.South,
            Direction.South => clockwise ? Direction.West : Direction.East,
            _ => Direction.North
        };
    }
    #endregion

    void Update()
    {
        int movementInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int rotationInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        if (movementInput == 1)
            Move(DirectionToMovement(facingDirection));
        else if (movementInput == -1)
            Move(-DirectionToMovement(facingDirection));

        if (rotationInput != 0)
            Rotate(clockwise: rotationInput == -1);

        InterpolatePosition();
        InterpolateRotation();
    }

    public void Move(Vector3Int translation)
    {
        if (translation == Vector3Int.zero || moving) return;

        Vector3Int nextPosition = position + translation;
        DungeonTile nextTile = tilemap.GetTile<DungeonTile>(nextPosition);

        if (nextTile == null || !nextTile.Walkable) return;

        position = nextPosition;
        posInterpolationStartValue = transform.position;
        posInterpolationTargetValue = tilemap.CellToWorld(position) + positionOffset;
        positionInterpolationStart = Time.time;
        moving = true;
    }
    public void RotateTo(Direction toDirection)
    {
        if (rotating) return;

        rotInterpolationStartValue = transform.rotation;
        rotInterpolationTargetValue = DirectionToQuaternion(toDirection);
        rotationInterpolationStart = Time.time;

        facingDirection = toDirection;
        rotating = true;
    }
    public void Rotate(bool clockwise)
    {
        RotateTo(RotateDirection(facingDirection, clockwise));
    }

    void InterpolatePosition()
    {
        if (!moving) return;
        float progress = (Time.time - positionInterpolationStart) / movementInterpolationDuration;

        transform.position = Vector3.Lerp(posInterpolationStartValue, posInterpolationTargetValue, movementInterpolationCurve.Evaluate(progress));
        if (progress >= 1f) moving = false;
    }
    void InterpolateRotation()
    {
        if (!rotating) return;
        float progress = (Time.time - rotationInterpolationStart) / rotationInterpolationDuration;

        transform.rotation = Quaternion.Lerp(rotInterpolationStartValue, rotInterpolationTargetValue, movementInterpolationCurve.Evaluate(progress));
        if (progress >= 1f) rotating = false;
    }
}
