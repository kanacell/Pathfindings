using System.Collections.Generic;

public class Bridge
{
    #region Public Methods
    public Bridge(Tile _Start, Tile _End)
    {
        m_Start = _Start;
        m_End = _End;
    }

    public void AddNeighbor(Bridge _Neighbor, Path _Path)
    {
        m_Neighbors.Add(_Neighbor, _Path);
    }

    public override bool Equals(object _Other)
    {
        return _Other is Bridge bridge && bridge == this;
    }

    public override int GetHashCode()
    {
        return m_Start.GetHashCode() * m_End.GetHashCode();
    }

    public Path GetPathToBridgeBeginningWith(Tile _TileToStartWith)
    {
        foreach (Bridge key in m_Neighbors.Keys)
        {
            if (key.Start == _TileToStartWith)
                return m_Neighbors[key];
        }
        return null;
    }
    #endregion

    #region Getters/Setters
    public Tile Start
    {
        get
        {
            return m_Start;
        }
    }

    public Tile End
    {
        get
        {
            return m_End;
        }
    }

    public IReadOnlyDictionary<Bridge, Path> Neighbors
    {
        get
        {
            return m_Neighbors;
        }
    }
    #endregion

    #region Operators
    public static bool operator ==(Bridge _A, Bridge _B)
    {
        bool areSame = _A.Start == _B.Start;
        areSame = areSame && _A.End == _B.End;
        return areSame;
    }

    public static bool operator !=(Bridge _A, Bridge _B)
    {
        return !(_A == _B);
    }
    #endregion

    #region Private Attributes
    private Tile m_Start;
    private Tile m_End;
    private Dictionary<Bridge, Path> m_Neighbors = new Dictionary<Bridge, Path>();
    #endregion
}
