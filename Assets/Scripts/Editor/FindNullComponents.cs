using UnityEditor;
using UnityEngine;

public class FindNullComponents : EditorWindow
{
    [MenuItem("Tools/Find Null Components in Prefabs")]
    static void Init()
    {
        var window = GetWindow<FindNullComponents>();
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Scan Prefabs"))
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                var components = prefab.GetComponentsInChildren<Component>(true);
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.LogError($"Null component found in prefab: {path}", prefab);
                    }
                }
            }
        }
    }
}