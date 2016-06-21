#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SwapPrefabEditor : EditorWindow
{
    private GameObject group;
    private GameObject targetGameObject;
    private GameObject targetPrefab;

    [MenuItem("Demonixis/Prefab Swapper")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SwapPrefabEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Swap", EditorStyles.boldLabel);

        group = EditorGUILayout.ObjectField("Group", group, typeof(GameObject), true) as GameObject;
        targetGameObject = EditorGUILayout.ObjectField("Target GameObject", targetGameObject, typeof(GameObject), true) as GameObject;
        targetPrefab = EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Apply") && (group != null || targetGameObject != null) && targetPrefab != null)
        {
            if (group != null)
            {
                foreach (Transform tr in group.transform)
                    SwapObject(tr.gameObject, targetPrefab);
            }
            else
                SwapObject(targetGameObject, targetPrefab);
        }
    }

    private void SwapObject(GameObject current, GameObject prefab)
    {
        var position = current.transform.position;
        var rotation = current.transform.rotation;
        var scale = current.transform.localScale;
        var name = current.name;
        var tag = current.tag;
        var parent = current.transform.parent;

        DestroyImmediate(current);

        var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        go.transform.parent = parent;
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.localScale = scale;
        go.name = name;
        go.tag = tag;
    }
}
#endif