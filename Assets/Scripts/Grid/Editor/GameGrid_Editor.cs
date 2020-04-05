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
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_GenerationMode);

        bool displayGenerationButtons = true;
        switch ((GameGrid.EGenerationMode)m_GenerationMode.intValue)
        {
            case GameGrid.EGenerationMode.GM_File:
                EditorGUILayout.PropertyField(m_Filename);
                break;
            case GameGrid.EGenerationMode.GM_Uniforme:
                EditorGUILayout.PropertyField(m_GridDimension);
                EditorGUILayout.PropertyField(m_TilePrefab);
                break;
            default:
                displayGenerationButtons = false;
                break;
        }

        if (displayGenerationButtons)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create Grid"))
                {
                    switch ((GameGrid.EGenerationMode)m_GenerationMode.intValue)
                    {
                        case GameGrid.EGenerationMode.GM_File:
                            m_GameGrid.CustomInvoke("InitGrid", m_Filename.stringValue);
                            break;
                        case GameGrid.EGenerationMode.GM_Uniforme:
                            m_GameGrid.CustomInvoke("InitGrid", m_GridDimension.vector2IntValue, m_TilePrefab.objectReferenceValue);
                            break;
                        default:
                            break;
                    }
                }
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clean Grid"))
                {
                    m_GameGrid.CustomInvoke("CleanGrid");
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnEnable()
    {
        m_GameGrid = target as GameGrid;
        m_GenerationMode = serializedObject.FindProperty("m_GenerationMode");
        m_GridDimension = serializedObject.FindProperty("m_EditorDimensions");
        m_TilePrefab = serializedObject.FindProperty("m_PrefabTile");
        m_Filename = serializedObject.FindProperty("m_Filename");
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
    private SerializedProperty m_GenerationMode = null;
    private SerializedProperty m_GridDimension = null;
    private SerializedProperty m_TilePrefab = null;
    private SerializedProperty m_Filename = null;
    #endregion
}
