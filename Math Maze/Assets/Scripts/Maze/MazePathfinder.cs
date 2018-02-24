using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePathfinder : MonoBehaviour
{

    List<MazeCell> previousCells = new List<MazeCell>();
    List<MazeCell> correctPath = new List<MazeCell>();

    private MazeCell endCell;


    public string FindCorrectDirection(MazeCell firstCell, MazeCell _endCell)
    {
        previousCells.Clear();
        endCell = _endCell;
        print(endCell.coordinates);
        bool solvable = CheckAdjacentCells(firstCell);

        if (solvable)
        {
            foreach (MazePassage passage in firstCell.GetComponentsInChildren<MazePassage>())
            {
                if (correctPath.Contains(passage.otherCell))
                    return passage.direction.ToString();
            }
        }
        return null;
    }

    private bool CheckAdjacentCells(MazeCell currentCell)
    {
        if (currentCell == endCell)
        {
            //Debug.Log("EndCell");
            return true;
        }
        if (currentCell.GetComponentsInChildren<MazePassage>().Length < 2 || previousCells.Contains(currentCell))
        {
            //Debug.Log("Wall or previous cell");
            return false;
        }
        previousCells.Add(currentCell);
        foreach (MazePassage passage in currentCell.GetComponentsInChildren<MazePassage>())
        {
            //Debug.Log("In passage loop");
            if (CheckAdjacentCells(passage.otherCell))
            {
                //Debug.Log("Correct Path");
                correctPath.Add(currentCell);
                return true;
            }
        }
        //Debug.Log("No return values");
        return false;
    }
}
