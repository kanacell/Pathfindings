using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridManager : MonoBehaviour
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
		GridClustered grid = new GridClustered(pathFile, m_GridRenderer);
		grid.GenerateClusters(_SizeClusterRows, _SizeClusterColumns);
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
