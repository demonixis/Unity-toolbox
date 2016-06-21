#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TransformChangerEditor : EditorWindow
{
    private GameObject _group;
    private float _ratio;

    [MenuItem("Demonixis/Transform Changer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TransformChangerEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Swap", EditorStyles.boldLabel);

        _group = EditorGUILayout.ObjectField("Group", _group, typeof(GameObject), true) as GameObject;
        _ratio = EditorGUILayout.FloatField("Ratio", _ratio);

        if (GUILayout.Button("Apply"))
        {
            var groupTransform = _group.transform;
            foreach (Transform tr in groupTransform)
                tr.position = new Vector3(tr.position.x * _ratio, tr.position.y * _ratio, tr.position.z * _ratio);
        }
    }
}
#endif