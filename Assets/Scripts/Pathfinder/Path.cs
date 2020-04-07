using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Path
{
	#region Public Methods
	public Path(GameTile _Tile)
	{
		m_Steps.Add(_Tile);
		m_Weight = 0; // init weight to zero just to be sure even if the value should already be initialized to zero at declaration
	}

	public Path(Path _ToCopy)
	{
		m_Steps.AddRange(_ToCopy.m_Steps);
		m_Weight = _ToCopy.m_Weight;
	}

	public void AddStep(GameTile _Tile)
	{
		m_Steps.Add(_Tile);
		m_Weight += _Tile.Weight;
	}
	#endregion

	#region Getters/Setters
	public GameTile LastStep
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

	public IReadOnlyList<GameTile> Steps
	{
		get
		{
			return m_Steps;
		}
	}
	#endregion

	#region Private Attributes
	private List<GameTile> m_Steps = new List<GameTile>();
	private int m_Weight = 0;
	#endregion
}
