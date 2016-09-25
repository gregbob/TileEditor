using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[System.Serializable]
public class TileSelection  {

    private TileMap tiles;
    private HashSet<Tile> selected;
    private Vector2 scrollPos;

    public Color selectColor = Color.blue;
    private Color deSelectColor = Color.white;

    public TileSelection(TileMap tiles)
    {
        this.tiles = tiles;
        selected = new HashSet<Tile>();
    }

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Remove selected"))
        {      
            tiles.RemoveSelected(ArrayUtility.HashSetToArray(selected));
            selected.Clear();
        }
        if (GUILayout.Button("Deselect all"))
        {
            DeselectAll();
        }
        selectColor = EditorGUILayout.ColorField(selectColor);

        GUILayout.EndHorizontal();

    }

    public void Select(Tile tile)
    {
        selected.Add(tile);
        gregbob.EditorUtility.ChangeColor(tile.GetComponent<Renderer>(), selectColor);
    }

    private void DeselectAll()
    {
        for (int i = 0; i < tiles.size.x; i++)
        {
            for (int j = 0; j < tiles.size.y; j++)
            {
                Deselect(tiles.tiles[i][j]);
            }
        }
    }

    public void Deselect(Tile tile)
    {
        selected.Remove(tile);
        gregbob.EditorUtility.ChangeColor(tile.GetComponent<Renderer>(), deSelectColor);
    }
    
    public void SelectRangeSquare(Tile t1, Tile t2)
    {
        int leftX;
        int rightX;

        int bottomY;
        int topY;

        if (t1.GetX() <= t2.GetX())
        {
            leftX = t1.GetX();
            rightX = t2.GetX();
        } else
        {
            leftX = t2.GetX();
            rightX = t1.GetX();
        }
            
        if (t1.GetY() <= t2.GetY())
        {
            bottomY = t1.GetY();
            topY = t2.GetY();
        } else
        {
            bottomY = t2.GetY();
            topY = t1.GetY();
        }

        for (int i = leftX; i <= rightX; i++)
        {
            for (int j = bottomY; j <= topY; j++)
            {
                Select(tiles.tiles[i][j]);
            }
        }

        
    }
}
