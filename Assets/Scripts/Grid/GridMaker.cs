using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
	#region Public Methods

	[ContextMenu("Create Grid")]
	public GameGrid CreateGrid()
	{
		string pathFile = $@"{Application.persistentDataPath}/Maps/{m_FileName}";
		GameGrid grid = new GameGrid(pathFile, m_GridRenderer);
		return grid;
	}

	[ContextMenu("Create Grid Clustered")]
	public GridClustered CreateGridClustered(int _SizeClusterRows, int _SizeClusterColumns)
	{
		string pathFile = $@"{Application.persistentDataPath}/Maps/{m_FileName}";
		GridClustered grid = new GridClustered(pathFile, m_GridRenderer, _SizeClusterRows, _SizeClusterColumns);
		return grid;
	}

	public GameGrid CreateGrid(string _MapName)
	{
		if (_MapName == string.Empty)
			return null;

		string pathFile = $@"{Application.persistentDataPath}/Maps/{_MapName}";
		GameGrid grid = new GameGrid(pathFile, m_GridRenderer);
		return grid;
	}
	#endregion

	#region Private Methods
	#endregion

	#region Private Attributes
	[Header("File")]
	[SerializeField] private string m_FileName = string.Empty;

	[Header("Grid Renderer")]
	[SerializeField] private GridRenderer m_GridRenderer = null;
	#endregion
}
