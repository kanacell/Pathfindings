using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	public void MergeBefore(Path _PathToMerge)
	{
		List<Tile> tempSteps = _PathToMerge.m_Steps;
		tempSteps.AddRange(m_Steps);
		m_Steps = tempSteps;
		m_Weight += _PathToMerge.m_Weight;
	}

	public Path ReversedPath()
	{
		Path reverse = new Path(this);
		reverse.m_Steps.Reverse();
		return reverse;
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