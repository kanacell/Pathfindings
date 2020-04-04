using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using System.Reflection;

[CustomEditor(typeof(GameGrid))]
public class GameGrid_Editor : Editor
{
    #region Public Methods
    public override void OnInspectorGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Create Grid"))
            {
                m_GameGrid.Invoke("InitGrid", m_GridDimension.vector2IntValue, m_TilePrefab.objectReferenceValue);
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clean Grid"))
            {
                m_GameGrid.Invoke("CleanGrid");
            }
        }

        base.OnInspectorGUI();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnEnable()
    {
        m_GameGrid = target as GameGrid;
        m_GridDimension = serializedObject.FindProperty("m_Dimensions");
        m_TilePrefab = serializedObject.FindProperty("m_PrefabTile");
    }
    #endregion

    #region Getters/Setters
    #endregion

    #region Public Attributes
    #endregion

    #region Protected Attributes
    #endregion

    #region Private Attributes
    private GameGrid m_GameGrid = null;
    private SerializedProperty m_GridDimension = null;
    private SerializedProperty m_TilePrefab = null;
    #endregion
}
