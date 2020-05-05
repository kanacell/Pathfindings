using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Public Methods
    public void SearchPath()
    {
        if (m_StartTile == null)
        {
            m_StartTile = m_GameGrid.GetTileAt(m_RowStart, m_ColumnStart);
        }

        if (m_EndTile == null)
        {
            m_EndTile = m_GameGrid.GetTileAt(m_RowEnd, m_ColumnEnd);
        }

        m_PathVisualizer.gameObject.SetActive(false);

        Path path = null;
        ChronoInfos infos = new ChronoInfos();
        float timeLimit = m_HasLimitSearchTime ? m_TimeLimit : -1;
        switch (m_PathfindingMode)
        {
            case Pathfinder.PathfindingMode.PM_Dijkstra:
                path = Pathfinder.SearchDijkstraPathFromTo(m_StartTile, m_EndTile, out infos, timeLimit);
                break;
            case Pathfinder.PathfindingMode.PM_Dijkstra_LinkOpti:
                path = Pathfinder.SearchDisjktraPathFromTo_LinkOpti(m_StartTile, m_EndTile, out infos, timeLimit);
                break;
            case Pathfinder.PathfindingMode.PM_AStar:
                path = Pathfinder.AStarCustomBasic(m_StartTile, m_EndTile, out infos, timeLimit);
                break;
            case Pathfinder.PathfindingMode.PM_AStar_LinkOpti:
                path = Pathfinder.AStar_LinkOpti(m_StartTile, m_EndTile, out infos, timeLimit);
                break;
            case Pathfinder.PathfindingMode.PM_HPA:
                path = Pathfinder.SearchHPAFromTo(m_GridClustered, m_StartTile, m_EndTile, out infos, timeLimit);
                break;
            default:
                break;
        }

        if (m_ChronoText)
        {
            m_ChronoText.text = $"path found : {path != null}\n";
            m_ChronoText.text += $"elasped time : {infos.ElapsedTime} ms\n";
            m_ChronoText.text += $"foreach neighbors : {infos.ForeachNeighborsChrono} ms\n";
            m_ChronoText.text += $"clone path : {infos.ClonePathChrono} ms\n";
            m_ChronoText.text += $"extend path : { infos.ExtendPathChrono} ms\n";
            m_ChronoText.text += $"search open : {infos.SearchInOpenListChrono} ms\n";
            m_ChronoText.text += $"search close : {infos.SearchInCloseListChrono} ms\n";
            m_ChronoText.text += $"search open to insert : {infos.SearchInsertionChrono} ms\n";
            m_ChronoText.text += $"insert open list : {infos.InsertToOpenListChrono} ms\n";
            m_ChronoText.text += $"create solution : {infos.CreateSolutionChrono} ms\n";
        }

        if (path != null)
        {
            if (!m_PathVisualizer)
                return;

            m_PathVisualizer.gameObject.SetActive(true);
            m_PathVisualizer.TracePath(path, m_GameGrid.TilesRows);
        }
    }
    #endregion

    #region Private Methods
    private void Start()
    {
        if (m_PathVisualizer)
        {
            m_PathVisualizer.gameObject.SetActive(false);
        }

        if (m_PlayerInputs)
        {
            m_PlayerInputs.OnLeftClic += MarkAsStart;
            m_PlayerInputs.OnRightClic += MarkAsEnd;
        }

        m_StartMark?.gameObject.SetActive(false);
        m_EndMark?.gameObject.SetActive(false);

        if (m_PathfindingMode == Pathfinder.PathfindingMode.PM_HPA)
        {
            m_GameGrid = m_GridCreator.CreateGridClustered(m_SizeClusterRows, m_SizeClusterColumns);
            m_GridClustered = m_GameGrid as GridClustered;
            Debug.Log($"clusters : {m_GridClustered.Clusters.Length}");
        }
        else
        {
            m_GameGrid = m_GridCreator.CreateGrid();
        }
        Camera.main.transform.position = new Vector3(m_GameGrid.TilesColumns / 2f, 10, m_GameGrid.TilesRows / 2f);
        Camera.main.orthographicSize = Mathf.Max(m_GameGrid.TilesRows, m_GameGrid.TilesColumns) / 2f + 1;
    }

    private void Update()
    {
        if (m_GridClustered != null)
        {
            switch (m_DisplayBridgeMode)
            {
                case DisplayBridgeMode.DB_One:
                    DrawClustersLimits();
                    DrawOneCluster(m_RowCluster, m_ColumnCluster);
                    break;
                case DisplayBridgeMode.DB_All:
                    DrawClustersLimits();
                    DrawAllClusters();
                    break;
                default:
                    break;
            }
        }
    }

    private void MarkAsStart(Vector3 _ScreenPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            Tile t = GetNearestTileTo(hitInfos.point);
            if (!t.IsAccessible)
                return;

            m_StartTile = t;
            PlaceMarkerAt(m_StartMark, m_StartTile);
        }
    }

    private void MarkAsEnd(Vector3 _ScreenPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            Tile t = GetNearestTileTo(hitInfos.point);
            if (!t.IsAccessible)
                return;

            m_EndTile = t;
            PlaceMarkerAt(m_EndMark, m_EndTile);
        }
    }

    private Tile GetNearestTileTo(Vector3 _Point)
    {
        int row = m_GameGrid.TilesRows - Mathf.FloorToInt(_Point.z) - 1;
        int column = Mathf.FloorToInt(_Point.x);
        return m_GameGrid.GetTileAt(row, column);
    }

    private void PlaceMarkerAt(Transform _Marker, Tile _TileAbove)
    {
        _Marker.gameObject.SetActive(true);

        float x = _TileAbove.Column + 0.5f;
        float z = m_GameGrid.TilesRows - _TileAbove.Row - 1 + 0.5f;
        _Marker.position = new Vector3(x, 0, z); ;
    }

    private void DrawClustersLimits()
    {
        for (int i = 0; i < m_GridClustered.NbClustersOnRows; i++)
        {
            Vector3 startLocation = Vector3.zero;
            startLocation.x = 0;
            startLocation.y = 0.5f;
            startLocation.z = m_GridClustered.TilesRows - (i + 1) * m_SizeClusterRows;
            Vector3 endLocation = Vector3.zero;
            endLocation.x = m_GridClustered.TilesColumns;
            endLocation.y = startLocation.y;
            endLocation.z = startLocation.z;
            Debug.DrawLine(startLocation, endLocation, Color.yellow);
        }

        for (int i = 0; i < m_GridClustered.NbClustersOnColumns; i++)
        {
            Vector3 startLocation = Vector3.zero;
            startLocation.x = (i + 1) * m_SizeClusterColumns;
            startLocation.y = 0.5f;
            startLocation.z = m_GridClustered.TilesRows;
            Vector3 endLocation = Vector3.zero;
            endLocation.x = startLocation.x;
            endLocation.y = startLocation.y;
            endLocation.z = 0;
            Debug.DrawLine(startLocation, endLocation, Color.red);
        }
    }

    private void DrawOneCluster(int _RowCluster, int _ColumnCluster)
    {
        Cluster cluster = m_GridClustered.GetClusterAt(_RowCluster, _ColumnCluster);
        if (cluster == null)
            return;

        string message = $"draw {cluster.Bridges.Count} bridges\n";
        for (int i = 0; i < cluster.Bridges.Count; i++)
        {
            Bridge b = cluster.Bridges[i];
            Vector3 startLocation = new Vector3(b.Start.Column + 0.5f, 1.0f, m_GridClustered.TilesRows - b.Start.Row - 0.5f);
            Vector3 endLocation = new Vector3(b.End.Column + 0.5f, 1.0f, m_GridClustered.TilesRows - b.End.Row - 0.5f);
            message += $"\tfrom [{b.Start.Row}; {b.Start.Column}] to [{b.End.Row};{b.End.Column}]\n";
            Debug.DrawLine(startLocation, endLocation, Color.red);

            foreach (var neighborKey in b.Neighbors.Keys)
            {
                Vector3 neighborLocation = Vector3.one;
                neighborLocation.x = neighborKey.Start.Column + 0.5f;
                neighborLocation.z = m_GridClustered.TilesRows - neighborKey.Start.Row - 0.5f;
                Debug.DrawLine(startLocation, neighborLocation, Color.green);
            }
        }

        message += "\n";
        message += $"draw bridges links\n";
        Debug.Log(message);
    }

    private void DrawAllClusters()
    {
        for (int i = 0; i < m_GridClustered.NbClustersOnRows; i++)
        {
            for (int j = 0; j < m_GridClustered.NbClustersOnColumns; j++)
            {
                DrawOneCluster(i, j);
            }
        }
    }
    #endregion

    #region Private Attributes
    [Header("Pathfinding Mode")]
    [SerializeField] private Pathfinder.PathfindingMode m_PathfindingMode = Pathfinder.PathfindingMode.PM_None;
    [SerializeField] private float m_TimeLimit = 1f;
    [SerializeField] private bool m_HasLimitSearchTime = true;

    [Header("Path UI")]
    [SerializeField] private TextMeshProUGUI m_ChronoText = null;

    [Header("Path Visualizer")]
    [SerializeField] private Transform m_StartMark = null;
    [SerializeField] private Transform m_EndMark = null;
    [SerializeField] private PathVisualizer m_PathVisualizer = null;

    [Header("Inputs")]
    [SerializeField] private PlayerInputs m_PlayerInputs = null;

    [Header("Grid Settings")]
    [SerializeField] private GridManager m_GridCreator = null;

    [Header("Start Tile")]
    [SerializeField] private int m_RowStart = 0;
    [SerializeField] private int m_ColumnStart = 0;

    [Header("End Tile")]
    [SerializeField] private int m_RowEnd = 0;
    [SerializeField] private int m_ColumnEnd = 0;

    [Header("Clusters Settings")]
    [SerializeField] private DisplayBridgeMode m_DisplayBridgeMode = DisplayBridgeMode.DB_None;
    [SerializeField] private int m_SizeClusterRows = 0;
    [SerializeField] private int m_SizeClusterColumns = 0;
    [SerializeField] private int m_RowCluster = 0;
    [SerializeField] private int m_ColumnCluster = 0;

    private GameGrid m_GameGrid = null;
    private GridClustered m_GridClustered = null;
    private Tile m_StartTile = null;
    private Tile m_EndTile = null;
    #endregion

    #region Enumerations
    private enum DisplayBridgeMode
    {
        DB_None,
        DB_One,
        DB_All
    }
    #endregion
}
