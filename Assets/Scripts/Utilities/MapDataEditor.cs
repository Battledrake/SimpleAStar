using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavigationArea))]
public class MapDataEditor : Editor
{
    SerializedProperty _graphCreationType;
    SerializedProperty _width;
    SerializedProperty _height;
    SerializedProperty _textureMap;

    private void OnEnable()
    {
        _graphCreationType = serializedObject.FindProperty("_graphCreationType");
        _width = serializedObject.FindProperty("_width");
        _height = serializedObject.FindProperty("_height");
        _textureMap = serializedObject.FindProperty("_textureMap");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_graphCreationType);

        if (_graphCreationType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(_width);
            EditorGUILayout.PropertyField(_height);
        }
        else
        {
            EditorGUILayout.PropertyField(_textureMap);
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}