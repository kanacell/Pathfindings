using System;
using System.Collections.Generic;
using System.Linq;

public class Cluster
{
    #region Public Methods
    public Cluster(int _AnchorRow, int _AnchorColumn, int _SizeRows, int _SizeColumns)
    {
        m_AnchorRow = _AnchorRow;
        m_AnchorColumn = _AnchorColumn;
        m_SizeRows = _SizeRows;
        m_SizeColumns = _SizeColumns;

    }

    public Cluster(int _AnchorRow, int _AnchorColumn, int _Rows, int _Columns, GridClustered _OwnerGrid) : this(_AnchorRow, _AnchorColumn, _Rows, _Columns)
    {
        m_OwnerGrid = _OwnerGrid;
    }

    public void GenerateLeftBridgesWith(Cluster _LeftCluster)
    {
        int offsetRow = 0;
        int rowBeginPart = -1;
        while (offsetRow < m_SizeRows && m_AnchorRow + offsetRow < m_OwnerGrid.TilesRows)
        {
            Tile currentTile = m_OwnerGrid.GetTileAt(m_AnchorRow + offsetRow, m_AnchorColumn);
            Tile leftNeighbor = m_OwnerGrid.GetTileAt(m_AnchorRow + offsetRow, m_AnchorColumn - 1);
            if (currentTile.IsAccessible && leftNeighbor.IsAccessible)
            {
                if (rowBeginPart == -1)
                {
                    rowBeginPart = m_AnchorRow + offsetRow;
                }
            }
            else if (rowBeginPart != -1)
            {
                int rowBridge = (rowBeginPart + m_AnchorRow + offsetRow - 1) / 2;
                Tile startBridge = m_OwnerGrid.GetTileAt(rowBridge, m_AnchorColumn);
                Tile endBridge = m_OwnerGrid.GetTileAt(rowBridge, m_AnchorColumn - 1);
                m_Bridges.Add(new Bridge(startBridge, endBridge));
                _LeftCluster.m_Bridges.Add(new Bridge(endBridge, startBridge));
                rowBeginPart = -1;
            }
            offsetRow++;
            currentTile = m_OwnerGrid.GetTileAt(m_AnchorRow + offsetRow, m_AnchorColumn);
        }
        if (rowBeginPart != -1)
        {
            int rowBridge = (rowBeginPart + m_AnchorRow + offsetRow - 1) / 2;
            Tile startBridge = m_OwnerGrid.GetTileAt(rowBridge, m_AnchorColumn);
            Tile endBridge = m_OwnerGrid.GetTileAt(rowBridge, m_AnchorColumn - 1);
            m_Bridges.Add(new Bridge(startBridge, endBridge));
            _LeftCluster.m_Bridges.Add(new Bridge(endBridge, startBridge));
        }
    }

    public void GenerateUpBridgesWith(Cluster _UpCluster)
    {
        int offsetColumn = 0;
        int columnBeginPart = -1;
        while (offsetColumn < m_SizeColumns && m_AnchorColumn + offsetColumn < m_OwnerGrid.TilesColumns)
        {
            Tile currentTile = m_OwnerGrid.GetTileAt(m_AnchorRow, m_AnchorColumn + offsetColumn);
            Tile upNeighbor = m_OwnerGrid.GetTileAt(m_AnchorRow - 1, m_AnchorColumn + offsetColumn );
            if (currentTile.IsAccessible && upNeighbor.IsAccessible)
            {
                if (columnBeginPart == -1)
                {
                    columnBeginPart = m_AnchorColumn + offsetColumn;
                }
            }
            else if (columnBeginPart != -1)
            {
                int columnBridge = (columnBeginPart + m_AnchorColumn + offsetColumn - 1) / 2;
                Tile startBridge = m_OwnerGrid.GetTileAt(m_AnchorRow, columnBridge);
                Tile endBridge = m_OwnerGrid.GetTileAt(m_AnchorRow - 1, columnBridge);
                m_Bridges.Add(new Bridge(startBridge, endBridge));
                _UpCluster.m_Bridges.Add(new Bridge(endBridge, startBridge));
                columnBeginPart = -1;
            }
            offsetColumn++;
        }

        if (columnBeginPart != -1)
        {
            int columnBridge = (columnBeginPart + m_AnchorColumn + offsetColumn - 1) / 2;
            Tile startBridge = m_OwnerGrid.GetTileAt(m_AnchorRow, columnBridge);
            Tile endBridge = m_OwnerGrid.GetTileAt(m_AnchorRow - 1, columnBridge);
            m_Bridges.Add(new Bridge(startBridge, endBridge));
            _UpCluster.m_Bridges.Add(new Bridge(endBridge, startBridge));
        }
    }

