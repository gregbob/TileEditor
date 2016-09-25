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
        selectColor = EditorGUILayout.ColorField("Select", selectColor);
        if (GUILayout.Button("Remove selected"))
        {
            
            tiles.RemoveSelected(ArrayUtility.HashSetToArray(selected));
            selected.Clear();
        }

    }

    public void Select(Tile tile)
    {
        selected.Add(tile);
        gregbob.EditorUtility.ChangeColor(tile.GetComponent<Renderer>(), selectColor);
    }

    public void Deselect(Tile tile)
    {
        selected.Remove(tile);
        gregbob.EditorUtility.ChangeColor(tile.GetComponent<Renderer>(), deSelectColor);
    }
}
