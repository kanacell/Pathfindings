using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GameGrid
{
    #region Public Methods
    public GameGrid(string _FileName, GridRenderer _Renderer)
    {
        string[] allLines = File.ReadAllLines(_FileName);
        m_TilesRows = int.Parse(allLines[0]);
        m_TilesColumns = int.Parse(allLines[1]);
        m_Tiles = new Tile[m_TilesRows * m_TilesColumns];
        _Renderer.InitRenderer(m_TilesRows, m_TilesColumns);

        for (int i = 0; i < m_TilesRows; i++)
        {
            for (int j = 0; j < m_TilesColumns; j++)
            {
                int weight = (int)char.GetNumericValue(allLines[i + 2][j]);
                int indexTile = GetTileIndexOf(i, j);
                m_Tiles[indexTile] = new Tile(i, j, weight);
                _Renderer.SetTexturePixel(j, m_TilesRows - i - 1, weight <= 0 ? Color.black : Color.white);

                int indexNeighbor = GetTileIndexOf(i - 1, j);
                if (indexNeighbor >= 0)
                {
                    m_Tiles[indexTile].AddNeighbor(m_Tiles[indexNeighbor]);
                    m_Tiles[indexNeighbor].AddNeighbor(m_Tiles[indexTile]);
                }

                indexNeighbor = GetTileIndexOf(i, j - 1);
                if (indexNeighbor >= 0)
                {
                    m_Tiles[indexTile].AddNeighbor(m_Tiles[indexNeighbor]);
                    m_Tiles[indexNeighbor].AddNeighbor(m_Tiles[indexTile]);
                }
            }
        }
        _Renderer.ApplyRenderer();
    }

    public Tile GetTileAt(int _Row, int _Column)
    {
        int indexTile = GetTileIndexOf(_Row, _Column);
        if (indexTile == -1)
            return null;
        return m_Tiles[indexTile];
    }
    #endregion

    #region Private Methods
    private int GetTileIndexOf(int _Row, int _Column)
    {
        if (_Row < 0 || _Row >= m_TilesRows || _Column < 0 || _Column >= m_TilesColumns)
            return -1;
        return _Row * m_TilesColumns + _Column;
    }
    #endregion

    #region Getters/Setters
    public int TilesRows
    {
        get
        {
            return m_TilesRows;
        }
    }

    public int TilesColumns
    {
        get
        {
            return m_TilesColumns;
        }
    }
    #endregion

    #region Private Attributes
    private int m_TilesRows = 0;
    private int m_TilesColumns = 0;
    private Tile[] m_Tiles = null;
    #endregion
}
