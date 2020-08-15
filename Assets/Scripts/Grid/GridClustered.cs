using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GridClustered : GameGrid
{
    #region Public Methods
    public GridClustered(string _FileName, GridRenderer _Renderer, int _SizeClusterRows, int _SizeClusterColumns) : base(_FileName, _Renderer)
    {
        GenerateClusters(_SizeClusterRows, _SizeClusterColumns);
    }

    public GridClustered(GameGrid _GameGrid, int _SizeClusterRows, int _SizeClusterColumns) : base (_GameGrid)
    {
        GenerateClusters(_SizeClusterRows, _SizeClusterColumns);
    }

    public void GenerateClusters(int _SizeClusterRows, int _SizeClusterColumns)
    {

        if (_SizeClusterRows < 1 || _SizeClusterColumns < 1)
            return;

        System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        float generationClusterTimer = 0f;
        chrono.Restart();

        m_BaseClusterSizeRows = _SizeClusterRows;
        m_NbClustersOnRows = TilesRows / m_BaseClusterSizeRows;
        if (TilesRows % m_BaseClusterSizeRows > 0)
        {
            m_NbClustersOnRows++;
        }

        m_BaseClusterSizeColumns = _SizeClusterColumns;
        m_NbClustersOnColumns = TilesColumns / m_BaseClusterSizeColumns;
        if (TilesColumns % m_BaseClusterSizeColumns > 0)
        {
            m_NbClustersOnColumns++;
        }

        m_Clusters = new Cluster[m_NbClustersOnRows * m_NbClustersOnColumns];
        for (int i = 0; i < m_NbClustersOnRows; i++)
        {
            for (int j = 0; j < m_NbClustersOnColumns; j++)
            {
                int indexCluster = GetClusterIndexOf(i, j);
                int anchorRow = i * _SizeClusterRows;
                int anchorColumn = j * _SizeClusterColumns;
                m_Clusters[indexCluster] = new Cluster(anchorRow, anchorColumn, _SizeClusterRows, _SizeClusterColumns, this);

                /**/
                if (i > 0)
                {
                    int indexClusterNeighbor = GetClusterIndexOf(i - 1, j);
                    m_Clusters[indexCluster].GenerateUpBridgesWith(m_Clusters[indexClusterNeighbor]);
                }
                if (j > 0)
                {
                    int indexClusterNeighbor = GetClusterIndexOf(i, j - 1);
                    m_Clusters[indexCluster].GenerateLeftBridgesWith(m_Clusters[indexClusterNeighbor]);
                }
                /**/
            }
        }

        /**/
        for (int i = 0; i < m_Clusters.Length; i++)
        {
            //m_Clusters[i].GenerateBridges();
            m_Clusters[i].LinkBridges();
        }
        /**/

        /**
        NativeArray<ClusterWrapper> clusterWrappers = new NativeArray<ClusterWrapper>(m_Clusters.Length, Allocator.Temp);

        for (int i  = 0; i < m_Clusters.Length; i++)
        {
            clusterWrappers[i] = new ClusterWrapper
            {
                m_Cluster = m_Clusters[i],
            };
        }

        ClustersInitializerParallelJob clusterInitializerParallelJob = new ClustersInitializerParallelJob
        {
            clusters = clusterWrappers,
        };

        JobHandle jobHandle =  clusterInitializerParallelJob.Schedule(m_Clusters.Length, 4);
        jobHandle.Complete();
        /**/

        chrono.Stop();
        generationClusterTimer = chrono.ElapsedMilliseconds;

        string message = $"clusters generation : {generationClusterTimer}ms";
        Debug.Log(message);
    }

    public Cluster GetClusterAt(int _RowCluster, int _ColumnCluster)
    {
        int indexCluster = GetClusterIndexOf(_RowCluster, _ColumnCluster);
        if (indexCluster == -1)
            return null;
        return m_Clusters[indexCluster];
    }

    public Cluster GetClusterOfTile(Tile _Tile)
    {
        int rowCluster = _Tile.Row / m_BaseClusterSizeRows;
        int columnCluster = _Tile.Column / m_BaseClusterSizeColumns;
        return GetClusterAt(rowCluster, columnCluster);
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private int GetClusterIndexOf(int _Row, int _Column)
    {
        if (_Row < 0 || _Row >= m_NbClustersOnRows || _Column < 0 || _Column >= m_NbClustersOnColumns)
            return -1;
        return _Row * m_NbClustersOnColumns + _Column;
    }

    private void GenerateBridges(Cluster _ClusterA, Cluster _ClusterB)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Getters/Setters
    public int NbClustersOnRows
    {
        get
        {
            return m_NbClustersOnRows;
        }
    }

    public int NbClustersOnColumns
    {
        get
        {
            return m_NbClustersOnColumns;
        }
    }

    public Cluster[] Clusters
    {
        get
        {
            return m_Clusters;
        }
    }
    #endregion

    #region Private Attributes
    private int m_NbClustersOnRows = 0;
    private int m_NbClustersOnColumns = 0;
    private int m_BaseClusterSizeRows = 0;
    private int m_BaseClusterSizeColumns = 0;
    private Cluster[] m_Clusters = null;
    #endregion
}

public struct ClustersInitializerParallelJob : IJobParallelFor
{
    public NativeArray<ClusterWrapper> clusters;
    public void Execute(int _Index)
    {
        //clusters[_Index].m_Cluster.GenerateBridges();
        //clusters[_Index].m_Cluster.LinkBridges();
    }
}

public struct ClusterWrapper
{
    //public Cluster m_Cluster;
    public int value;
}