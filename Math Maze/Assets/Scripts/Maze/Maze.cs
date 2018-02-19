using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{

    public Vector2Int size;

    public MazeCell cellPrefab;

    public Material intersectionMaterial;
    public Material startMaterial;
    public Material endMaterial;

    public MazePassage passagePrefab;
    public MazeWall wallPrefab;

    private MazeCell[,] cells;
    public MazeCell[] endpointCells;

    public MazeCell mazeStart;
    public MazeCell mazeEnd;

    public Vector2Int RandomCoordinates
    {
        get
        {
            return new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
        }
    }

    public bool ContainsCoordinates(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y;
    }

    public MazeCell GetCell(Vector2Int coordinates)
    {
        return cells[coordinates.x, coordinates.y];
    }

    public void Generate()
    {

        cells = new MazeCell[size.x, size.y];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {

            DoNextGenerationStep(activeCells);
        }

        int index = 0;
        endpointCells = new MazeCell[cells.Length / 4];
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                List<MazeWall> walls = new List<MazeWall>();
                cells[x, z].GetComponentsInChildren<MazeWall>(false, walls);
                if (walls != null)
                {
                    if (walls.Count < 2)
                    {
                        cells[x, z].GetComponentInChildren<Renderer>().material = intersectionMaterial;
                    }
                    else if (walls.Count == 3)
                    {
                        endpointCells[index] = cells[x, z];
                        index++;
                    }
                }
            }
        }

        foreach (MazeCell possibleCell in endpointCells)
        {
            if (possibleCell != null)
            {
                //possibleCell.GetComponentInChildren<Renderer>().material = startMaterial;

                if (mazeStart == null)
                {
                    mazeStart = possibleCell;
                }
                else if (getDistanceToStartCorner(possibleCell) < getDistanceToStartCorner(mazeStart))
                {
                    print(possibleCell.coordinates);
                    print(getDistanceToStartCorner(possibleCell));
                    print(getDistanceToStartCorner(mazeStart));
                    mazeStart = possibleCell;
                }

                if (mazeEnd == null)
                {
                    mazeEnd = possibleCell;
                }
                else if (getDistanceToEndCorner(possibleCell) < getDistanceToEndCorner(mazeEnd))
                {
                    mazeEnd = possibleCell;
                }
            }
        }

        mazeStart.isStart = true;
        mazeStart.GetComponentInChildren<Renderer>().material = startMaterial;
        mazeStart.gameObject.tag = "mazeStart";

        mazeEnd.isEnd = true;
        mazeEnd.GetComponentInChildren<Renderer>().material = endMaterial;
        mazeEnd.gameObject.tag = "mazeEnd";

    }

    private float getDistanceToStartCorner(MazeCell checkCell)
    {
        float distance = Mathf.Abs(Vector2Int.Distance(checkCell.coordinates, cells[0, 0].coordinates));
        return distance;

    }

    private float getDistanceToEndCorner(MazeCell checkCell)
    {
        float distance = Mathf.Abs(Vector2Int.Distance(cells[size.x - 1, size.y - 1].coordinates, checkCell.coordinates));
        return distance;
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }
        MazeDirection direction = currentCell.RandomUninitializedDirection;
        Vector2Int coordinates = currentCell.coordinates + direction.ToVector2Int();
        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else
        {
            CreateWall(currentCell, null, direction);
        }
    }

    private MazeCell CreateCell(Vector2Int coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.y] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.y;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.y - size.y * 0.5f + 0.5f);
        return newCell;
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }
}