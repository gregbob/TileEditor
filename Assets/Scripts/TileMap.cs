using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Row
{
    // Make public to allow field to become serializable. If you don't do this
    // then Unity won't store the values created in the editor.
    public Tile[] row;

    public Row(int size)
    {
        row = new Tile[size];
    }

    public Tile this[int idx]
    {
        get { return row[idx]; }
        set { row[idx] = value; }
    }
}

[System.Serializable]
public class TileMap : MonoBehaviour {
 
   
    
    public Row[] tiles;

    public Vector2 size;

    public delegate bool IsValid(Tile source, Tile destination, Row[] map);

    // Use this for initialization
    void Start()
    {
        BFS(tiles[1][1], tiles[(int)size.x - 1][(int)size.y - 1], heightValid);
    }

    // Update is called once per frame
    void Update()
    {
        //.Log(test);
    }

    void OnDrawGizmos()
    {
        //if (size == null)
        //    return;

        //for (int i = 0; i <= size.x; i++)
        //{
        //    Gizmos.color = Color.black;
        //    Vector3 start = new Vector3(i - .5f, .5f, -.5f);
        //    Vector3 end = new Vector3(i - .5f, .5f, size.y - .5f);
        //    Gizmos.DrawLine(start, end);
        //}
        //for (int i = 0; i <= size.y; i++)
        //{
        //    Gizmos.color = Color.black;
        //    Vector3 start = new Vector3(- .5f, .5f, i -.5f);
        //    Vector3 end = new Vector3(size.x - .5f, .5f, i - .5f);
        //    Gizmos.DrawLine(start, end);
        //}
    }

    /// <summary>
    /// Create a 2D grid of tiles. Initialize the grid with x rows of length y. 
    /// </summary>
    /// <param name="x">Number of rows.</param>
    /// <param name="y">Size of row.</param>
    public void InitMap(int x, int y)
    {
        GameObject tile = Resources.Load("defaultTile") as GameObject;
        size = new Vector2(x, y);

        tiles = new Row[x];
        for (int i = 0; i < x; i++)
        {
            tiles[i] = new Row(y);

            for (int j = 0; j < y; j++)
            {
                GameObject go = Instantiate(tile);
                go.name = "(" + i + ", " + j + ")";
                go.transform.localScale = new Vector3(1f, 1f, 1f);
                go.transform.position = new Vector3(i, 0, j);
                tiles[i][j] = go.AddComponent<Tile>();
                tiles[i][j].transform.SetParent(transform);
                tiles[i][j].index = new Vector2(i, j);

            }
        }

    }

    /// <summary>
    /// Remove tiles
    /// </summary>
    /// <param name="toRemove"></param>
    public void RemoveSelected(Tile[] toRemove)
    {
        Debug.Log(ToString());
        for (int i = 0; i < toRemove.Length; i++)
        {
            Vector2 idx = toRemove[i].index;
            Tile t = tiles[(int)idx.x][(int)idx.y];
            t.gameObject.SetActive(false);
            t.canVisit = false;
        }
    }
    #region Pathfinding
    public bool nullValid(Tile source, Tile dest, Row[] map)
    {
        return true;
    }

    public bool heightValid(Tile source, Tile dest, Row[] map)
    {
        return Mathf.Abs(source.height - dest.height) <= 1;
    }

    public bool BFS(Tile start, Tile goal, IsValid isValid)
    {
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

                nbrs = GetNeighbors(current, isValid);
                foreach(Tile child in nbrs)
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

    public Tile[] GetNeighbors(Tile tile, IsValid isValid)
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
            if (IndexInBounds(temp[i]))
            {
                Tile nbr = tiles[(int)temp[i].x][(int)temp[i].y];
                if (nbr.canVisit && isValid(tile, nbr, tiles))
                    nbrs.Add(nbr);
            }
        }

        return nbrs.ToArray();

    }

    public void RaiseAll(float inc)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (tiles[i][j].enabled)
                    tiles[i][j].RaiseTile(inc);
            }
        }
    }
    public void LowerAll(float inc)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (tiles[i][j].enabled)
                    tiles[i][j].LowerTile(inc);
            }
        }
    }

    private bool IndexInBounds(Vector2 idx)
    {
        int xLength = tiles.Length;
        int yLength = tiles[0].row.Length;

        if (idx.x >= 0 && idx.y >= 0 && idx.x < xLength && idx.y < yLength)
            return true;
        return false;
    }

    #endregion

    private void HighlightStartAndGoal(Tile start, Color startColor, Tile goal, Color goalColor)
    {
        start.FindHighestTile().GetComponent<Renderer>().material.color = startColor;
        goal.FindHighestTile().GetComponent<Renderer>().material.color = goalColor;
    }
    /// <summary>
    /// Use linerenderer to draw the path from start to finish.
    /// </summary>
    /// <param name="path"></param>
    private void DrawPath(List<Tile> path)
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        if (line == null)
            line = gameObject.AddComponent<LineRenderer>();

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
    private List<Tile> TracePath(Dictionary<Tile, Tile> parents, Tile end, Tile start)
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
