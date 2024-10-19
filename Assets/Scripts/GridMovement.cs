using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
    [field: SerializeReference] public Tilemap tilemap { get; protected set; }
    [SerializeField] AnimationCurve movementInterpolationCurve;
    [SerializeField] float movementInterpolationDuration;
    [SerializeField] float rotationInterpolationDuration;

    [SerializeField] Vector3 positionOffset;

    public Vector3Int position;
    public Vector3 WorldPosition { get { return tilemap.CellToWorld(position) + positionOffset; } }
    float positionInterpolationStart;
    float rotationInterpolationStart;

    Vector3 posInterpolationStartValue;
    Vector3 posInterpolationTargetValue;
    Quaternion rotInterpolationStartValue;
    Quaternion rotInterpolationTargetValue;
    protected bool moving = false;
    protected bool rotating = false;
    public Direction facingDirection { get; protected set; }

    Vector3Int failedToMoveOntoCell = new(-10000, 0, 0);
    public UnityEvent onMove;

    #region Direction manipulation
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
    void Start()
    {
        Vector3Int startingPos = tilemap.WorldToCell(transform.position);
        startingPos.z = 0;
        SetPosition(startingPos);
    }
    public static Vector3Int DirectionToMovement(Direction direction)
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
    public static Quaternion DirectionToQuaternion(Direction direction)
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
    public static Direction RotateDirection(Direction direction, bool clockwise = true)
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

    protected virtual void Update()
    {
        InterpolatePosition();
        InterpolateRotation();
    }

    public void SetPosition(Vector3Int newPos)
    {
        position = newPos;
        transform.position = tilemap.CellToWorld(position) + positionOffset;
        moving = false;
    }
    public void SetRotation(Direction newDir)
    {
        facingDirection = newDir;
        transform.rotation = DirectionToQuaternion(newDir);
        rotating = false;
    }
    public void Move(Vector3Int translation)
    {
        if (translation == Vector3Int.zero || moving) return;

        Vector3Int nextPosition = position + translation;

        if (nextPosition == failedToMoveOntoCell) return;
        if (!TryWalkOnTile(nextPosition))
        {
            failedToMoveOntoCell = nextPosition;
            return;
        };

        position = nextPosition;
        posInterpolationStartValue = transform.position;
        posInterpolationTargetValue = tilemap.CellToWorld(position) + positionOffset;
        positionInterpolationStart = Time.time;
        moving = true;
        failedToMoveOntoCell = new Vector3Int(-10000, 0, 0);
        onMove?.Invoke();
    }
    protected virtual bool TryWalkOnTile(Vector3Int position)
    {
        DungeonTile nextTile = tilemap.GetTile<DungeonTile>(position);

        return nextTile != null && nextTile.Walkable;
    }
    public void MoveForward()
    {
        Move(DirectionToMovement(facingDirection));
    }
    public void MoveInDirection(Direction direction)
    {
        Move(DirectionToMovement(direction));
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
