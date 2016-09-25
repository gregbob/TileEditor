using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class TileSelectionState : TileEditorState {

    TileSelection selector;

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
                    if (e.button == 0)
                    {
                        selector.Select(tile);

                    }
                    else if (e.button == 1)
                    {
                        selector.Deselect(tile);
                    }


                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
    }
}
