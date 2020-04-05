using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameGrid : MonoBehaviour
{
    #region Public Methods
    public GameTile GetNodeAtLocation(int _Row, int _Column)
    {
        if (m_Tiles == null || _Row < 0 || _Row >= m_Dimensions.y || _Column < 0 || _Column >= m_Dimensions.x)
            return null;

        return m_Tiles[_Row, _Column];
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods


#if UNITY_EDITOR
    private void InitGrid(Vector2Int _Dimensions, GameTile _PrefabTile)
    {
        if (!_PrefabTile || _Dimensions.x <= 0 || _Dimensions.y <= 0)
            return;

        // Destroy all current tiles
        CleanGrid();

        // Re-assign dimensions
        m_Dimensions = _Dimensions;

        // Instantiate new tiles
        m_Tiles = new GameTile[m_Dimensions.y, m_Dimensions.x];
        for (int i = 0; i < _Dimensions.y; i++)
        {
            for (int j = 0; j < _Dimensions.x; j++)
            {
                m_Tiles[i, j] = CreateTile(_PrefabTile, i, j);

                /**
                // Update neighbors links
                if (i > 0)
                {
                    m_Tiles[i - 1, j].AddNeighbor(m_Tiles[i, j]);
                    m_Tiles[i, j].AddNeighbor(m_Tiles[i - 1, j]);
                }
                if (j > 0)
                {
                    m_Tiles[i, j - 1].AddNeighbor(m_Tiles[i, j]);
                    m_Tiles[i, j].AddNeighbor(m_Tiles[i, j - 1]);
                }
                /**/
            }
        }
    }

    private void InitGrid()
    {
        if (!m_PrefabTile || m_EditorDimensions.x <= 0 || m_EditorDimensions.y <= 0)
            return;

        // Destroy all current tiles
        CleanGrid();

        // Re-assign dimensions
        m_Dimensions = m_EditorDimensions;

        // Instantiate new tiles
        m_Tiles = new GameTile[m_Dimensions.y, m_Dimensions.x];
        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                m_Tiles[i, j] = CreateTile(m_PrefabTile, i, j);
            }
        }
    }

    private void InitGrid(string _Filename)
    {
        if (!m_PrefabTile)
            return;

        // Check if the filename is valid
        string pathFile = $@"{Application.persistentDataPath}/Maps/{_Filename}";

        string[] lines = File.ReadLines(pathFile).ToArray();
        int rows = int.Parse(lines[0]);
        int columns = int.Parse(lines[1]);

        if (rows <= 0 || columns <= 0)
            return;

        // Destroy all current tiles
        CleanGrid();

        // Re-assign dimensions
        m_Dimensions = new Vector2Int(columns, rows);

        // Instantiate new tiles
        m_Tiles = new GameTile[m_Dimensions.y, m_Dimensions.x];
        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                int weight = lines[i + 2][j] - '0';
                m_Tiles[i, j] = CreateTile(m_PrefabTile, i, j, weight);
            }
        }
    }

    /// <summary>
    /// Destroy all tiles
    /// </summary>
    private void CleanGrid()
    {
        if (m_Tiles == null)
            return;

        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                if (m_Tiles[i, j])
                {
                    DestroyImmediate(m_Tiles[i, j].gameObject);
                    m_Tiles[i, j] = null;
                }
            }
        }
        m_Tiles = null;
    }

    /// <summary>
    /// Remove null references from neighbors in each tile
    /// </summary>
    private void FixNullTiles()
    {
        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                m_Tiles[i, j].CustomInvoke("FixNullNeighbors");
            }
        }
    }

    private GameTile CreateTile(GameTile _Prefab, int _Row, int _Column)
    {
        GameTile tile = PrefabUtility.InstantiatePrefab(_Prefab, transform) as GameTile;
        tile.transform.localPosition = new Vector3(_Column, 0, m_Dimensions.y - _Row - 1);
        tile.name = $@"Tile [{_Row}; {_Column}]";

        /**/
        // Update neighbors links
        if (_Row > 0)
        {
            tile.AddNeighbor(m_Tiles[_Row - 1, _Column]);
            m_Tiles[_Row - 1, _Column].AddNeighbor(tile);
        }
        if (_Column > 0)
        {
            tile.AddNeighbor(m_Tiles[_Row, _Column - 1]);
            m_Tiles[_Row, _Column - 1].AddNeighbor(tile);
        }
        /**/

        return tile;
    }

    private GameTile CreateTile(GameTile _Prefab, int _Row, int _Column, int _Weight)
    {
        GameTile tile = CreateTile(_Prefab, _Row, _Column);
        tile.CustomInvoke("InitWeight", _Weight);
        return tile;
    }

#endif
    #endregion

    #region Getters/Setters
    public Vector2Int GetDimensions
    {
        get
        {
            return m_Dimensions;
        }
    }
    #endregion

    #region Public Attributes
    #endregion

    #region Protected Attributes
    #endregion

    #region Private Attributes
    [Header("Grid Generation Settings")]
    [SerializeField] private EGenerationMode m_GenerationMode = EGenerationMode.GM_None;

    [Header("Uniform Settings")]
    [SerializeField] private Vector2Int m_EditorDimensions = Vector2Int.zero;
    [SerializeField] private GameTile m_PrefabTile = null;

    [Header("File Settings")]
    [SerializeField] private string m_Filename = string.Empty;

    private Vector2Int m_Dimensions = Vector2Int.zero;
    private GameTile[,] m_Tiles = null;
    #endregion

    #region Enumerations
    public enum EGenerationMode
    {
        GM_None,
        GM_File,
        GM_Uniforme
    }
    #endregion
}
