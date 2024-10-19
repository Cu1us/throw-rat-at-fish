using UnityEngine;
using System;
using Vector3Int = UnityEngine.Vector3Int;
using System.Collections.Generic;
using System.Linq;

public class EnemyMovement : GridMovement
{
    [SerializeField] private PlayerMovement player;

    private Vector3Int arrayOffset;

    private void Start()
    {
        CreateArrayOffset();
    }
    
    private void CreateArrayOffset()
    {
        arrayOffset = tilemap.cellBounds.size / 2;
    }

    private void OnEnable() => player.onMove.AddListener(FindLowestScore);

    private void OnDisable() => player.onMove.RemoveListener(FindLowestScore);
    
    private void FindLowestScore()
    {
        Cell[,] openCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
        Cell[,] closedCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
        
        Cell startCell = InitializeFirstCell();
        Cell cellToExplore = startCell;

        int ranTimes = 0;
        
        while (cellToExplore.position != player.position && ranTimes < 1000)
        {
            ranTimes++;
            // Initially sets the lowest distance score to the highest possible so it will search all options.
            int lowestDistanceScore = (openCells.GetLength(0) + openCells.GetLength(1)) * 10;
            Cell neighbourCell = AddNeighboursToOpenList(openCells, cellToExplore, lowestDistanceScore);

            if (neighbourCell is null)
            {
                foreach (Cell openCell in openCells)
                {
                    if (openCell is null)
                        continue;

                    if (openCell.distanceScore >= lowestDistanceScore) 
                        continue;
                    
                    lowestDistanceScore = openCell.distanceScore;
                    cellToExplore = openCell;
                }
            }
            else
            {
                cellToExplore = neighbourCell;
            }

            int x = arrayOffset.x + cellToExplore.position.x;
            int y = arrayOffset.y + cellToExplore.position.y;
            
            closedCells[x, y] = cellToExplore;
        }

        Vector3Int direction = new();
        List<Cell> options = new();
        
        foreach (Cell closedCell in closedCells)
        {
            if (closedCell is null)
                continue;
            
            Vector3Int middlePoint = startCell.position - closedCell.position;
            
            if (middlePoint is not { x: <= 1 and >= -1, y: <= 1 and >= -1 }) 
                continue;
            
            options.Add(closedCell);
        }
        
        Cell nextCell = options.Aggregate((cellWithLowestTarget, cell) => cell.distanceFromTarget <= cellWithLowestTarget.distanceFromTarget ? cell : cellWithLowestTarget);
        
        direction = nextCell.position - startCell.position;
        Debug.Log(direction);
        
        Move(direction);
    }
    
    private Cell InitializeFirstCell()
    {
        return new Cell(new Vector3Int(position.x, position.y, 0), 0, CalculateDistance(position, player.position));
    }
    
    private Cell AddNeighboursToOpenList(Cell[,] cells, Cell cell, int distanceScore)
    {
        Cell[] neighbours = new Cell[4];
        
        neighbours[0] = CreateOpenCell(cells, cell, Vector3Int.up);
        neighbours[1] = CreateOpenCell(cells, cell, Vector3Int.right);
        neighbours[2] = CreateOpenCell(cells, cell, Vector3Int.down);
        neighbours[3] = CreateOpenCell(cells, cell, Vector3Int.left);
        
        return GetLowestScore(neighbours, distanceScore);
    }
    
    private Cell GetLowestScore(Cell[] cells, int distanceScore)
    {
        Cell checkedCell = null;
        
        foreach (Cell cell in cells)
        {
            if (cell is null || cell.distanceScore >= distanceScore)
                continue;

            distanceScore = cell.distanceScore;
            checkedCell = cell;
        }
        
        return checkedCell;
    }

    private Cell CreateOpenCell(Cell[,] cells, Cell cell, Vector3Int direction)
    {
        Vector3Int futurePosition = new(
            cell.position.x + direction.x,
            cell.position.y + direction.y,
            0);

        DungeonTile tile = tilemap.GetTile<DungeonTile>(futurePosition);

        if (tile is null)
            return null;
        
        if (!tile.Walkable) 
            return null;
        
        int startToCell = CalculateDistance(position, futurePosition);
        int targetToCell = CalculateDistance(futurePosition, player.position);

        Cell newCell = new(futurePosition, startToCell, targetToCell);
        
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
    public int distanceFromTarget;

    public int distanceScore;

    public Cell(Vector3Int position, int distanceFromStart, int distanceFromTarget)
    {
        this.position = position;
        this.distanceFromStart = distanceFromStart;
        this.distanceFromTarget = distanceFromTarget;

        distanceScore = this.distanceFromStart + this.distanceFromTarget;
    }
}
