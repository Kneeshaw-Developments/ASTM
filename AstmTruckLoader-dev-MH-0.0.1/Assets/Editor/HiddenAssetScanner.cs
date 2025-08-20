using UnityEngine;
using UnityEditor;

public static class HiddenAssetScanner
{
    [MenuItem("Tools/Scan For Hidden Assets")]
    public static void Scan()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material t:Shader t:Texture");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj != null && obj.name.Contains("Hidden"))
            {
                Debug.LogError($"⚠ Hidden asset found: {obj.name} at {path}", obj);
            }
        }
    }
}
