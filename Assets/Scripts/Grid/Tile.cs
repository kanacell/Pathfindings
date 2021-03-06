﻿using System.Collections.Generic;
using System;

public class Tile
{
	#region Public Methods
	public static int GetManhattanDistance(Tile _TileA, Tile _TileB)
	{
		if (_TileA == null || _TileB == null)
			return -1;
		return Math.Abs(_TileA.Row - _TileB.Row) + Math.Abs(_TileA.Column - _TileB.Column);
	}
	public Tile(int _Row, int _Column, int _Weight)
	{
		m_PosRow = _Row;
		m_PosColumn = _Column;
		m_Weight = _Weight;
	}

	public void AddNeighbor(Tile _Neighbor)
	{
		if (_Neighbor == null)
			return;
		m_Neighbors.Add(_Neighbor);
	}
	#endregion

	#region Getters/Setters
	public int Row
	{
		get
		{
			return m_PosRow;
		}
	}

	public int Column
	{
		get
		{
			return m_PosColumn;
		}
	}

	public bool IsAccessible
	{
		get
		{
			return m_Weight > 0;
		}
	}

	public int Weight
	{
		get
		{
			return m_Weight;
		}
	}

	public IReadOnlyList<Tile> Neighbors
	{
		get
		{
			return m_Neighbors;
		}
	}
	#endregion

	#region Private Attributes
	private int m_PosRow = 0;
	private int m_PosColumn = 0;
	private int m_Weight = 0;
	private List<Tile> m_Neighbors = new List<Tile>(8);
	#endregion
}
