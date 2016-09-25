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
        Pathfind.BFS(tiles, tiles[1][1], tiles[(int)size.x - 1][(int)size.y - 1], null);

        Pathfind.ShowMoves(tiles, tiles[9][9], 3, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //.Log(test);
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

  
}
