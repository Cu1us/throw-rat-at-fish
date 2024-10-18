using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] AnimationCurve movementInterpolationCurve;
    [SerializeField] float movementInterpolationDuration;

    Vector3Int position;
    float interpolationStartTime;
    Vector3 interpolationStartPosition;
    bool interpolating = false;

    void Update()
    {
        Vector3Int movement = new
        (
            Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")),
            Mathf.RoundToInt(Input.GetAxisRaw("Vertical")),
            0
        );
        Move(movement);
        InterpolatePosition();
    }

    public void Move(Vector3Int translation)
    {
        if (translation == Vector3Int.zero || interpolating) return;
        position += translation;
        interpolationStartPosition = transform.position;
        interpolationStartTime = Time.time;
        interpolating = true;
    }

    void InterpolatePosition()
    {
        if (!interpolating) return;
        float progress = (Time.time - interpolationStartTime) / movementInterpolationDuration;
        transform.position = Vector3.Lerp(interpolationStartPosition, grid.CellToWorld(position), movementInterpolationCurve.Evaluate(progress));
        if (progress >= 1f) interpolating = false;
    }
}
