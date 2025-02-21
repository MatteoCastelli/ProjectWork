using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeGeneratorAlgorithm : MonoBehaviour
{
    private bool isPaused = false;
    private int c = 0;

    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeDepth;

    private MazeCell[,] _mazeGrid;
    private Stack<MazeCell> path;

    public IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];
        path = new Stack<MazeCell>();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                yield return new WaitForSeconds(0.005f);
            }
        }

        yield return GenerateMainPath(_mazeGrid[0, 0], _mazeGrid[_mazeWidth - 1, _mazeDepth - 1]);
        yield return new WaitForSeconds(1.0f);
        yield return GenerateSecondaryPaths();
    }

    private IEnumerator GenerateMainPath(MazeCell start, MazeCell end)
    {
        MazeCell current = start;
        path.Push(current);

        MazeCell next = null;

        while (current != end)
        {
            while (isPaused) yield return null;

            current.Visit();
            current.ColorPath(c);

            var NonVisitedAdiacentCells = GetNonVisitedCells(current).ToList();
            
            if (NonVisitedAdiacentCells.Count == 0)
            {
                current = Backtrack();
                path.Push(current);
            }
            else 
            {
                
                List<MazeCell> weightedCells = new List<MazeCell>();
                foreach (var cell in NonVisitedAdiacentCells)
                {
                    int x = (int)cell.transform.position.x;
                    int z = (int)cell.transform.position.z;
                    int currentX = (int)current.transform.position.x;
                    int currentZ = (int)current.transform.position.z;

                    if (x > currentX || z > currentZ) 
                    {
                        weightedCells.Add(cell);
                        weightedCells.Add(cell);
                        weightedCells.Add(cell);
                        weightedCells.Add(cell);
                    }
                    else
                    {
                        weightedCells.Add(cell);
                    }
                }

                next = weightedCells[UnityEngine.Random.Range(0, weightedCells.Count)];
                yield return new WaitForSeconds(0.15f);
                ClearWalls(current, next);
                current = next;
                path.Push(current);
            }
        }
        end.Visit();
        end.ColorPath(c);
    }

    private IEnumerator GenerateSecondaryPaths()
    
    {
        bool hasUnvisitedCells;

        do
        {
            hasUnvisitedCells = false;

            var pathList = path.Reverse<MazeCell>().ToList();
            c++;

            foreach (var cell in pathList)
            {
                MazeCell current = cell;

                while (true)
                {
                    while (isPaused) yield return null;

                    var nonVisitedAdiacentCells = GetNonVisitedCells(current).ToList();

                    if (nonVisitedAdiacentCells.Count == 0) break;

                    MazeCell next = nonVisitedAdiacentCells[UnityEngine.Random.Range(0, nonVisitedAdiacentCells.Count)];
                    ClearWalls(current, next);
                    next.Visit();
                    next.ColorPath(c);
                    path.Push(next);
                    current = next;
                    hasUnvisitedCells = true;
                    yield return new WaitForSeconds(0.15f);
                }
            }
        } while (hasUnvisitedCells);
    }

    private MazeCell Backtrack()

    {
        MazeCell closestCell = null;
        float minDistance = float.MaxValue;

        foreach (var cell in path)
        {
            var nonVisitedAdiacentCells = GetNonVisitedCells(cell).ToList();
            if (nonVisitedAdiacentCells.Count > 0)
            {
                float distance = Vector3.Distance(cell.transform.position, _mazeGrid[_mazeWidth-1, _mazeDepth-1].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCell = cell;
                }
            }
        }
        
        return closestCell;
    }

    private IEnumerable<MazeCell> GetNonVisitedCells(MazeCell current)//ok
    {
        int x = (int)current.transform.position.x;
        int z = (int)current.transform.position.z;

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited) yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited) yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited) yield return _mazeGrid[x, z - 1];
    }

    private void ClearWalls(MazeCell current, MazeCell next)
    {
        if (current == null) return;

        if (current.transform.position.x < next.transform.position.x)
        {
            current.ClearRightWall();
            next.ClearLeftWall();
        }
        else if (current.transform.position.x > next.transform.position.x)
        {
            current.ClearLeftWall();
            next.ClearRightWall();
        }
        else if (current.transform.position.z < next.transform.position.z)
        {
            current.ClearFrontWall();
            next.ClearBackWall();
        }
        else if (current.transform.position.z > next.transform.position.z)
        {
            current.ClearBackWall();
            next.ClearFrontWall();
        }
    }

    public void Quit()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void AlghorithmVieer()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void Pause()
    {
        isPaused = !isPaused;
    }
}