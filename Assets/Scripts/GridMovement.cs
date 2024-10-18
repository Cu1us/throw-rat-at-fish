using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] AnimationCurve movementInterpolationCurve;
    [SerializeField] float movementInterpolationDuration;

    Vector3Int position;
    float interpolationStartTime;
    Vector3 interpolationStartPosition;
    Vector3 interpolationTargetPosition;
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
        interpolationTargetPosition = tilemap.CellToWorld(position);
        interpolationStartTime = Time.time;
        interpolating = true;

        GameObject tileObject = tilemap.GetInstantiatedObject(position);
        if (tileObject == null)
            Debug.Log("Moving onto empty tile");
        else
            Debug.Log("Moving onto: " + tileObject.tag);
    }

    void InterpolatePosition()
    {
        if (!interpolating) return;
        float progress = (Time.time - interpolationStartTime) / movementInterpolationDuration;

        transform.position = Vector3.Lerp(interpolationStartPosition, interpolationTargetPosition, movementInterpolationCurve.Evaluate(progress));
        if (progress >= 1f) interpolating = false;
    }
}
