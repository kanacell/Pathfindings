using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
	#region Public Methods
	public Tile GetTileAt(int _Row, int _Column)
	{
		int index = GetIndexOf(_Row, _Column);
		if (index == -1)
			return null;

		return m_Tiles[index];
	}
	#endregion

	#region Private Methods
	private void Start()
	{
		CreateGrid();
	}

	[ContextMenu("Create Grid")]
	private void CreateGrid()
	{
		CleanGrid();

		string pathFile = $@"{Application.persistentDataPath}/Maps/{m_FileName}";
		string[] allLines = File.ReadAllLines(pathFile);
		m_Rows = int.Parse(allLines[0]);
		m_Columns = int.Parse(allLines[1]);

		m_Tiles = new Tile[m_Rows * m_Columns];
		m_GridTexture = new Texture2D(m_Columns, m_Rows);
		m_GridTexture.filterMode = FilterMode.Point;

		int weight = 0;
		for (int i = 0; i < m_Rows; i++)
		{
			for (int j = 0; j < m_Columns; j++)
			{
				weight = (int)char.GetNumericValue(allLines[i + 2][j]);
				m_GridTexture.SetPixel(j, m_Rows - i - 1, weight <= 0 ? Color.black : Color.white);
				int indexTile = GetIndexOf(i, j);
				m_Tiles[indexTile] = new Tile(i, j, weight);

				if (i > 0)
				{
					int indexOther = GetIndexOf(i - 1, j);
					m_Tiles[indexTile].AddNeighbor(m_Tiles[indexOther]);
					m_Tiles[indexOther].AddNeighbor(m_Tiles[indexTile]);
				}

				if (j > 0)
				{
					int indexOther = GetIndexOf(i, j - 1);
					m_Tiles[indexTile].AddNeighbor(m_Tiles[indexOther]);
					m_Tiles[indexOther].AddNeighbor(m_Tiles[indexTile]);
				}
			}
		}

		m_GridTexture.Apply();
		m_GridRenderer.transform.localScale = new Vector3Int(m_Columns, 1, m_Rows);
		m_GridRenderer.ApplyTexture(m_GridTexture);

		Camera.main.transform.position = new Vector3(m_Columns / 2f, 10, m_Rows / 2f);
		Camera.main.orthographicSize = Mathf.Max(m_Rows, m_Columns) / 2f + 0.5f;
		m_GridRenderer.transform.position = new Vector3(m_Columns / 2f, 0, m_Rows / 2f);
	}

	[ContextMenu("Clean Grid")]
	private void CleanGrid()
	{
		m_GridTexture = null;
		m_Tiles = null;
	}

	private int GetIndexOf(int _Row, int _Column)
	{
		if (_Row < 0 || _Row >= m_Rows || _Column < 0 || _Column >= m_Columns)
			return -1;
		return _Row * m_Columns + _Column;
	}
	#endregion

	#region Getters / Setters
	public int Rows
	{
		get
		{
			return m_Rows;
		}
	}

	public int Columns
	{
		get
		{
			return m_Columns;
		}
	}
	#endregion

	#region Private Attributes
	[Header("File")]
	[SerializeField] private string m_FileName = string.Empty;

	[Header("Grid Renderer")]
	[SerializeField] private GridRenderer m_GridRenderer = null;

	[SerializeField] private Tile[] m_Tiles = null;
	private int m_Rows = 0;
	private int m_Columns = 0;
	private Texture2D m_GridTexture = null;
	#endregion
}
