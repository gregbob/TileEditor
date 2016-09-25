using UnityEngine;

using System.Collections.Generic;
using UnityEditor;

public class FindAssets {

    public static Object[] FindAllAssets(string type)
    {
        List<Object> objs = new List<Object>();
        foreach (string guid in AssetDatabase.FindAssets("t:" + type))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            objs.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        return objs.ToArray();
    }

    public static Object[] FindAllAssets(string type, string location)
    {
        List<Object> objs = new List<Object>();
        foreach (string guid in AssetDatabase.FindAssets("t:" + type))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            objs.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        return objs.ToArray();
    }

    private static GameObject[] FindAllPrefabs()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (string guid in AssetDatabase.FindAssets("t:prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            objs.Add((GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));

        }
        return objs.ToArray();
    }

    private static Texture[] FindAllTextures()
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

    public static Texture GetMaterialIcon()
    {
        List<Object> objs = new List<Object>();
        foreach (string guid in AssetDatabase.FindAssets("t:material"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            objs.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        return AssetPreview.GetAssetPreview(objs[0]);
    }


}
