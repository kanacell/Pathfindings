using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Path : System.IComparable<Path>
{
	#region Public Methods
	public static Path CreatePath(PathLink _Link)
	{
		Path p = new Path();
		p.m_Weight = _Link.Weight;
		while(_Link != null)
		{
			p.m_Steps.Insert(0, _Link.Tile);
			_Link = _Link.PreviousLink;
		}
		return p;
	}

	public Path(Tile _Tile)
	{
		m_Steps.Add(_Tile);
		m_Weight = 0; // init weight to zero just to be sure even if the value should already be initialized to zero at declaration
	}

	public Path(Path _ToCopy)
	{
		m_Steps.AddRange(_ToCopy.m_Steps);
		m_Weight = _ToCopy.m_Weight;
	}

	public void AddStep(Tile _Tile)
	{
		m_Steps.Add(_Tile);
		m_Weight += _Tile.Weight;
	}

	public bool IsBeforeLast(Tile _Tile)
	{
		if (Steps.Count == 1)
			return false;
		return Steps[Steps.Count - 2] == _Tile;
	}

	public List<Path> GetExtendedPaths()
	{
		List<Path> allExtendedpaths = new List<Path>();
		for (int i = 0; i < LastStep.Neighbors.Count; i++)
		{
			Tile neighbor = LastStep.Neighbors[i];
			if (!neighbor.IsAccessible)
				continue;

			Tile beforeLast = Steps.Count > 1 ? Steps[Steps.Count - 2] : null;
			if (neighbor == beforeLast)
				continue;

			Path extendedPath = new Path(this);
			extendedPath.AddStep(neighbor);
			allExtendedpaths.Add(extendedPath);
		}
		return allExtendedpaths;
	}

	public virtual int CompareTo(Path _Other)
	{
		return Weight.CompareTo(_Other.Weight);
	}
	#endregion

	#region Private Methods
	private Path()
	{
	}
	#endregion

	#region Getters/Setters
	public Tile LastStep
	{
		get
		{
			return m_Steps.LastOrDefault();
		}
	}

	public int Weight
	{
		get
		{
			return m_Weight;
		}
	}

	public IReadOnlyList<Tile> Steps
	{
		get
		{
			return m_Steps;
		}
	}
	#endregion

	#region Private Attributes
	protected List<Tile> m_Steps = new List<Tile>(10); // 10 as arbritary value to initial capacity
	private int m_Weight = 0;
	#endregion
}