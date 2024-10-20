using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class EnemyMovement : GridMovement
{
    [SerializeField] private PlayerMovement player;

    [SerializeField] private int maxDirectDistanceBeforeMove = 100;
    [SerializeField] private int maxDistanceScore = 300;

    public bool hasBeenHit = false;

    private void Start()
    {
        AppendToGameManager();
    }

    private void AppendToGameManager()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        manager.enemies.Add(this);
    }

    private void OnEnable() => player.onMove.AddListener(FindLowestScore);

    private void OnDisable() => player.onMove.RemoveListener(FindLowestScore);

    public void GetHit()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.grey;
        FindFirstObjectByType<GameManager>().enemies.Remove(this);
        Destroy(gameObject, 0.5f);
    }
    private void FindLowestScore()
    {
        if (hasBeenHit) return;
        int currentDistance = CalculateDistance(position, player.position);

        if (currentDistance > maxDirectDistanceBeforeMove)
            return;

        List<Cell> openCells = new();
        List<Cell> closedCells = new();

        Cell startCell = InitializeFirstCell();
        Cell cellToExplore = startCell;

        int ranTimes = 0;

        while (cellToExplore.position != player.position && ranTimes < 1000)
        {
            ranTimes++;
            // Initially sets the lowest distance score to the highest possible so it will search all options.
            int lowestDistanceScore = maxDistanceScore;
            Cell neighbourCell = AddNeighboursToOpenList(openCells, cellToExplore, lowestDistanceScore);

            if (neighbourCell is null)
            {
                foreach (Cell openCell in openCells)
                {
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

            closedCells.Add(cellToExplore);
            openCells.Remove(cellToExplore);

            if (openCells.Count == 0)
                break;
        }

        List<Cell> options = new();

        foreach (Cell closedCell in closedCells)
        {
            Vector3Int middlePoint = startCell.position - closedCell.position;

            if (middlePoint is { x: <= 1 and >= -1, y: 0 } or { x: 0, y: <= 1 and >= -1 })
                options.Add(closedCell);
        }

        if (options.Count == 0)
            return;

        Cell nextCell = options.Aggregate((cellWithLowestTarget, cell) => cell.distanceFromTarget <= cellWithLowestTarget.distanceFromTarget ? cell : cellWithLowestTarget);

        if (nextCell.distanceFromTarget <= 10)
            player.AttackedByEnemy(this);

        if (nextCell.distanceFromTarget < 10)
            return;

        if (nextCell.distanceFromTarget > maxDirectDistanceBeforeMove)
            return;

        Vector3Int direction = nextCell.position - startCell.position;
        Move(direction);
    }

    private Cell InitializeFirstCell()
    {
        return new Cell(new Vector3Int(position.x, position.y, 0), 0, CalculateDistance(position, player.position));
    }

    private int CalculateDistance(Vector3Int start, Vector3Int target)
    {
        return (Math.Abs(start.x - target.x) + Math.Abs(start.y - target.y)) * 10;
    }

    private Cell AddNeighboursToOpenList(List<Cell> cells, Cell cell, int distanceScore)
    {
        List<Cell> neighbours = new()
        {
            CreateOpenCell(cells, cell, Vector3Int.up),
            CreateOpenCell(cells, cell, Vector3Int.right),
            CreateOpenCell(cells, cell, Vector3Int.down),
            CreateOpenCell(cells, cell, Vector3Int.left)
        };

        return GetLowestScore(neighbours, distanceScore);
    }

    private Cell GetLowestScore(List<Cell> cells, int distanceScore)
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

    private Cell CreateOpenCell(List<Cell> cells, Cell cell, Vector3Int direction)
    {
        Vector3Int futurePosition = new(
            cell.position.x + direction.x,
            cell.position.y + direction.y,
            0);

        DungeonTile tile = tilemap.GetTile<DungeonTile>(futurePosition);

        if (tile is null)
            return null;

        if (!tile.Walkable || (tilemap.GetInstantiatedObject(futurePosition).TryGetComponent(out Door door) && !door.IsOpen))
            return null;

        int startToCell = CalculateDistance(position, futurePosition);
        int targetToCell = CalculateDistance(futurePosition, player.position);

        Cell newCell = new(futurePosition, startToCell, targetToCell);

        cells.Add(newCell);

        return newCell;
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
