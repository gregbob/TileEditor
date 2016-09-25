using UnityEngine;
using System.Collections;
using UnityEditor;

namespace gregbob {

    public class EditorUtility 
    {
        /// <summary>
        /// Creates a new material to avoid leaking memory when changing shared material in editor.
        /// Changing shared material changes all objects with the material on it. This is not desired for this case.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="texture"></param>
        public static void ChangeTexture(Renderer renderer, Texture texture)
        {
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.mainTexture = texture;
            renderer.sharedMaterial = tempMaterial;
        }

        public static void ChangeColor(Renderer renderer, Color color)
        {
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.color = color;
            renderer.sharedMaterial = tempMaterial;
        }

        public static void CreateSectionLabel(string name)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUIStyle sectionLabelStyle = new GUIStyle();
            sectionLabelStyle.fontSize = 15;
            sectionLabelStyle.alignment = TextAnchor.UpperCenter;
            sectionLabelStyle.border = new RectOffset(10, 10, 10, 10);
            sectionLabelStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(name, sectionLabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}

