using UnityEngine;
using System;

public class EnemyMovement : GridMovement
{
    [SerializeField] private PlayerMovement player;
    
    private Vector3Int playerGridPosition;

    private Vector3Int arrayOffset;

    private Cell[,] openCells;
    private Cell[,] closedCells;

    private int lowestDistanceScore;

    private Cell explorationCell;
    private Cell nextCell;

    private void Start()
    {
        CreateArrayOffset();
        lowestDistanceScore = CalculateDistance(position, playerGridPosition);
        
        InitializeFirstCell();
    }
    
    private void CreateArrayOffset()
    {
        arrayOffset = tilemap.WorldToCell(new Vector3(0, 0, 0)) - tilemap.cellBounds.min;
    }

    private void InitializeFirstCell()
    {
        explorationCell = new Cell(position, 0, CalculateDistance(position, playerGridPosition));
    }

    private void OnEnable() => player.onMove.AddListener(FindLowestScore);

    private void OnDisable() => player.onMove.RemoveListener(FindLowestScore);
    
    private void FindLowestScore()
    {
        (int lowestNeighbourScore, Cell checkedCell) = AddNeighboursToOpenList(explorationCell);

        if (lowestNeighbourScore < lowestDistanceScore)
        {
            lowestDistanceScore = lowestNeighbourScore;
            explorationCell = checkedCell;
        }
        else
        {
            foreach (Cell openCell in openCells)
            {
                if (openCell.distanceScore < lowestDistanceScore)
                    lowestDistanceScore = openCell.distanceScore;
            }
        }
        
        Move(position - explorationCell.position);
    }
    
    private (int, Cell) AddNeighboursToOpenList(Cell cell)
    {
        Cell[] neighbours = new Cell[4];
        
        neighbours[0] = CreateOpenCell(cell, Vector3Int.up);
        neighbours[1] = CreateOpenCell(cell, Vector3Int.right);
        neighbours[2] = CreateOpenCell(cell, Vector3Int.down);
        neighbours[3] = CreateOpenCell(cell, Vector3Int.left);
        
        return GetLowestScore(neighbours);
    }
    
    private (int, Cell) GetLowestScore(Cell[] cells)
    {
        int lowestNeighbourScore = CalculateDistance(position, playerGridPosition);
        Cell checkedCell = null;
        
        foreach (Cell cell in cells)
        {
            if (cell is null || cell.distanceScore >= lowestDistanceScore)
                continue;
            
            lowestNeighbourScore = cell.distanceScore;
            checkedCell = cell;
        }
        
        return (lowestNeighbourScore, checkedCell);
    }

    private Cell CreateOpenCell(Cell cell, Vector3Int direction)
    {
        Vector3Int tempPosition = new(
            arrayOffset.x + cell.position.x + direction.x, 
            arrayOffset.y + cell.position.y + direction.y, 
            arrayOffset.z + cell.position.z + direction.z);

        DungeonTile tile = tilemap.GetTile<DungeonTile>(tempPosition);

        if (!tile.Walkable) 
            return null;
        
        int startToCell = CalculateDistance(tempPosition, position);
        int targetToCell = CalculateDistance(tempPosition, playerGridPosition);

        Cell newCell = new(tempPosition, startToCell, targetToCell);
        
        openCells[cell.position.x + direction.x, cell.position.y + direction.y] = newCell;

        return newCell;
    }
    
    private int CalculateDistance(Vector3Int start, Vector3Int target)
    {
        return (Math.Abs(start.x - target.x) + Math.Abs(start.y - target.y)) * 10;
    }
}

public class Cell
{
    public Vector3Int position;
    
    private int distanceFromStart;
    private int distanceFromTarget;

    public int distanceScore;

    public Cell(Vector3Int position, int distanceFromStart, int distanceFromTarget)
    {
        this.position = position;
        this.distanceFromStart = distanceFromStart;
        this.distanceFromTarget = distanceFromTarget;

        distanceScore = this.distanceFromStart + this.distanceFromTarget;
    }
}
