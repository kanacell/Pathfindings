
public class CustomComparer<T> : System.Collections.Generic.IComparer<T> where T : System.IComparable<T>
{
    #region Public Methods
    public CustomComparer(bool _AscendingOrder)
    {
        m_AscendingOrder = _AscendingOrder;
    }

    public int Compare(T _Caller, T _Target)
    {
        int compareValue = _Caller.CompareTo(_Target);
        if (!m_AscendingOrder)
        {
            compareValue *= -1;
        }
        return compareValue;
    }
    #endregion

    #region Private Attributes
    private bool m_AscendingOrder = false;
    #endregion
}