using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfind : MonoBehaviour {

    public delegate bool IsValid(Tile source, Tile destination, Row[] map);

    #region Pathfinding
    public static bool nullValid(Tile source, Tile dest, Row[] map)
    {
        return true;
    }

    public static bool heightValid(Tile source, Tile dest, Row[] map)
    {
        return Mathf.Abs(source.height - dest.height) <= 1;
    }



    public static Tile[] ShowMoves(Row[] tiles, Tile start, int maxDistance, float height)
    {

        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Tile, int> distance = new Dictionary<Tile, int>();

        int currDist = 0;
        distance.Add(start, currDist);
        queue.Enqueue(start);

        Tile current;
        Tile[] nbrs;
        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            currDist = distance[current];

            if (!visited.Contains(current))
            {
                visited.Add(current);

                nbrs = GetNeighbors(tiles, current);
                foreach (Tile child in nbrs)
                {
                    if (!visited.Contains(child) && currDist < maxDistance && Mathf.Abs(current.height - child.height) <= height)
                    {
                        queue.Enqueue(child);
                        distance[child] = currDist + 1;
                    }
                }
            }
        }

        foreach (Tile t in visited)
        {
            t.Highlight(Color.blue);
        }

        return ArrayUtility.HashSetToArray(visited);
    }

    public static bool BFS(Row[] tiles, Tile start, Tile goal, IsValid isValid)
    {
        if (isValid == null)
            isValid = nullValid;
        HighlightStartAndGoal(start, Color.green, goal, Color.red);

        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Tile, Tile> parents = new Dictionary<Tile, Tile>();

        queue.Enqueue(start);

        Tile current;
        Tile[] nbrs;
        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            if (current.Equals(goal))
            {
                Debug.Log("Found the goal!");
                DrawPath(TracePath(parents, goal, start));
                return true;
            }

            if (!visited.Contains(current))
            {
                visited.Add(current);

                nbrs = GetNeighbors(tiles, current, isValid);
                foreach (Tile child in nbrs)
                {
                    if (!visited.Contains(child))
                    {
                        queue.Enqueue(child);
                        if (parents.ContainsKey(child))
                        {
                            parents[child] = current;
                        }
                        else
                        {
                            parents.Add(child, current);
                        }
                    }
                }
            }
        }
        Debug.Log("Can't reach the goal: " + goal + " from starting: " + start);

        return false;
    }

    public static Tile[] GetNeighbors(Row[] tiles, Tile tile, IsValid isValid)
    {
        Vector2 currIdx = tile.index;
        List<Tile> nbrs = new List<Tile>();
        Vector2[] temp = new Vector2[4];
        temp[0] = new Vector2(currIdx.x + 1, currIdx.y);
        temp[1] = new Vector2(currIdx.x, currIdx.y + 1);
        temp[2] = new Vector2(currIdx.x - 1, currIdx.y);
        temp[3] = new Vector2(currIdx.x, currIdx.y - 1);

        for (int i = 0; i < temp.Length; i++)
        {
            if (IndexInBounds(tiles, temp[i]))
            {
                Tile nbr = tiles[(int)temp[i].x][(int)temp[i].y];
                if (nbr.canVisit && isValid(tile, nbr, tiles))
                    nbrs.Add(nbr);
            }
        }

        return nbrs.ToArray();

    }

    public static Tile[] GetNeighbors(Row[] tiles, Tile tile)
    {
        Vector2 currIdx = tile.index;
        List<Tile> nbrs = new List<Tile>();
        Vector2[] temp = new Vector2[4];
        temp[0] = new Vector2(currIdx.x + 1, currIdx.y);
        temp[1] = new Vector2(currIdx.x, currIdx.y + 1);
        temp[2] = new Vector2(currIdx.x - 1, currIdx.y);
        temp[3] = new Vector2(currIdx.x, currIdx.y - 1);

        for (int i = 0; i < temp.Length; i++)
        {
            if (IndexInBounds(tiles, temp[i]))
            {
                Tile nbr = tiles[(int)temp[i].x][(int)temp[i].y];
                if (nbr.canVisit)
                    nbrs.Add(nbr);
            }
        }

        return nbrs.ToArray();

    }

    private static bool IndexInBounds(Row[] tiles, Vector2 idx)
    {
        int xLength = tiles.Length;
        int yLength = tiles[0].row.Length;

        if (idx.x >= 0 && idx.y >= 0 && idx.x < xLength && idx.y < yLength)
            return true;
        return false;
    }

    #endregion

    private static  void HighlightStartAndGoal(Tile start, Color startColor, Tile goal, Color goalColor)
    {
        start.FindHighestTile().GetComponent<Renderer>().material.color = startColor;
        goal.FindHighestTile().GetComponent<Renderer>().material.color = goalColor;
    }
    /// <summary>
    /// Use linerenderer to draw the path from start to finish.
    /// </summary>
    /// <param name="path"></param>
    private static void DrawPath(List<Tile> path)
    {
        GameObject gobj = new GameObject();
        LineRenderer line = gobj.GetComponent<LineRenderer>();
        if (line == null)
            line = gobj.AddComponent<LineRenderer>();

        line.SetWidth(.1f, .1f);

        line.SetVertexCount(path.Count);
        for (int i = 0; i < path.Count; i++)
        {
            var temp = path[i].FindHighestTile().transform.position;
            var pos = new Vector3(temp.x, temp.y + 1, temp.z);
            line.SetPosition(i, pos);
        }
    }
    /// <summary>
    /// Start at the goal and keep grabbing the parent of the current. Should
    /// trace backwards up the path. Path obtained in this way is reversed. 
    /// Reverse path before returning.
    /// </summary>
    /// <param name="parents">List of parents used for backtracking.</param>
    /// <param name="end"></param>
    /// <param name="start"></param>
    /// <returns>Returns path from start to goal.</returns>
    private static List<Tile> TracePath(Dictionary<Tile, Tile> parents, Tile end, Tile start)
    {
        Tile curr = end;
        List<Tile> path = new List<Tile>();

        while (curr != null)
        {
            path.Add(curr);
            curr = parents[curr];
            if (!parents.ContainsKey(curr))
            {
                path.Add(start);
                break;
            }
        }
        path.Reverse();
        return path;
    }
}
