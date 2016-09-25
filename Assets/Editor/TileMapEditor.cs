using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;


public enum EditorState { ADD_REMOVE, PAINT, RAISE_LOWER, TEXTURE, ROTATE, PLACE_OBJECT, SELECT };

[CustomEditor (typeof(TileMap))]
public class TileMapEditor : Editor {

    private HashSet<Tile> _toRemove = new HashSet<Tile>();

    private float raiseHeight = 1;
    private float rotateBy = 90;

    private Material paintMaterial;
    private GameObject objToPlace;
    private GameObject objToPlacePreview;
    private bool visitableOnPlace = true;
    private EditorState state = EditorState.ADD_REMOVE;
    private bool[] stateToggles = new bool[6]; // Number of states

    private GameObject[] allPrefabs;
    private Material[] allMaterials;

    private ResizableSelectionWindow prefabSelection;
    private ResizableSelectionWindow materialSelection;

    private TileSelection tileSelection;
    private TileSelectionState tss;

    private TilePainter tilePainter;
    private TilePainterState tps;



    void OnEnable()
    {  
        stateToggles[(int)EditorState.ADD_REMOVE] = true;

        allPrefabs = System.Array.ConvertAll(FindAssets.FindAllAssets("prefab"), item => (GameObject)item);
        allMaterials = System.Array.ConvertAll(FindAssets.FindAllAssets("material"), item => (Material)item);
        prefabSelection = new ResizableSelectionWindow(64);
        materialSelection = new ResizableSelectionWindow(64);
        prefabSelection.onSelection += PrefabSelection;
        materialSelection.onSelection += MaterialSelection;

        tileSelection = new TileSelection(target as TileMap);
        tss = new TileSelectionState(tileSelection);

        tilePainter = new TilePainter();
        tps = new TilePainterState(tilePainter);
        
    }

    void OnDisable()
    {
        DestroyImmediate(objToPlacePreview);

        prefabSelection.onSelection -= PrefabSelection;
    }

    private Tile GetTile(GameObject obj)
    {
        Tile tile = null;
        tile = obj.GetComponent<Tile>();
        if (tile != null)
            return tile;
        tile = obj.GetComponentInParent<Tile>();
        if (tile != null)
            return tile;

        return null;
    }

