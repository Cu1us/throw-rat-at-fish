using UnityEngine;
using System;

public class EnemyMovement : GridMovement
{
    [SerializeField] private PlayerMovement player;

    private Vector3Int arrayOffset;

    private Cell startCell;
    private Cell nextCell;

    private void Start()
    {
        CreateArrayOffset();
    }
    
    private void CreateArrayOffset()
    {
        arrayOffset = tilemap.cellBounds.size / 2;
    }

    private void InitializeFirstCell()
    {
        startCell = new Cell(position, 0, CalculateDistance(position, player.position));
    }

    private void OnEnable() => player.onMove.AddListener(FindLowestScore);

    private void OnDisable() => player.onMove.RemoveListener(FindLowestScore);
    
    private void FindLowestScore()
    {
        Cell[,] openCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
        Cell[,] closedCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
            
        int lowestDistanceScore = CalculateDistance(position, player.position);
        
        InitializeFirstCell();
        
        while (startCell.position != player.position)
        {
            (int lowestNeighbourScore, Cell neighbourCell) = AddNeighboursToOpenList(openCells, startCell, lowestDistanceScore);
            
            if (neighbourCell is not null && lowestNeighbourScore < lowestDistanceScore)
            {
                lowestDistanceScore = lowestNeighbourScore;
            
                startCell = neighbourCell;
                closedCells[arrayOffset.x + startCell.position.x, arrayOffset.y + startCell.position.y] = startCell;
            }
            else
            {
                foreach (Cell openCell in openCells)
                {
                    if (openCell is null)
                        continue;
                
                    if (openCell.distanceScore < lowestDistanceScore)
                        lowestDistanceScore = openCell.distanceScore;
                }
            }
        }
        
        Move(position - startCell.position);
    }
    
    private (int, Cell) AddNeighboursToOpenList(Cell[,] cells, Cell cell, int distanceScore)
    {
        Cell[] neighbours = new Cell[4];
        
        neighbours[0] = CreateOpenCell(cells, cell, Vector3Int.up);
        neighbours[1] = CreateOpenCell(cells, cell, Vector3Int.right);
        neighbours[2] = CreateOpenCell(cells, cell, Vector3Int.down);
        neighbours[3] = CreateOpenCell(cells, cell, Vector3Int.left);
        
        return GetLowestScore(neighbours, distanceScore);
    }
    
    private (int, Cell) GetLowestScore(Cell[] cells, int distanceScore)
    {
        int lowestNeighbourScore = CalculateDistance(new Vector3Int(), new Vector3Int(tilemap.cellBounds.max.x, tilemap.cellBounds.max.y, 0));
        Cell checkedCell = null;
        
        foreach (Cell cell in cells)
        {
            if (cell is null || cell.distanceScore >= distanceScore)
                continue;
            
            lowestNeighbourScore = cell.distanceScore;
            checkedCell = cell;
        }
        
        return (lowestNeighbourScore, checkedCell);
    }

    private Cell CreateOpenCell(Cell[,] cells, Cell cell, Vector3Int direction)
    {
        Vector3Int tempPosition = new(
            cell.position.x + direction.x,
            0,
            cell.position.y + direction.y);

        DungeonTile tile = tilemap.GetTile<DungeonTile>(tempPosition);

        if (tile is null)
            return null;
        
        if (!tile.Walkable) 
            return null;
        
        int startToCell = CalculateDistance(tempPosition, position);
        int targetToCell = CalculateDistance(tempPosition, player.position);

        Cell newCell = new(tempPosition, startToCell, targetToCell);
        
        cells[arrayOffset.x + cell.position.x + direction.x, arrayOffset.y + cell.position.z + direction.y] = newCell;

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
