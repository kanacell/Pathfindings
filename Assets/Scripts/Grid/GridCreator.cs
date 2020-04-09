using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	[ContextMenu("Create Grid")]
	private void CreateGrid()
	{
		CleanGrid();

		string pathFile = $@"{Application.persistentDataPath}/Maps/{m_FileName}";
		string[] allLines = File.ReadAllLines(pathFile);
		int rows = int.Parse(allLines[0]);
		int columns = int.Parse(allLines[1]);

		m_Tiles = new Tile[rows, columns];
		m_GridTexture = new Texture2D(columns, rows);
		m_GridTexture.filterMode = FilterMode.Point;

		int weight = 0;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				weight = (int)char.GetNumericValue(allLines[i + 2][j]);
				m_GridTexture.SetPixel(j, rows - i - 1, weight <= 0 ? Color.black : Color.white);

				m_Tiles[i, j] = new Tile(i, j, weight);

				if (i > 0)
				{
					m_Tiles[i, j].AddNeighbor(m_Tiles[i - 1, j]);
					m_Tiles[i - 1, j].AddNeighbor(m_Tiles[i, j]);
				}

				if (j > 0)
				{
					m_Tiles[i, j].AddNeighbor(m_Tiles[i, j - 1]);
					m_Tiles[i, j - 1].AddNeighbor(m_Tiles[i, j]);
				}
			}
		}

		m_GridTexture.Apply();
		m_GridRenderer.transform.localScale = new Vector3Int(columns, 1, rows);
		m_GridRenderer.ApplyTexture(m_GridTexture);

		Camera.main.transform.position = new Vector3(columns / 2f, 10, rows / 2f);
		Camera.main.orthographicSize = Mathf.Max(rows, columns) / 2f + 0.5f;
		m_GridRenderer.transform.position = new Vector3(columns / 2f, 0, rows / 2f);
	}

	[ContextMenu("Clean Grid")]
	private void CleanGrid()
	{
		m_GridTexture = null;
		m_Tiles = null;
	}
	#endregion

	#region Getters/Setters
	#endregion

	#region Private Attributes
	[Header("File")]
	[SerializeField] private string m_FileName = string.Empty;

	[Header("Grid Renderer")]
	[SerializeField] private GridRenderer m_GridRenderer = null;

	private Tile[,] m_Tiles = null;
	private Texture2D m_GridTexture = null;
	#endregion
}
