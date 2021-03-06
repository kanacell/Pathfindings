﻿
public class PathLinkHeuristic : PathLink, System.IComparable<PathLinkHeuristic>
{
	#region Public Methods
	public PathLinkHeuristic(Tile _CurrentTile, int _Heuristic) : base(_CurrentTile)
	{
		m_Heuristic = _Heuristic;
	}

	/**
	public PathLinkHeuristic(PathLinkHeuristic _Previous, Tile _CurrentTile, int _Heuristic) : base(_Previous, _CurrentTile)
	{
		m_Heuristic = _Heuristic;
	}
	/**/

	public PathLinkHeuristic MakeExtensionWith(Tile _NextTile, int _Heuristic)
	{
		PathLinkHeuristic extension = new PathLinkHeuristic(_NextTile, _Heuristic);
		extension.m_PreviousLink = this;
		extension.m_Weight = this.m_Weight + _NextTile.Weight;
		return extension;
	}

	public int CompareTo(PathLinkHeuristic _Other)
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
