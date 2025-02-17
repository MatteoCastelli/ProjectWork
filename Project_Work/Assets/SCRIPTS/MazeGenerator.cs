using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeDepth;
    [SerializeField] private GameObject _teleportPrefab;
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private CinemachineFreeLook _freeLookCamera;
    private MazeCell[,] _mazeGrid;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        GenerateMainPath(_mazeGrid[0, 0], _mazeGrid[_mazeWidth - 1, _mazeDepth - 1]);
        GenerateSecondaryPaths();

        PlaceTeleportPrefab(_mazeGrid[_mazeWidth - 1, _mazeDepth - 1]);
        PlaceCharacterPrefab(_mazeGrid[0, 0]);
    }

    private void PlaceCharacterPrefab(MazeCell cell)
    {
        Vector3 position = cell.transform.position;
        GameObject _characterInstance = Instantiate(_characterPrefab, position, Quaternion.identity);
        _freeLookCamera.Follow = _characterInstance.transform;
        _freeLookCamera.LookAt = _characterInstance.transform;
    }


    private void PlaceTeleportPrefab(MazeCell cell)
    {
        Vector3 position = cell.transform.position;
        Instantiate(_teleportPrefab, position, Quaternion.identity);
    }

    private void GenerateMainPath(MazeCell startCell, MazeCell endCell)
    {
        MazeCell currentCell = startCell;
        MazeCell nextCell;

        while (currentCell != endCell)
        {
            currentCell.Visit();
            nextCell = GetNextCellWithBias(currentCell, endCell);

            if (nextCell != null)
            {
                ClearWalls(currentCell, nextCell);
                currentCell = nextCell;
            }
            else
            {
                currentCell = Backtrack(currentCell);
            }
        }

        endCell.Visit();
    }

    private MazeCell GetNextCellWithBias(MazeCell currentCell, MazeCell endCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell).ToList();

        if (unvisitedCells.Count == 0)
            return null;

        var preferredCells = unvisitedCells.Where(c =>
            c.transform.position.x > currentCell.transform.position.x ||
            c.transform.position.z > currentCell.transform.position.z
        ).ToList();

        if (preferredCells.Count > 0)
        {
            return preferredCells[Random.Range(0, preferredCells.Count)];
        }
        else
        {
            return unvisitedCells[Random.Range(0, unvisitedCells.Count)];
        }
    }

    private MazeCell Backtrack(MazeCell currentCell)
    {
        var visitedCells = GetVisitedCells(currentCell).ToList();
        foreach (var cell in visitedCells)
        {
            var unvisitedNeighbors = GetUnvisitedCells(cell).ToList();
            if (unvisitedNeighbors.Count > 0)
            {
                return cell;
            }
        }

        return null;
    }

    /////////////////////////////////////////////////////////////////////////////////////////

    private void GenerateSecondaryPaths()
    {
        while (HasUnvisitedCells())
        {
            var startCell = GetRandomVisitedCellWithUnvisitedNeighbors();
            if (startCell == null)
                break;

            GenerateSecondaryPath(startCell);
        }
    }

    private void GenerateSecondaryPath(MazeCell startCell)
    {
        MazeCell currentCell = startCell;
        int pathLength = Random.Range(5, 11);

        for (int i = 0; i < pathLength; i++)
        {
            var nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                ClearWalls(currentCell, nextCell);
                nextCell.Visit();
                currentCell = nextCell;
            }
            else
            {
                break;
            }
        }
    }

    private MazeCell GetRandomVisitedCellWithUnvisitedNeighbors()
    {

        var candidates = new List<MazeCell>();
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                if (_mazeGrid[x, z].IsVisited && GetUnvisitedCells(_mazeGrid[x, z]).Any())
                {
                    candidates.Add(_mazeGrid[x, z]);
                }
            }
        }

        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }

    private bool HasUnvisitedCells()
    {
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                if (!_mazeGrid[x, z].IsVisited)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell).ToList();
        return unvisitedCells.Count > 0 ? unvisitedCells[Random.Range(0, unvisitedCells.Count)] : null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited) yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited) yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited) yield return _mazeGrid[x, z - 1];
    }

    private IEnumerable<MazeCell> GetVisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && _mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0 && _mazeGrid[x - 1, z].IsVisited) yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && _mazeGrid[x, z + 1].IsVisited) yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0 && _mazeGrid[x, z - 1].IsVisited) yield return _mazeGrid[x, z - 1];
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }
    }

    void Update() { }
}