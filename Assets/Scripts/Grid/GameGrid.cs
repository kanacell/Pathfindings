using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    #region Public Methods
    public GameTile GetNodeAtLocation(int _Row, int _Column)
    {
        if (m_Nodes == null || _Row < 0 || _Row >= m_Dimensions.y || _Column < 0 || _Column >= m_Dimensions.x)
            return null;

        return m_Nodes[_Row, _Column];
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void Start()
    {
    }

    private void InitGrid(Vector2Int _Dimensions, GameTile _PrefabTile)
    {
        if (!_PrefabTile)
            return;

        // Destroy all current nodes
        CleanGrid();

        // Re-assign dimensions
        m_Dimensions = _Dimensions;

        // Instantiate new nodes
        m_Nodes = new GameTile[m_Dimensions.y, m_Dimensions.x];
        for (int i = 0; i < _Dimensions.y; i++)
        {
            for (int j = 0; j < _Dimensions.x; j++)
            {
                Vector3 localPos = new Vector3(j, 0, i);
                // Instantiate new GameTile
                m_Nodes[i, j] = UnityEditor.PrefabUtility.InstantiatePrefab(_PrefabTile, transform) as GameTile;
                m_Nodes[i, j].transform.localPosition = localPos;
                m_Nodes[i, j].name = $@"Tile [{i}; {j}]";

                // Update neighbors links
                if (i > 0)
                {
                    m_Nodes[i - 1, j].AddNeighbor(m_Nodes[i, j]);
                    m_Nodes[i, j].AddNeighbor(m_Nodes[i - 1, j]);
                }
                if (j > 0)
                {
                    m_Nodes[i, j - 1].AddNeighbor(m_Nodes[i, j]);
                    m_Nodes[i, j].AddNeighbor(m_Nodes[i, j - 1]);
                }
            }
        }
    }

    private void CleanGrid()
    {
        if (m_Nodes == null)
            return;

        for (int i = 0; i < m_Dimensions.y; i++)
        {
            for (int j = 0; j < m_Dimensions.x; j++)
            {
                if (m_Nodes[i, j])
                {
                    DestroyImmediate(m_Nodes[i, j].gameObject);
                    m_Nodes[i, j] = null;
                }
            }
        }
        m_Nodes = null;
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

    #region Public Attributes
    #endregion

    #region Protected Attributes
    #endregion

    #region Private Attributes
    [SerializeField] private Vector2Int m_Dimensions = Vector2Int.zero;
    [SerializeField] private GameTile m_PrefabTile = null;
    private GameTile[,] m_Nodes = null;
    #endregion
}
