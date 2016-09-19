using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileMapWindow : EditorWindow {

    int width = 0;
    int height = 0;

    [MenuItem("Window/TileMap")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TileMapWindow));
    }

    //void OnSceneGUI(SceneView sceneView)
    //{
    //    Event e = Event.current;
    //    // Get controlID to prevent clicking to deselect the selected object in hierarchy
    //    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

    //    if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
    //    {
    //        var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Debug.Log("Clicked on " + hit.collider.gameObject.name);
    //        }

    //        e.Use();
    //    }

        

    //}

    //void OnEnable()
    //{
    //    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    //}

    //void OnDisable()
    //{
    //    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    //}


    void OnGUI()
    {

        width  = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        if (GUILayout.Button("Create Tile Map"))
        {
            GameObject tileMap = new GameObject("_TileMap");
            TileMap tm = tileMap.AddComponent<TileMap>();
            tm.InitMap(width, height);
            
        }

    }

}
