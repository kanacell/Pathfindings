using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHeuristic : Path
{
	#region Public Methods
	public PathHeuristic(GameTile _Tile, int _Heuristic) : base(_Tile)
	{
		m_Heuristic = _Heuristic;
	}

	public PathHeuristic(PathHeuristic _ToCopy) : base(_ToCopy)
	{
		m_Heuristic = _ToCopy.m_Heuristic;
	}

	public void AddStep(GameTile _Tile, int _Heuristic)
	{
		base.AddStep(_Tile);
		m_Heuristic = _Heuristic;
	}

	public override int CompareTo(object obj)
	{
		PathHeuristic otherPath = obj as PathHeuristic;
		if (otherPath == null)
			return 1;

		return WeightWithHeuristic.CompareTo(otherPath.WeightWithHeuristic);
	}

	public List<PathHeuristic> GetExtendedPath(System.Func<GameTile, GameTile, int> _HeuristicFunction, GameTile _End)
	{
		List<PathHeuristic> allExtendedpaths = new List<PathHeuristic>(10); // assume 4 as initial capacity
		for (int i = 0; i < LastStep.Neighbors.Count; i++)
		{
			GameTile beforeLast = Steps.Count > 1 ? Steps[Steps.Count - 2] : null;
			GameTile neighbor = LastStep.Neighbors[i];
			if (!neighbor.IsAccessible || beforeLast == neighbor)
				continue;
			PathHeuristic extendedPath = new PathHeuristic(this);
			extendedPath.AddStep(neighbor, _HeuristicFunction.Invoke(neighbor, _End));
			allExtendedpaths.Add(extendedPath);
		}
		return allExtendedpaths;
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
