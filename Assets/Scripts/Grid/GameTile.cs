using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GameTile : MonoBehaviour
{
	#region Public Methods
	public void AddNeighbor(GameTile _Neighbor)
	{
		if (!_Neighbor)
			return;
		m_Neighbors.Add(_Neighbor);
	}

	public void Unmark()
	{
		UpdateMeshColor(Color.white);
	}

	public void MarkAsStart()
	{
		UpdateMeshColor(Color.green);
	}

	public void MarkAsEnd()
	{
		UpdateMeshColor(Color.yellow);
	}
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	private void InitGridPosition(int _Row, int _Column)
	{
		m_GridPosition.x = _Column;
		m_GridPosition.y = _Row;
	}

	private void InitWeight(int _Weight)
	{
		m_Weight = _Weight;
		m_IsAccessible = m_Weight > 0;
		if (!m_IsAccessible && m_MeshRenderer)
		{
			Material tempMat = new Material(m_MeshRenderer.sharedMaterial);
			tempMat.color = Color.black;
			m_MeshRenderer.sharedMaterial = tempMat;
		}
	}

	private void FixNullNeighbors()
	{
		for(int i = m_Neighbors.Count - 1; i >= 0; i--)
		{
			if (!m_Neighbors[i])
			{
				m_Neighbors.RemoveAt(i);
			}
		}
	}

	private void UpdateMeshColor(Color _Color)
	{
		if (!m_MeshRenderer)
			return;
		m_MeshRenderer.material.color = _Color;
	}
	#endregion

	#region Getters/Setters
	public bool IsAccessible
	{
		get
		{
			return m_IsAccessible;
		}
	}

	public int Weight
	{
		get
		{
			return m_Weight;
		}
	}

	public Vector2Int GridPosition
	{
		get
		{
			return m_GridPosition;
		}
	}

	public IReadOnlyList<GameTile> Neighbors
	{
		get
		{
			return m_Neighbors;
		}
	}
	#endregion

	#region Private Attributes
	[SerializeField] private bool m_IsAccessible = true;
	[SerializeField, Range(0, 10)] private int m_Weight = 1;
	[SerializeField] MeshRenderer m_MeshRenderer = null;
	[SerializeField, HideInInspector] private List<GameTile> m_Neighbors = new List<GameTile>(8); // 8 because I assume at most 8 neighbors to a square tile with diagonals
	[SerializeField] private Vector2Int m_GridPosition = Vector2Int.zero;
	#endregion
}
