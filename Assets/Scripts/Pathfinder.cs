using UnityEngine;
using System;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridMovement player;
    
    // TODO Get player and enemy positions in Vector3Int
    // TODO Compare the distance between the two
    
    // TODO Calculate the new distance in each non-diagonal cell and how far away it is from the original enemy cell
    // TODO Repeat this process until it finds the best calculated path

    private Vector3Int gridPosition = new();
    private Vector3Int playerGridPosition = new();

    private Cell[,] openCells;
    private Cell[,] closedCells;

    private void InitializeOpenCell()
    {
        // TODO initialize size of cell arrays.
    }

    private void FindLowestOpenCellScore()
    {
        
    }
    
    private int CalculateInitialDistance()
    {
        return (int)((gridPosition - playerGridPosition).magnitude * 10);
    }
}

public struct Cell
{
    private Vector3Int position;
    
    private int fromDistance;
    private int toDistance;

    public int distanceScore;

    public Cell(Vector3Int position, int fromDistance, int toDistance)
    {
        this.position = position;
        this.fromDistance = fromDistance * 10;
        this.toDistance = toDistance * 10;

        distanceScore = this.fromDistance + this.toDistance;
    }
}