    public void LinkBridges()
    {
        for (int i = 0; i < m_Bridges.Count; i++)
        {
            Tile startTile = m_Bridges[i].Start;
            for (int j = i + 1; j < m_Bridges.Count; j++)
            {
                Tile endTile = m_Bridges[j].Start;

                // Compute path inside cluster to bind each bridge if it's possible.
                Path p = ComputeInternalPathFromTo(startTile, endTile);

                if (p != null)
                {
                    m_Bridges[i].AddNeighbor(m_Bridges[j], p);
                    m_Bridges[j].AddNeighbor(m_Bridges[i], p.ReversedPath());
                }
            }
        }
    }

    public bool IsTileInside(Tile _Tile)
    {
        bool isInside = _Tile.Row >= m_AnchorRow;
        isInside = isInside && _Tile.Row < m_AnchorRow + m_SizeRows;
        isInside = isInside && _Tile.Column >= m_AnchorColumn;
        isInside = isInside && _Tile.Column < m_AnchorColumn + m_SizeColumns;
        return isInside;
    }

    public Path ComputeInternalPathFromTo(Tile _StartTile, Tile _EndTile)
    {
        List<PathLinkHeuristic> openList = new List<PathLinkHeuristic>();
        HashSet<Tile> closeSet = new HashSet<Tile>();
        openList.Add(new PathLinkHeuristic(_StartTile, Tile.GetManhattanDistance(_StartTile, _EndTile)));
        CustomComparer<PathLinkHeuristic> comparer = new CustomComparer<PathLinkHeuristic>(false);

        while (openList.Count > 0)
        {
            PathLinkHeuristic linkToExtend = openList.Last();
            if (linkToExtend.Tile == _EndTile)
            {
                return Path.CreatePath(linkToExtend);
            }
            openList.RemoveAt(openList.Count - 1);
            Tile tileToExtend = linkToExtend.Tile;
            if (closeSet.Contains(tileToExtend))
                continue;

            closeSet.Add(tileToExtend);
            for (int i = 0; i < tileToExtend.Neighbors.Count; i++)
            {
                Tile neighbor = tileToExtend.Neighbors[i];
                if (!neighbor.IsAccessible || !IsTileInside(neighbor) || closeSet.Contains(neighbor))
                    continue;

                //PathLinkHeuristic extension = new PathLinkHeuristic(linkToExtend, neighbor, Tile.GetManhattanDistance(neighbor, _EndTile));
                PathLinkHeuristic extension = linkToExtend.MakeExtensionWith(neighbor, Tile.GetManhattanDistance(neighbor, _EndTile));
                int indexInsertion = openList.BinarySearch(extension, comparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                openList.Insert(indexInsertion, extension);
            }

        }

        return null;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion

    #region Getters/Setters
    public List<Bridge> Bridges
    {
        get
        {
            return m_Bridges;
        }
    }

    //public IReadOnlyDictionary<Bridge, Tuple<Bridge, Path>> BridgesLinks
    //{
    //    get
    //    {
    //        return m_BridgesLinks;
    //    }
    //}
    #endregion

    #region Private Attributes
    private GridClustered m_OwnerGrid = null;
    private int m_AnchorRow = 0;
    private int m_AnchorColumn = 0;
    private int m_SizeRows = 0;
    private int m_SizeColumns = 0;
    private List<Bridge> m_Bridges = new List<Bridge>();
    //private Dictionary<Bridge, Tuple<Bridge, Path>> m_BridgesLinks = new Dictionary<Bridge, Tuple<Bridge, Path>>();
    #endregion
}
