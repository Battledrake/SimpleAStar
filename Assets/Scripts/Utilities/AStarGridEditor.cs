using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AStarGrid))]
public class AStarGridEditor : Editor
{
    SerializedProperty _graphCreationType;
    SerializedProperty _width;
    SerializedProperty _height;
    SerializedProperty _textureMap;
    SerializedProperty _cellSize;
    SerializedProperty _connectionType;

    private void OnEnable()
    {
        _graphCreationType = serializedObject.FindProperty("_graphCreationType");
        _width = serializedObject.FindProperty("_width");
        _height = serializedObject.FindProperty("_height");
        _textureMap = serializedObject.FindProperty("_textureMap");
        _cellSize = serializedObject.FindProperty("_cellSize");
        _connectionType = serializedObject.FindProperty("_connectionType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_graphCreationType);

        if (_graphCreationType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(_width);
            EditorGUILayout.PropertyField(_height);
            EditorGUILayout.PropertyField(_cellSize);
            EditorGUILayout.PropertyField(_connectionType);
        }
        else if(_graphCreationType.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(_textureMap);
            EditorGUILayout.PropertyField(_cellSize);
            EditorGUILayout.PropertyField(_connectionType);
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}