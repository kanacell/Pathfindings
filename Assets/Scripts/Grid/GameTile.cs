﻿using System.Collections;
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
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	#endregion

	#region Getters/Setters
	public bool IsAccessible
	{
		get
		{
			return m_IsAccessible;
		}
	}

	public uint Weight
	{
		get
		{
			return m_Weight;
		}
	}
	#endregion

	#region Public Attributes
	#endregion

	#region Protected Attributes
	#endregion

	#region Private Attributes
	[SerializeField] private bool m_IsAccessible = true;
	[SerializeField, Range(1, 10)] private uint m_Weight = 1;
	private List<GameTile> m_Neighbors = new List<GameTile>(8); // 8 because I assume at most 8 neighbors to a square tile with diagonals
	#endregion
}