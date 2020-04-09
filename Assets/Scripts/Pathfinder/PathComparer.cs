using System.Collections.Generic;

public class PathComparer : IComparer<Path>
{
	#region Public Methods
    public PathComparer(bool _Ascending)
    {
        m_AscendingOrder = _Ascending;
    }

	public int Compare(Path _A, Path _B)
	{
        int retValue;
        if (_A == null)
        {
            if (_B == null)
            {
                retValue = 0;
            }
            else
            {
                retValue = -1;
            }
        }
        else
        {
            if (_B == null)
            {
                retValue = 1;
            }
            else
            {
                retValue = _A.CompareTo(_B);
            }
        }

        if (!m_AscendingOrder)
        {
            retValue *= -1;
        }
        return retValue;
        //return retValue * -1; // * -1 to sort from the biggest to the smallest path
    }
    #endregion

    #region Private Attributes
    private bool m_AscendingOrder = true;
    #endregion
}

public class PathHeuristicComparer : IComparer<PathHeuristic>
{
    #region Public Methods
    public PathHeuristicComparer(bool _Ascending)
    {
        m_AscendingOrder = _Ascending;
    }

    public int Compare(PathHeuristic _A, PathHeuristic _B)
    {
        int retValue;
        if (_A == null)
        {
            if (_B == null)
            {
                retValue = 0;
            }
            else
            {
                retValue = -1;
            }
        }
        else
        {
            if (_B == null)
            {
                retValue = 1;
            }
            else
            {
                retValue = _A.CompareTo(_B);
            }
        }

        if (!m_AscendingOrder)
        {
            retValue *= -1;
        }
        return retValue;
        //return retValue * -1; // * -1 to sort from the biggest to the shortest path
    }
    #endregion

    #region Private Attributes
    private bool m_AscendingOrder = false;
    #endregion
}