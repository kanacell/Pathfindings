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
