using System.Collections.Generic;

public class PathLink : System.IComparable<PathLink>
{
    #region Public Methods
    public PathLink(Tile _CurrentTile)
    {
        m_PreviousLink = null;
        m_Tile = _CurrentTile;
        m_Weight = 0;
    }

    public PathLink(PathLink _Previous, Tile _CurrentTile)
    {
        m_PreviousLink = _Previous;
        m_Tile = _CurrentTile;
        m_Weight = m_PreviousLink.m_Weight + m_Tile.Weight;
    }

    public int CompareTo(PathLink _Other)
    {
        return m_Weight.CompareTo(_Other.Weight);
    }
    #endregion

    #region Getters/Setters
    public PathLink PreviousLink
    {
        get
        {
            return m_PreviousLink;
        }
    }

    public Tile Tile
    {
        get
        {
            return m_Tile;
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
    private PathLink m_PreviousLink = null;
    private Tile m_Tile = null;
    private int m_Weight = 0;
    #endregion
}