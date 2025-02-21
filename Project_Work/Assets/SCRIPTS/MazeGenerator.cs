using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private Stack<MazeCell> path;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];
        path = new Stack<MazeCell>();

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

    private void GenerateMainPath(MazeCell start, MazeCell end)
    {
        MazeCell current = start;
        path.Push(current);

        MazeCell next = null;

        while (current != end)
        {
            current.Visit();

            var NonVisitedAdiacentCells = GetNonVisitedCells(current).ToList();

            if (NonVisitedAdiacentCells.Count == 0)
            {
                current = Backtrack(); // trova la cella del mainPath piu vicina alla fine
                path.Push(current);
            }
            else
            {
                // Aumenta la probabilità di scegliere destra e basso in modo da limitare i punti cechi
                // e arrivare prima alla fine lasciando spazio ai percosi secondari
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
                        weightedCells.Add(cell); // 4/5 80% di probabilità destra o basso
                    }
                    else
                    {
                        weightedCells.Add(cell); // 1/5 20% di probabilità sinistra o alto
                    }
                }

                next = weightedCells[UnityEngine.Random.Range(0, weightedCells.Count)];
                ClearWalls(current, next);
                current = next;
                path.Push(current);
            }
        }
        end.Visit();
    }

    private void GenerateSecondaryPaths()
    /// <summary>
    /// genera i percorsi secondari partendo dal primo nodo e si ferma quando il percorso si incastra
    /// ritorna al percorso principale e trova il nodo successivo con delle celle adiacenti libere e fa partire un percorso nuovo
    /// ripete fino alla fine del percorso principale
    /// se restano celle libere ripete ripartendo dal primo nodo e crea i percorsi "terziari" e cosi via
    /// </summary>
    {
        bool hasUnvisitedCells;

        do
        {
            hasUnvisitedCells = false;

            var pathList = path.Reverse<MazeCell>().ToList();

            foreach (var cell in pathList)
            {
                MazeCell current = cell;

                while (true)
                {
                    var nonVisitedAdiacentCells = GetNonVisitedCells(current).ToList();

                    if (nonVisitedAdiacentCells.Count == 0) break;

                    MazeCell next = nonVisitedAdiacentCells[UnityEngine.Random.Range(0, nonVisitedAdiacentCells.Count)];
                    ClearWalls(current, next);
                    next.Visit();
                    path.Push(next);
                    current = next;
                    hasUnvisitedCells = true;
                    
                }
            }
        } while (hasUnvisitedCells);
    }

    private MazeCell Backtrack()
    /// <summary>
    /// trova la cella più vicina alla fine dal percorso principale.
    /// invece di trovare la prima cella con celle adiacenti non visitate del percorso principale
    /// trova la più vicina alla fine. In questo modo il percorso principale ha una probabilità
    /// minore di occupare quasi tutto il labirinto e maggiore di arrivare alla fine lascinado spazio ai percorsi secondari
    /// </summary>
    {
        MazeCell closestCell = null;
        float minDistance = float.MaxValue;

        foreach (var cell in path)
        {
            var nonVisitedAdiacentCells = GetNonVisitedCells(cell).ToList();
            if (nonVisitedAdiacentCells.Count > 0)
            {
                float distance = Vector3.Distance(cell.transform.position, _mazeGrid[_mazeWidth - 1, _mazeDepth - 1].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCell = cell;
                }
            }
        }

        return closestCell;
    }

    private IEnumerable<MazeCell> GetNonVisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited) yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited) yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited) yield return _mazeGrid[x, z - 1];
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
}