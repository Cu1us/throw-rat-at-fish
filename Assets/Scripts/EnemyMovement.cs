using UnityEngine;
using System;

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

    private Cell InitializeFirstCell()
    {
        return new Cell(position, 0, CalculateDistance(position, player.position));
    }

    private void OnEnable() => player.onMove.AddListener(FindLowestScore);

    private void OnDisable() => player.onMove.RemoveListener(FindLowestScore);
    
    private void FindLowestScore()
    {
        Cell[,] openCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
        Cell[,] closedCells = new Cell[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];

        Cell nextCell;
        
        Cell startCell = InitializeFirstCell();
        Cell cellToExplore = startCell;
        
        while (cellToExplore.position != player.position)
        {
            // Initially sets the lowest distance score to the highest possible so it will search all options.
            int lowestDistanceScore = (openCells.GetLength(0) + openCells.GetLength(1)) * 10;
            Cell neighbourCell = AddNeighboursToOpenList(openCells, cellToExplore, lowestDistanceScore);

            if (neighbourCell is null)
            {
                foreach (Cell openCell in openCells)
                {
                    lowestDistanceScore = CalculateDistance(position, player.position);
                    
                    if (openCell is null)
                        continue;
                
                    if (openCell.distanceScore < lowestDistanceScore)
                    {
                        cellToExplore = openCell;
                    }
                }
            }
            else
            {
                cellToExplore = neighbourCell;
            }

            int x = arrayOffset.x + cellToExplore.position.x;
            int y = arrayOffset.y + cellToExplore.position.y;

            openCells[x, y] = null;
            
            closedCells[x, y] = cellToExplore;
        }
        
        Move(position - startCell.position);
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
            
            checkedCell = cell;
        }
        
        return checkedCell;
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
