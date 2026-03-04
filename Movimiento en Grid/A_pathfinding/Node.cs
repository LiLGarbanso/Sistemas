using UnityEngine;

public class Node
{
    public Vector2Int pos;
    public bool walkable;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(Vector2Int pos, bool walkable)
    {
        this.pos = pos;
        this.walkable = walkable;

        gCost = int.MaxValue;
        hCost = 0;
        parent = null;
    }
}
