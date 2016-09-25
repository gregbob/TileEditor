using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileRaiserState : TileEditorState  {

    private float raiseHeight = 1;
    private TileMap map;

    public TileRaiserState(TileMap map)
    {
        this.map = map;
    }

    public override void OnSceneGUI(Event e )
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
        {
            RaycastHit hit;

            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));

            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = Tile.GetTile(hit.collider.gameObject);

                float inc = raiseHeight;
                if (e.shift)
                {
                    inc = inc * .5f;
                }
                if (e.button == 0)
                {
                    tile.RaiseTile(inc);
                }
                else if (e.button == 1)
                {
                    tile.LowerTile(inc);
                }
            }
            e.Use();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
    }


    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Raise or lower tiles");
        raiseHeight = EditorGUILayout.FloatField("Increase height by: ", raiseHeight);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Raise all"))
        {
            map.RaiseAll(raiseHeight);

        }
        if (GUILayout.Button("Lower all"))
        {
            map.LowerAll(raiseHeight);

        }
        EditorGUILayout.EndHorizontal();
    }
}
