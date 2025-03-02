using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private GameObject _teleportPrefab;
    [SerializeField] private GameObject[] enemyPrefabs; // Array di prefab nemici
    private int _mazeWidth;
    private int _mazeDepth;
    private MazeCell[,] _mazeGrid;
    private Stack<MazeCell> path;

    void Start()
    {
        _mazeDepth = MainMenu.SelectedMazeSize;
        _mazeWidth = MainMenu.SelectedMazeSize;
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];
        path = new Stack<MazeCell>();

        Debug.Log("Maze generation started.");

        // Creazione delle celle del labirinto
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        // Genera il percorso principale e secondario
        GenerateMainPath(_mazeGrid[0, 0], _mazeGrid[_mazeWidth - 1, _mazeDepth - 1]);
        GenerateSecondaryPaths();

        // Posiziona il teleport
        PlaceTeleportPrefab(_mazeGrid[_mazeWidth - 1, _mazeDepth - 1]);

        // Spawna i nemici nel labirinto
        SpawnEnemies();
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

        Debug.Log("Generating main path...");

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
        Debug.Log("Main path generated.");
    }

    private void GenerateSecondaryPaths()
    {
        bool hasUnvisitedCells;

        Debug.Log("Generating secondary paths...");

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

        Debug.Log("Secondary paths generated.");
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

    // Funzione per spawnare i nemici nel labirinto
    private void SpawnEnemies()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        Debug.Log("Checking valid spawn positions...");

        // Aggiungi alcune celle casuali per lo spawn
        foreach (var cell in _mazeGrid)
        {
            // Modifica qui per scegliere celle casualmente
            if (Random.value > 0.5f) // 50% di probabilità per ogni cella
            {
                // Posiziona il nemico esattamente sulla Y della cella
                Vector3 spawnPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, cell.transform.position.z);
                spawnPositions.Add(spawnPosition);
            }
        }

        Debug.Log($"Found {spawnPositions.Count} valid spawn positions.");

        // Mescola le posizioni per una distribuzione casuale
        spawnPositions = spawnPositions.OrderBy(x => Random.value).ToList();

        // Istanzia i nemici nelle posizioni selezionate
        foreach (var position in spawnPositions)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            Debug.Log($"Enemy spawned at {position}");

            // Verifica che il nemico sia effettivamente posizionato correttamente
            if (enemy != null)
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Debug.Log($"Enemy collider at {enemyCollider.transform.position}");
                }
            }
        }

        if (spawnPositions.Count == 0)
        {
            Debug.LogWarning("No valid spawn positions found.");
        }
    }



}
