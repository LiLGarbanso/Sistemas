using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance;
    Node[,] grid;
    public Vector2Int startpos, targetpos;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void BuildGrid()
    {
        int width = GridManager.Instance.GetGridLogica().GetLength(0);
        int height = GridManager.Instance.GetGridLogica().GetLength(1);

        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int aux = new Vector2Int(x, y);
                grid[x, y] = new Node(aux, GridManager.Instance.GetTile(aux).isWalkable);
            }
            
        }
    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        Vector2Int[] dirs =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (var d in dirs)
        {
            Vector2Int p = node.pos + d;

            if (p.x >= 0 && p.x < grid.GetLength(0) &&
                p.y >= 0 && p.y < grid.GetLength(1))
            {
                neighbours.Add(grid[p.x, p.y]);
            }
        }

        return neighbours;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {

        Node startNode = grid[start.x, start.y];
        Node targetNode = grid[target.x, target.y];

        if (!targetNode.walkable)
            return null;

        ResetPathFindingMatrix();

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                    openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost)
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCost = current.gCost + 10;       //Es aqui donde hay que settear el coste de cada casilla si es especial

                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    int Heuristic(Node a, Node b)
    {
        return (Mathf.Abs(a.pos.x - b.pos.x) +
                Mathf.Abs(a.pos.y - b.pos.y)) * 10;
    }

    List<Vector2Int> RetracePath(Node start, Node end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = end;

        while (current != start)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    public void ResetPathFindingMatrix()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y].gCost = int.MaxValue;
                grid[x, y].hCost = 0;
                grid[x, y].parent = null;
            }
        }
    }
}
