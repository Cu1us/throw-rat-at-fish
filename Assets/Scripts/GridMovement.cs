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
    enum Direction
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
            Direction.North => Vector3Int.forward,
            Direction.East => Vector3Int.right,
            Direction.West => Vector3Int.left,
            Direction.South => Vector3Int.back,
            _ => Vector3Int.zero
        };
    }
    Quaternion DirectionToQuaternion(Direction direction)
    {
        return direction switch
        {
            Direction.North => Quaternion.Euler(0, 0, 0),
            Direction.East => Quaternion.Euler(0, 0, 270),
            Direction.West => Quaternion.Euler(0, 0, 90),
            Direction.South => Quaternion.Euler(0, 0, 180),
            _ => Quaternion.identity
        };
    }
    /*Direction RotateDirection(Direction direction, bool reverse)
    {
        return direction switch
        {
            Direction.North => Quaternion.Euler(0, 0, 0),
            Direction.East => Quaternion.Euler(0, 0, 270),
            Direction.West => Quaternion.Euler(0, 0, 90),
            Direction.South => Quaternion.Euler(0, 0, 180),
            _ => Quaternion.identity
        };
    }*/


    void Update()
    {
        int movementInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        if (movementInput == 1)
            Move(DirectionToMovement(facingDirection));
        else if (movementInput == -1)
            Move(-DirectionToMovement(facingDirection));

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
    /*public void Rotate(Direction toDirection)
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
    }*/

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
