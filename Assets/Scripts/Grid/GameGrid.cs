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
    public GameTile GetTileAt(int _Row, int _Column)
    {
        /**
        if (m_Tiles == null || _Row < 0 || _Row >= m_Dimensions.y || _Column < 0 || _Column >= m_Dimensions.x)
            return null;

        return m_Tiles[_Row, _Column];
        /**/

        if (m_Tiles == null)
            return null;
        int indexTile = GetIndex(_Row, _Column);
        if (indexTile == -1)
            return null;

        return m_Tiles[indexTile];
    }
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
        m_Tiles = new GameTile[m_Dimensions.y * m_Dimensions.x];
        for (int i = 0; i < _Dimensions.y; i++)
        {
            for (int j = 0; j < _Dimensions.x; j++)
            {
                int indexTile = GetIndex(i, j);
                m_Tiles[indexTile] = CreateTile(_PrefabTile, i, j);
            }
        }

        // Setup the camera to the middle map
        Camera.main.orthographicSize = Mathf.Max(m_Dimensions.x, m_Dimensions.y) / 2.0f;
        Camera.main.transform.position = new Vector3(m_Dimensions.x / 2.0f - 0.5f, 5, m_Dimensions.y / 2.0f - 0.5f);
    }

    [ContextMenu("Init grid from file")]
    private void InitGridFromFile()
    {
        InitGrid(m_Filename);
    }

    public void InitGrid(string _Filename)
    {
        if (!m_PrefabTile)
            return;

        // Check if the filename is valid
        string pathFile = $@"{Application.persistentDataPath}/Maps/{_Filename}";

        string[] lines = File.ReadLines(pathFile).ToArray();
        int rows = int.Parse(lines[0]);
        int columns = int.Parse(lines[1]);

        /* vvvvv Generation 1 vvvvv */
        /**
        if (rows <= 0 || columns <= 0)
            return;

        // Destroy all current tiles
        CleanGrid();

        // Re-assign dimensions
        m_Dimensions = new Vector2Int(columns, rows);

        // Instantiate new tiles
        m_Tiles = new GameTile[m_Dimensions.y * m_Dimensions.x];
        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                int weight = lines[i + 2][j] - '0';
                int indexTile = GetIndex(i, j);
                m_Tiles[indexTile] = CreateTile(m_PrefabTile, i, j, weight);
            }
        }
        /**/
        /* ^^^^^ Generation 1 ^^^^^ */

        /* ===== OU BIEN ===== */

        InitGrid(new Vector2Int(columns, rows), m_PrefabTile);
        for(int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int weight = (int)char.GetNumericValue(lines[i + 2][j]);
                int indexTile = GetIndex(i, j);
                m_Tiles[indexTile].CustomInvoke("InitWeight", weight);
                //m_Tiles[indexTile].InitWeight(weight);
            }
        }


    }

    /// <summary>
    /// Destroy all tiles
    /// </summary>
    [ContextMenu("clean grid")]
    private void CleanGrid()
    {
        if (m_Tiles == null)
            return;

        for (int i = 0; i < m_Tiles.Length; i++)
        {
            if (m_Tiles[i])
            {
                DestroyImmediate(m_Tiles[i].gameObject);
            }
        }
        m_Tiles = null;
    }

    [ContextMenu("Clean static methods")]
    private void CleanMethods()
    {
        this.ClearStaticCache();
    }

    /// <summary>
    /// Remove null references from neighbors in each tile
    /// </summary>
    private void FixNeighbors()
    {
        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                //m_Tiles[i, j].CustomInvoke("FixNullNeighbors");
            }
        }
    }

    private GameTile CreateTile(GameTile _Prefab, int _Row, int _Column)
    {
        GameTile tile = Instantiate<GameTile>(_Prefab, m_GridAnchor);
        tile.transform.localPosition = new Vector3(_Column, 0, m_Dimensions.y - _Row - 1);
        tile.name = $"Tile [{_Row}; {_Column}]";
        tile.CustomInvoke("InitGridPosition", _Row, _Column);

        /**/
        int indexTile = -1;
        // Update neighbors links
        if (_Row > 0)
        {
            indexTile = GetIndex(_Row - 1, _Column);
            tile.AddNeighbor(m_Tiles[indexTile]);
            m_Tiles[indexTile].AddNeighbor(tile);

        }
        if (_Column > 0)
        {
            indexTile = GetIndex(_Row, _Column - 1);
            tile.AddNeighbor(m_Tiles[indexTile]);
            m_Tiles[indexTile].AddNeighbor(tile);
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

    private int GetIndex(int _Row, int _Column)
    {
        if (_Row < 0 || _Row >= m_Dimensions.y || _Column < 0 || _Column >= m_Dimensions.x)
            return -1;
        return _Row * m_Dimensions.x + _Column;
    }
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

    #region Private Attributes
    [Header("Grid Generation Settings")]
    [SerializeField] private EGenerationMode m_GenerationMode = EGenerationMode.GM_None;
    [SerializeField] private Transform m_GridAnchor = null;

    [Header("Uniform Settings")]
    [SerializeField] private Vector2Int m_EditorDimensions = Vector2Int.zero;
    [SerializeField] private GameTile m_PrefabTile = null;

    [Header("File Settings")]
    [SerializeField] private string m_Filename = string.Empty;

    [SerializeField, HideInInspector] private Vector2Int m_Dimensions = Vector2Int.zero;
    [SerializeField, HideInInspector] private GameTile[] m_Tiles = null;

    //[SerializeField] private 
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
