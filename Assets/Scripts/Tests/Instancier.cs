using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Instancier : MonoBehaviour
{
    #region Private Methods
    [ContextMenu("Create Grids")]
    private void CreateGrids()
    {
        System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        m_GridA = new GameTile[m_Rows * m_Columns];

        int indexTile = -1;
        int indexNeighbor = -1;

        /**
        chrono.Start();
        for (int i = 0; i < m_Rows; i++)
        {
            for (int j = 0; j < m_Columns; j++)
            {
                indexTile = GetIndex(i, j);
                m_GridA[indexTile] = Instantiate<GameTile>(m_PrefabTile, m_AnchorGridA);
                m_GridA[indexTile].name = $"Tile [{i}; {j}]";
                m_GridA[indexTile].InitWeight(1);
                m_GridA[indexTile].InitGridPosition(i, j);
                m_GridA[indexTile].transform.localPosition = new Vector3(j, 0, i);

                if (i > 0)
                {
                    indexNeighbor = GetIndex(i - 1, j);
                    m_GridA[indexTile].AddNeighbor(m_GridA[indexNeighbor]);
                    m_GridA[indexNeighbor].AddNeighbor(m_GridA[indexTile]);
                }

                if (j > 0)
                {
                    indexNeighbor = GetIndex(i, j - 1);
                    m_GridA[indexTile].AddNeighbor(m_GridA[indexNeighbor]);
                    m_GridA[indexNeighbor].AddNeighbor(m_GridA[indexTile]);
                }
            }
        }
        chrono.Stop();
        /**/
        var elapsedTime1 = chrono.ElapsedMilliseconds;

        chrono.Restart();
        m_GridB = new GameTile[m_Rows * m_Columns];
        for (int i = 0; i < m_Rows; i++)
        {
            for (int j = 0; j < m_Columns; j++)
            {
                indexTile = GetIndex(i, j);
                m_GridB[indexTile] = Instantiate<GameTile>(m_PrefabTile, m_AnchorGridB);
                m_GridB[indexTile].name = $"Tile [{i}; {j}]";
                m_GridB[indexTile].CustomInvoke("InitWeight", 1);
                m_GridB[indexTile].CustomInvoke("InitGridPosition", i, j);
                m_GridB[indexTile].transform.localPosition = new Vector3(j, 0, i);

                if (i > 0)
                {
                    indexNeighbor = GetIndex(i - 1, j);
                    m_GridB[indexTile].AddNeighbor(m_GridB[indexNeighbor]);
                    m_GridB[indexNeighbor].AddNeighbor(m_GridB[indexTile]);
                }

                if (j > 0)
                {
                    indexNeighbor = GetIndex(i, j - 1);
                    m_GridB[indexTile].AddNeighbor(m_GridB[indexNeighbor]);
                    m_GridB[indexNeighbor].AddNeighbor(m_GridB[indexTile]);
                }
            }
        }
        chrono.Stop();

        var elapsedTime2 = chrono.ElapsedMilliseconds;

        string message = $"{m_Rows} x {m_Columns}\n";
        message += $"direct call : {elapsedTime1} ms\n";
        message += $"custom invoke : {elapsedTime2} ms";
        Debug.Log(message);
    }

    [ContextMenu("Clean Grids")]
    private void CleanGrids()
    {
        for (int i = 0; i < m_GridA.Length; i++)
        {
            DestroyImmediate(m_GridA[i].gameObject);
        }
        m_GridA = null;

        for (int i = 0; i < m_GridB.Length; i++)
        {
            DestroyImmediate(m_GridB[i].gameObject);
        }
        m_GridB = null;
    }

    [ContextMenu("clear cache")]
    private void ClearCache()
    {
        this.ClearStaticCache();
    }

    private int GetIndex(int _Row, int _Column)
    {
        if (_Row < 0 || _Row >= m_Rows || _Column < 0 || _Column >= m_Columns)
            return -1;
        return _Row * m_Columns + _Column;
    }
    #endregion

    #region Private Attributes
    public GameTile m_PrefabTile = null;
    public int m_Rows = 0;
    public int m_Columns = 0;
    public GameTile[] m_GridA = null;
    public GameTile[] m_GridB = null;
    public Transform m_AnchorGridA = null;
    public Transform m_AnchorGridB = null;
    [SerializeField] private string m_string = string.Empty;
    #endregion
}
