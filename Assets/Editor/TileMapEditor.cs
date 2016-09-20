using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public enum EditorState { ADD_REMOVE, PAINT, RAISE_LOWER, TEXTURE, ROTATE, PLACE_OBJECT, SELECT };

[CustomEditor (typeof(TileMap))]
public class TileMapEditor : Editor {

    private HashSet<Tile> _toRemove = new HashSet<Tile>();

    private float raiseHeight = 1;
    private float rotateBy = 90;

    private Color removeColor = Color.red;
    private Color keepColor = Color.white;
    private Color paintColor = Color.white;
    private Color paintSecondaryColor = Color.black;

    private Texture paintTexture;
    private GameObject objToPlace;
    private GameObject objToPlacePreview;
    private bool visitableOnPlace = true;
    private EditorState state = EditorState.ADD_REMOVE;
    private bool[] stateToggles = new bool[6]; // Number of states
    private Vector2 paintTextureScrollLevel = new Vector2(0,0);

    private Texture[] allTextures;
    private GameObject[] allPrefabs;


    
    void OnEnable()
    {  
        stateToggles[(int)EditorState.ADD_REMOVE] = true;

        allTextures = FindAllTextures();
        allPrefabs = FindAllPrefabs();

    }

    void OnDisable()
    {
        DestroyImmediate(objToPlacePreview);
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
                ChangeColor(tile.GetComponent<Renderer>(), removeColor);
                _toRemove.Add(tile);
                Debug.Log(_toRemove.Count);
            }
            else if (e.button == 1)
            {
                ChangeColor(tile.GetComponent<Renderer>(), keepColor);
                _toRemove.Remove(tile);
                Debug.Log(_toRemove.Count);
            }


        }
        else if (state == EditorState.PAINT)
        {
            if (e.button == 0)
            {
                if (e.control)
                {
                    paintColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(clickedOn.GetComponent<Renderer>(), paintColor);
            }
            else if (e.button == 1)
            {
                if (e.control)
                {
                    paintSecondaryColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(clickedOn.GetComponent<Renderer>(), paintSecondaryColor);
            }


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
                paintTexture = clickedOn.GetComponent<Renderer>().sharedMaterial.mainTexture;
            }
            else
            {
                ChangeTexture(clickedOn.GetComponent<Renderer>(), paintTexture);

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
        }
        else if (e.type == EventType.KeyDown )
        {
            OnKeyDown(e);
            e.Use();

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

    private bool[] Copy(bool[] a)
    {
        bool[] copy = new bool[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            copy[i] = a[i];
        }
        return copy;
    }

    private int FindDifferentElement(bool[] a, bool[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
                return i;
        }
        return -1;
    }

    public override void OnInspectorGUI()
    {

        bool[] cachedStateToggles = Copy(stateToggles);

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 16;
       
        EditorGUILayout.LabelField("Parameters", labelStyle);
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor", labelStyle);

        // Add/Remove state
        stateToggles[(int)EditorState.ADD_REMOVE] = EditorGUILayout.BeginToggleGroup("Add or remove tiles [I]", stateToggles[(int)EditorState.ADD_REMOVE]);
        removeColor = EditorGUILayout.ColorField("Remove", removeColor);
        keepColor = EditorGUILayout.ColorField("Keep", keepColor);
        if (GUILayout.Button("Remove selected"))
        {
            TileMap tileMap = target as TileMap;
            tileMap.RemoveSelected(HashSetToArray(_toRemove));
            _toRemove.Clear();
        }
        EditorGUILayout.EndToggleGroup();

    
        // Paint state
        stateToggles[(int)EditorState.PAINT] = EditorGUILayout.BeginToggleGroup("Paint tiles [P]", stateToggles[(int)EditorState.PAINT]);
        paintColor = EditorGUILayout.ColorField("Primary", paintColor);
        paintSecondaryColor = EditorGUILayout.ColorField("Secondary", paintSecondaryColor);
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

        EditorGUILayout.ObjectField("Texture to paint", paintTexture, typeof(Texture), true);
        GUILayout.FlexibleSpace();
        Vector2 aboveGuiPos = GUILayoutUtility.GetLastRect().position;
        Vector2 bottom = CreateResizableTextureBox(aboveGuiPos, allTextures);
        EditorGUILayout.EndToggleGroup();


        GUILayout.FlexibleSpace();

    
        // Rotate state
        stateToggles[(int)EditorState.ROTATE] = EditorGUILayout.BeginToggleGroup("Rotate tiles [O]", stateToggles[(int)EditorState.ROTATE]);
        rotateBy = EditorGUILayout.FloatField("Rotate by: ", rotateBy);
        EditorGUILayout.EndToggleGroup();

        // Place object state
        stateToggles[(int)EditorState.PLACE_OBJECT] = EditorGUILayout.BeginToggleGroup("Place Object [U]", stateToggles[(int)EditorState.PLACE_OBJECT]);
        objToPlace = (GameObject)EditorGUILayout.ObjectField("Object to place", objToPlace, typeof(GameObject), true);
        visitableOnPlace = EditorGUILayout.Toggle("Tile visitable after placing object?", visitableOnPlace);
        CreateResizablePrefabBox(GUILayoutUtility.GetLastRect().position, allPrefabs);
        EditorGUILayout.EndToggleGroup();

        GUILayout.FlexibleSpace();


        if (GUI.changed)
            EditorUtility.SetDirty(target);


        // Check to see if toggle value changed
        int idx = FindDifferentElement(cachedStateToggles, stateToggles);
        if (idx != -1)
            ChangeState((EditorState)idx, state);
        
    }

    public void CreateResizablePrefabBox(Vector2 position, GameObject[] gameObjs)
    {

        //   Vector2 guiAbove = GUILayoutUtility.GetLastRect().position;
        // Width of current window. Gives width of inspector.

        int textureIconSize = 64;
        int textHeight = 15;
        int boxWidth = Screen.width;
        int numTextures = gameObjs.Length;
        int numTilesInRow = boxWidth / textureIconSize;

        int numRows = (numTextures + numTextures - 1) / numTilesInRow; // Rounded up integer division
        int boxHeight = numRows * textureIconSize;
        int boxPadding = 20;

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.padding = new RectOffset(0, 0, 10, 10);

        GUIStyle g = new GUIStyle();
        g.padding = new RectOffset(5, 5, 10, 0);
        GUI.Box(new Rect(position.x, position.y + boxPadding, boxWidth - position.x * 2, boxHeight), "");

        int count = 0;                          // Decides index in the row
        int row = 0;                            // Decides which row
        for (int i = 0; i < numTextures; i++)
        {
            if (GUI.Button(new Rect(count * textureIconSize + position.x, row * textureIconSize + position.y + boxPadding, textureIconSize, textureIconSize), AssetPreview.GetAssetPreview(gameObjs[i]), g))
            {
                objToPlace = gameObjs[i];
                if (objToPlacePreview != null)                
                    DestroyImmediate(objToPlacePreview);                   

                objToPlacePreview = Instantiate(objToPlace);
                objToPlacePreview.GetComponent<Collider>().enabled = false;
                TileMap tm = target as TileMap;
                objToPlacePreview.transform.SetParent(tm.transform);

            }
            GUI.TextArea(new Rect(count * textureIconSize + position.x, row * textureIconSize + position.y + boxPadding, textureIconSize, textHeight), gameObjs[i].name);

            count++;
            if (count >= numTilesInRow)
            {
                count = 0;
                row += 1;
            }
        }

    }

    public Vector2 CreateResizableTextureBox(Vector2 position, Texture[] textures)
    {

     //   Vector2 guiAbove = GUILayoutUtility.GetLastRect().position;
        // Width of current window. Gives width of inspector.
       
        int textureIconSize = 64;
        int textHeight = 15;
        int boxWidth = Screen.width;
        int numTextures = textures.Length;
        int numTilesInRow = boxWidth / textureIconSize;
        
        int numRows = (numTextures + numTextures - 1)/ numTilesInRow; // Rounded up integer division
        int boxHeight = numRows * textureIconSize;
        int boxPadding = 20;

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.padding = new RectOffset(0, 0, 10, 10);

        GUIStyle g = new GUIStyle();
        g.padding = new RectOffset(5, 5, 10, 0);
        GUI.Box(new Rect(position.x, position.y + boxPadding, boxWidth - position.x * 2, boxHeight),"");

        int count = 0;                          // Decides index in the row
        int row = 0;                            // Decides which row
        for (int i = 0; i < numTextures; i++)
        {
            if (GUI.Button(new Rect(count * textureIconSize + position.x, row * textureIconSize + position.y  + boxPadding, textureIconSize, textureIconSize), textures[i], g))
            {
                paintTexture = textures[i];
            }
            GUI.TextArea(new Rect(count * textureIconSize + position.x, row * textureIconSize + position.y + boxPadding, textureIconSize, textHeight), textures[i].name);

            count++;
            if (count >= numTilesInRow)
            {
                count = 0;
                row += 1;
            }
        }

        return position + new Vector2(0, boxHeight);
    }

    public void GameObjectInteractionHandler(Event e, GameObject obj)
    {
        Debug.Log("Clicked on " + obj.name);
        if (state == EditorState.TEXTURE)
        {
            ChangeTexture(obj.GetComponent<Renderer>(), paintTexture);
        }
        else if (state == EditorState.PAINT)
        {
            if (e.button == 0)
            {
                if (e.control)
                {
                    paintColor = obj.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(obj.GetComponent<Renderer>(), paintColor);
            }
            else if (e.button == 1)
            {
                if (e.control)
                {
                    paintSecondaryColor = obj.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(obj.GetComponent<Renderer>(), paintSecondaryColor);
            }
        }
        else if (state == EditorState.RAISE_LOWER)
        {
            Tile tile = obj.GetComponentInParent<Tile>();
            if (tile == null)
                return;

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
        else if (state == EditorState.ROTATE)
        {
            obj.transform.Rotate(obj.transform.up, rotateBy);
        }
        else if (state == EditorState.PLACE_OBJECT)
        {
            Tile tile = obj.GetComponentInParent<Tile>();
            if (tile == null)
                return;
            if (e.button == 0)
                tile.PlaceObject(objToPlace, visitableOnPlace);
            else if (e.button == 1)
                tile.RemoveObject();
        }
    }

    private void TileInteractionHandler(Event e, Tile tile)
    {
        if (state == EditorState.ADD_REMOVE)
        {
            if (e.button == 0)
            {
                ChangeColor(tile.GetComponent<Renderer>(), removeColor);
                _toRemove.Add(tile);
                Debug.Log(_toRemove.Count);
            }
            else if (e.button == 1)
            {
                ChangeColor(tile.GetComponent<Renderer>(), keepColor);
                _toRemove.Remove(tile);
                Debug.Log(_toRemove.Count);
            }


        }
        else if (state == EditorState.PAINT)
        {
            if (e.button == 0)
            {
                if (e.control)
                {
                    paintColor = tile.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(tile.GetComponent<Renderer>(), paintColor);
            } else if (e.button == 1)
            {
                if (e.control)
                {
                    paintSecondaryColor = tile.GetComponent<Renderer>().sharedMaterial.color;
                }
                ChangeColor(tile.GetComponent<Renderer>(), paintSecondaryColor);
            }
            
            
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
                paintTexture = tile.GetComponent<Renderer>().sharedMaterial.mainTexture;
            } else
            {
                ChangeTexture(tile.GetComponent<Renderer>(), paintTexture);

            }
        } else if (state == EditorState.ROTATE)
        {
            tile.transform.Rotate(tile.transform.up, rotateBy);
        } else if (state == EditorState.PLACE_OBJECT)
        {
            if (e.button == 0)
                tile.PlaceObject(objToPlace, visitableOnPlace);
            else if (e.button == 1)
                tile.RemoveObject();
        }
    }

    private GameObject[] FindAllPrefabs()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (string guid in AssetDatabase.FindAssets("t:prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            objs.Add((GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));
            Debug.Log(path);
            
        }
        return objs.ToArray();
    }

    private Texture[] FindAllTextures()
    {
        List<string> paths = new List<string>();

        foreach (string guid in AssetDatabase.FindAssets("t:Texture"))
        {
            string tex = AssetDatabase.GUIDToAssetPath(guid);
            if (tex.Contains(".png") || tex.Contains(".jpg") || tex.Contains(".jpeg"))
                paths.Add(tex);
        }

        Texture[] textures = new Texture[paths.Count];
        for (int i = 0; i < paths.Count; i++)
        {
            textures[i] = AssetDatabase.LoadAssetAtPath(paths[i], typeof(Texture)) as Texture;
        }

        return textures;
    }


    private void PlaceObject(Tile tile, GameObject toPlace)
    {
        float topOfTile = tile.transform.position.y + tile.transform.localScale.y / 2 + toPlace.transform.localScale.y / 2;
        Vector3 pos = new Vector3(tile.transform.position.x, topOfTile, tile.transform.position.z);

        GameObject created = Instantiate(toPlace, pos, Quaternion.identity) as GameObject;
        created.transform.SetParent(tile.transform);
    }

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
    /// <summary>
    /// Creates a new material to avoid leaking memory when changing shared material in editor.
    /// Changing shared material changes all objects with the material on it. This is not desired for this case.
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="texture"></param>
    private void ChangeTexture(Renderer renderer, Texture texture)
    {
        var tempMaterial = new Material(renderer.sharedMaterial);
        tempMaterial.mainTexture = texture;
        renderer.sharedMaterial = tempMaterial;
    }

    private void ChangeColor(Renderer renderer, Color color)
    {
        var tempMaterial = new Material(renderer.sharedMaterial);
        tempMaterial.color = color;
        renderer.sharedMaterial = tempMaterial;
    }

    private Tile[] HashSetToArray(HashSet<Tile> set)
    {
        Tile[] arr = new Tile[set.Count];

        var enumerator = set.GetEnumerator();

        int i = 0;
        while (enumerator.MoveNext())
        {           
            arr[i] = enumerator.Current;
            i++;
        }
        return arr;
    }


}
