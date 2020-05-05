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

    public PathLink MakeExtensionWith(Tile _NextTile)
    {
        PathLink extension = new PathLink(_NextTile);
        extension.m_PreviousLink = this;
        extension.m_Weight = this.Weight + _NextTile.Weight;
        return extension;
    }

    /**
    public PathLink(PathLink _Previous, Tile _CurrentTile)
    {
        m_PreviousLink = _Previous;
        m_Tile = _CurrentTile;
        m_Weight = m_PreviousLink.m_Weight + m_Tile.Weight;
    }
    /**/

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

    #region Protected Attributes
    protected PathLink m_PreviousLink = null;
    protected Tile m_Tile = null;
    protected int m_Weight = 0;
    #endregion
}