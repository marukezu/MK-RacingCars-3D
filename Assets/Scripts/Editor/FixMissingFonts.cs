using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class FixMissingFonts : EditorWindow
{
    private Font newFont;
    private TMP_FontAsset newTMPFont;

    [MenuItem("Tools/Fonts/Replace Missing Fonts")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FixMissingFonts));
    }

    void OnGUI()
    {
        GUILayout.Label("Corrigir fontes faltando", EditorStyles.boldLabel);
        newFont = (Font)EditorGUILayout.ObjectField("Nova fonte (UI Text)", newFont, typeof(Font), false);
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Nova fonte (TMP)", newTMPFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Aplicar em todos os Prefabs"))
        {
            Replace();
        }
    }

    void Replace()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            bool changed = false;

            // Text (legacy)
            foreach (Text text in prefab.GetComponentsInChildren<Text>(true))
            {
                if (text.font == null && newFont != null)
                {
                    text.font = newFont;
                    changed = true;
                }
            }

            // TextMeshPro
            foreach (TMP_Text tmp in prefab.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmp.font == null && newTMPFont != null)
                {
                    tmp.font = newTMPFont;
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(prefab);
                Debug.Log("Corrigido prefab: " + path);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}