    public void OnMouseMove(Event e)
    {
        if (state == EditorState.PLACE_OBJECT)
        {
            RaycastHit hit;
            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), out hit))
            {

                if (objToPlacePreview != null)
                    objToPlacePreview.transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1, hit.collider.transform.position.z);
            }
        }
    }

    // Currently assumes we are clicking on a tile 100% of the time. Seems to work. for now...
    public void OnMouseDown(Event e, GameObject clickedOn)
    {
        Tile tile = GetTile(clickedOn);
        if (state == EditorState.ADD_REMOVE)
        {
            if (e.button == 0)
            {
                tileSelection.Select(tile);

            }
            else if (e.button == 1)
            {
                tileSelection.Deselect(tile);
            }


        }
        else if (state == EditorState.PAINT)
        {
            //if (e.button == 0)
            //{
            //    if (e.control)
            //    {
            //        paintColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
            //    }
            //    gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), paintColor);
            //}
            //else if (e.button == 1)
            //{
            //    if (e.control)
            //    {
            //        paintSecondaryColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
            //    }
            //    gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), paintSecondaryColor);
            //}


        }
        else if (state == EditorState.RAISE_LOWER)
        {
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
        else if (state == EditorState.TEXTURE)
        {
            if (e.control)
            {
                paintMaterial = clickedOn.GetComponent<Renderer>().material;
            }
            else
            {
                clickedOn.GetComponent<Renderer>().material = paintMaterial;
                //ChangeTexture(clickedOn.GetComponent<Renderer>(), paintTexture);

            }
        }
        else if (state == EditorState.ROTATE)
        {
            clickedOn.transform.Rotate(clickedOn.transform.up, rotateBy);
        }
        else if (state == EditorState.PLACE_OBJECT)
        {
            if (e.button == 0)
                tile.PlaceObject(objToPlace, visitableOnPlace);
            else if (e.button == 1)
                tile.RemoveObject();
        }
    }

    public void OnKeyDown(Event e)
    {
        if (e.keyCode == KeyCode.I)
        {
            ChangeState(EditorState.ADD_REMOVE, state);
        }
        else if (e.keyCode == KeyCode.P)
        {
            ChangeState(EditorState.PAINT, state);
        }
        else if (e.keyCode == KeyCode.R)
        {
            ChangeState(EditorState.RAISE_LOWER, state);
        }
        else if (e.keyCode == KeyCode.T)
        {
            ChangeState(EditorState.TEXTURE, state);
        }
        else if (e.keyCode == KeyCode.O)
        {
            ChangeState(EditorState.ROTATE, state);
        }
        else if (e.keyCode == KeyCode.U)
        {
            ChangeState(EditorState.PLACE_OBJECT, state);
        }
        
    }
    // Don't call e.Use() every frame or the scene view will never update until you manually click out and back in!!
    void OnSceneGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            OnKeyDown(e);
            e.Use();
            EditorSceneManager.MarkAllScenesDirty();
        }

        if (state == EditorState.ADD_REMOVE)
        {
            tss.OnSceneGUI(Event.current);
            Debug.Log("??");
            return;
        } else if (state == EditorState.PAINT)
        {
            tps.OnSceneGUI(Event.current);
            return;
        }
       
        
        // Get controlID to prevent clicking to deselect the selected object in hierarchy
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        if (e.type == EventType.MouseMove)
        {
            OnMouseMove(e);
            e.Use();
        }
        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
        {
            RaycastHit hit;

            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));
            
            if (Physics.Raycast(ray, out hit))
            {
                OnMouseDown(e, hit.collider.gameObject);
            }
            e.Use();
            EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }

        Repaint();   // Forces inspector to redraw. Useful for situations where inspector is modified outside of OnInspectorGUI

        
    }


    public void ChangeState(EditorState newState, EditorState currState)
    {
        state = newState;
        SetOnlyStateToggleTrue((int)newState);
        if (currState == EditorState.PLACE_OBJECT)
        {
            DestroyImmediate(objToPlacePreview);
        }
        if (newState == EditorState.ADD_REMOVE)
        {
            
        }
        else if (newState == EditorState.PAINT)
        {

        }
        else if (newState == EditorState.RAISE_LOWER)
        {

        }
        else if (newState == EditorState.TEXTURE)
        {

        }
        else if (newState == EditorState.ROTATE)
        {

        } else if (newState == EditorState.PLACE_OBJECT)
        {

        }        
    }


    

    private void CreateSectionLabel(string name)
    {
        GUIStyle sectionLabelStyle = new GUIStyle();
        sectionLabelStyle.fontSize = 15;
        sectionLabelStyle.alignment = TextAnchor.UpperCenter;
        sectionLabelStyle.border = new RectOffset(10, 10, 10, 10);
        sectionLabelStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(name, sectionLabelStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    public override void OnInspectorGUI()
    {
        bool[] cachedStateToggles = ArrayUtility.ArrayCopy(stateToggles); // Store initial state of toggles to check if its changed


        CreateSectionLabel("Paramaters");

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        CreateSectionLabel("Editor");

        // Add/Remove state
        stateToggles[(int)EditorState.ADD_REMOVE] = EditorGUILayout.BeginToggleGroup("Add or remove tiles [I]", stateToggles[(int)EditorState.ADD_REMOVE]);
        tileSelection.OnGUI();
        EditorGUILayout.EndToggleGroup();

    
        // Paint state
        stateToggles[(int)EditorState.PAINT] = EditorGUILayout.BeginToggleGroup("Paint tiles [P]", stateToggles[(int)EditorState.PAINT]);
        tilePainter.OnGUI();
        EditorGUILayout.EndToggleGroup();

        // Raise/Lower state
        stateToggles[(int)EditorState.RAISE_LOWER] = EditorGUILayout.BeginToggleGroup("Raise or lower tiles [R]", stateToggles[(int)EditorState.RAISE_LOWER]);
        raiseHeight = EditorGUILayout.FloatField("Increase height by: ", raiseHeight);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Raise all"))
        {
            TileMap tileMap = target as TileMap;
            tileMap.RaiseAll(raiseHeight);
            
        }
        if (GUILayout.Button("Lower all"))
        {
            TileMap tileMap = target as TileMap;
            tileMap.LowerAll(raiseHeight);

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();

        
        // Texture state
        stateToggles[(int)EditorState.TEXTURE] = EditorGUILayout.BeginToggleGroup("Texture tiles [T]", stateToggles[(int)EditorState.TEXTURE]);
        EditorGUILayout.ObjectField("Texture to paint", paintMaterial, typeof(Material), true);
        materialSelection.CreateResizableBox(allMaterials);
        EditorGUILayout.EndToggleGroup();


        // Rotate state
        stateToggles[(int)EditorState.ROTATE] = EditorGUILayout.BeginToggleGroup("Rotate tiles [O]", stateToggles[(int)EditorState.ROTATE]);
        rotateBy = EditorGUILayout.FloatField("Rotate by: ", rotateBy);
        EditorGUILayout.EndToggleGroup();

        // Place object state
        stateToggles[(int)EditorState.PLACE_OBJECT] = EditorGUILayout.BeginToggleGroup("Place Object [U]", stateToggles[(int)EditorState.PLACE_OBJECT]);
        objToPlace = (GameObject)EditorGUILayout.ObjectField("Object to place", objToPlace, typeof(GameObject), true);
        visitableOnPlace = EditorGUILayout.Toggle("Tile visitable after placing object?", visitableOnPlace);
        prefabSelection.CreateResizableBox(allPrefabs);
        EditorGUILayout.EndToggleGroup();


        if (GUI.changed)
            EditorUtility.SetDirty(target);


        // Check to see if toggle value changed
        int idx = ArrayUtility.FindDifferentElement(cachedStateToggles, stateToggles);
        if (idx != -1)
            ChangeState((EditorState)idx, state);
        
    }

    #region Delegates   
    public void MaterialSelection(object sender, OnSelectionEventArgs e)
    {
        paintMaterial = (Material)e.clicked;
    }

    public void PrefabSelection(object sender, OnSelectionEventArgs e)
    {
        TileMap tm = target as TileMap;

        objToPlace = (GameObject)e.clicked;
        if (objToPlacePreview != null)
            DestroyImmediate(objToPlacePreview);

        objToPlacePreview = Instantiate(objToPlace);
        objToPlacePreview.GetComponent<Collider>().enabled = false;
        
        objToPlacePreview.transform.SetParent(tm.transform);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx">Set idx to true. Set all others false.</param>
    private void SetOnlyStateToggleTrue(int idx)
    {
        for (int i = 0; i < stateToggles.Length; i++)
        {
            stateToggles[i] = false;
        }
        stateToggles[idx] = true;
    }





}
