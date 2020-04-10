using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHeuristic : Path, IComparable<PathHeuristic>
{
	#region Public Methods
	public PathHeuristic(Tile _Tile, int _Heuristic) : base(_Tile)
	{
		m_Heuristic = _Heuristic;
	}

	public PathHeuristic(PathHeuristic _ToCopy) : base(_ToCopy)
	{
		m_Heuristic = _ToCopy.m_Heuristic;
	}

	public void AddStep(Tile _Tile, int _Heuristic)
	{
		base.AddStep(_Tile);
		m_Heuristic = _Heuristic;
	}


	public List<PathHeuristic> GetExtendedPaths(System.Func<Tile, Tile, int> _HeuristicFunction, Tile _End)
	{
		List<PathHeuristic> allExtendedpaths = new List<PathHeuristic>(10); // assume 4 as initial capacity
		for (int i = 0; i < LastStep.Neighbors.Count; i++)
		{
			Tile beforeLast = Steps.Count > 1 ? Steps[Steps.Count - 2] : null;
			Tile neighbor = LastStep.Neighbors[i];
			if (!neighbor.IsAccessible || beforeLast == neighbor)
				continue;
			PathHeuristic extendedPath = new PathHeuristic(this);
			extendedPath.AddStep(neighbor, _HeuristicFunction.Invoke(neighbor, _End));
			allExtendedpaths.Add(extendedPath);
		}
		return allExtendedpaths;
	}

	public int CompareTo(PathHeuristic _Other)
	{
		return WeightWithHeuristic.CompareTo(_Other.WeightWithHeuristic);
	}
	#endregion

	#region Getters/Setters
	public int WeightWithHeuristic
	{
		get
		{
			return Weight + m_Heuristic;
		}
	}
	#endregion

	#region Private Attributes
	private int m_Heuristic = 0;
	#endregion
}