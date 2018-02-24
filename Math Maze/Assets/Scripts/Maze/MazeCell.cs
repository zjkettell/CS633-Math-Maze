using UnityEngine;
using System.Collections.Generic;

public class MazeCell : MonoBehaviour {

	public Vector2Int coordinates;

	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    public Dictionary<string,MazeCell> adjacentCells = new Dictionary<string,MazeCell>();

	private int initializedEdgeCount;

    public bool isStart;
    public bool isEnd;
    public bool isIntersection = false;

	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == MazeDirections.Count;
		}
	}

	public MazeDirection RandomUninitializedDirection {
		get {
			int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
			for (int i = 0; i < MazeDirections.Count; i++) {
				if (edges[i] == null) {
					if (skips == 0) {
						return (MazeDirection)i;
					}
					skips -= 1;
				}
			}
			throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
		}
	}

	public MazeCellEdge GetEdge (MazeDirection direction) {
		return edges[(int)direction];
	}

	public void SetEdge (MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount += 1;
	}

    public void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Player>().currentCell = this;

        if (isEnd)
        {
            FindObjectOfType<GameManager>().LoadEnd();
        }
        if (isIntersection)
        {
            FindObjectOfType<GameManager>().DisplayQuestion(this);
        }
    }
}