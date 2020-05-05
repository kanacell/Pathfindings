
//--------------------------------------//
// To think how to implement this class //
//--------------------------------------//

public class CustomPathLinked<T> where T : IWeightable
{
	#region public Methods
	public CustomPathLinked(T _Data)
	{
		m_Data = _Data;
	}

	public CustomPathLinked<T> CreateExtensionWith(T _DataToAdd)
	{
		CustomPathLinked<T> extension = new CustomPathLinked<T>(_DataToAdd);
		extension.m_PreviousLink = this;
		extension.m_Weight += _DataToAdd.Weight();
		return extension;
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
	private CustomPathLinked<T> m_PreviousLink = null;
	private T m_Data = default;
	private int m_Weight = 0;
	#endregion
}
