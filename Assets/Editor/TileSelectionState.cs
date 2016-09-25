using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class TileSelectionState : TileEditorState {

    private TileMap tileMap;
    private HashSet<Tile> selected;
    private Vector2 scrollPos;

    public Color selectColor = Color.blue;
    private Color deSelectColor = Color.white;

    private List<Tile> squareSelector = new List<Tile>();

    public TileSelectionState(TileMap tileMap)
    {
        this.tileMap = tileMap;
        selected = new HashSet<Tile>();

    }

    public override void OnSceneGUI(Event e)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
        {
            RaycastHit hit;

            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));

            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = Tile.GetTile(hit.collider.gameObject);
                if (tile != null)
                {
                    if (e.shift)
                    {
                        Select(tile);
                        squareSelector.Add(tile);
                        if (squareSelector.Count == 2)
                        {
                            SelectRangeSquare(squareSelector[0], squareSelector[1]);
                            squareSelector.Clear();

                        }
                    } else
                    {
                        if (e.button == 0)
                        {
                            Select(tile);

                        }
                        else if (e.button == 1)
                        {
                            Deselect(tile);
                        }

                        squareSelector.Clear();
                    }
                    


                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
    }
    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Selector");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Remove selected"))
        {
            tileMap.RemoveSelected(ArrayUtility.HashSetToArray(selected));
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
        for (int i = 0; i < tileMap.size.x; i++)
        {
            for (int j = 0; j < tileMap.size.y; j++)
            {
                Deselect(tileMap.tiles[i][j]);
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
        }
        else
        {
            leftX = t2.GetX();
            rightX = t1.GetX();
        }

        if (t1.GetY() <= t2.GetY())
        {
            bottomY = t1.GetY();
            topY = t2.GetY();
        }
        else
        {
            bottomY = t2.GetY();
            topY = t1.GetY();
        }

        for (int i = leftX; i <= rightX; i++)
        {
            for (int j = bottomY; j <= topY; j++)
            {
                Select(tileMap.tiles[i][j]);
            }
        }


    }


}
