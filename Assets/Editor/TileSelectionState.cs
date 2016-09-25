using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class TileSelectionState : TileEditorState {

    TileSelection selector;

    private List<Tile> square = new List<Tile>();

    public TileSelectionState(TileSelection selector)
    {
        this.selector = selector;

    }

    public void OnSceneGUI(Event e)
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
                        selector.Select(tile);
                        square.Add(tile);
                        if (square.Count == 2)
                        {
                            selector.SelectRangeSquare(square[0], square[1]);
                            square.Clear();

                        }
                    } else
                    {
                        if (e.button == 0)
                        {
                            selector.Select(tile);

                        }
                        else if (e.button == 1)
                        {
                            selector.Deselect(tile);
                        }

                        square.Clear();
                    }
                    


                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
    }

    
}
