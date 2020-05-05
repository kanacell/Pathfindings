using System;
//--------------------------------------//
// To think how to implement this class //
//--------------------------------------//

public class PathNode<T> : IComparable<PathNode<T>> where T : IWeightable
{
	#region public Methods
	public PathNode(T _Data)
	{
		m_Data = _Data;
	}

	public PathNode<T> CreateExtensionWith(T _DataToAdd)
	{
		PathNode<T> extension = new PathNode<T>(_DataToAdd);
		extension.m_PreviousLink = this;
		extension.m_Weight += _DataToAdd.Weight();
		return extension;
	}

	public int CompareTo(PathNode<T> _Other)
	{
		return m_Weight.CompareTo(_Other.m_Weight);
	}
	#endregion

	#region Getters/Setters
	public T Data
	{
		get
		{
			return m_Data;
		}
	}

	public int Weight
	{
		get
		{
			return m_Weight;
		}
	}
	#endregion

	#region Private Attributes
	private PathNode<T> m_PreviousLink = null;
	private T m_Data = default;
	private int m_Weight = 0;
	#endregion
}
