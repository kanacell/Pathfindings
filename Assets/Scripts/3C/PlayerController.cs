using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Public Methods
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
            m_PlayerInputs.OnHovered += HoverTile;
            m_PlayerInputs.OnLeftClic += MarkAsStart;
            m_PlayerInputs.OnRightClic += MarkAsEnd;
        }

        m_StartMark?.gameObject.SetActive(false);
        m_EndMark?.gameObject.SetActive(false);

        if (m_Hud)
        {
            m_Hud.CreateMapFromFile += GenerateMap;
            m_Hud.OnGenerateCluster += GenerateClusters;
            m_Hud.OnSearch += SearchPath;
        }

        if (m_AutoGenerateMap)
        {
            if (m_PathfindingMode == Pathfinder.PathfindingMode.PM_HPA)
            {
                m_GameGrid = m_GridMaker.CreateGridClustered(m_SizeClusterRows, m_SizeClusterColumns);
                m_GridClustered = m_GameGrid as GridClustered;
                Debug.Log($"clusters : {m_GridClustered.Clusters.Length}");
            }
            else
            {
                m_GameGrid = m_GridMaker.CreateGrid();
            }
            Camera.main.transform.position = new Vector3(m_GameGrid.TilesColumns / 2f, 10, m_GameGrid.TilesRows / 2f);
            Camera.main.orthographicSize = Mathf.Max(m_GameGrid.TilesRows, m_GameGrid.TilesColumns) / 2f + 1;
        }


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

    private void HoverTile(Vector3 _ScreenPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            Tile t = GetNearestTileTo(hitInfos.point);
            m_Hud?.SetHoveredCoords(t);
            PlaceMarkerAt(m_HoverMark, t);
        }
        else
        {
            m_HoverMark?.gameObject.SetActive(false);
            m_Hud?.CancelHoveredCoords();
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
            m_Hud?.SetStartCoords(m_StartTile);
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
            m_Hud?.SetEndCoords(m_EndTile);
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
        if (!_Marker)
            return;

        _Marker.gameObject.SetActive(true);

        float x = _TileAbove.Column + 0.5f;
        float z = m_GameGrid.TilesRows - _TileAbove.Row - 1 + 0.5f;
        _Marker.position = new Vector3(x, 0, z); ;
    }

    private void SearchPath(Pathfinder.PathfindingMode _SearchMode)
    {
        Debug.Log(_SearchMode);

        /**
        if (m_StartTile == null)
        {
            m_StartTile = m_GameGrid.GetTileAt(m_RowStart, m_ColumnStart);
        }

        if (m_EndTile == null)
        {
            m_EndTile = m_GameGrid.GetTileAt(m_RowEnd, m_ColumnEnd);
        }
        /**/

        if (m_PathVisualizer)
        {
            m_PathVisualizer.gameObject.SetActive(false);
        }

        Path path = null;
        ChronoInfos infos = new ChronoInfos();
        float timeLimit = m_HasLimitSearchTime ? m_TimeLimit : -1;
        switch (_SearchMode)
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

        Debug.Log(infos.ToLogMessage());

        m_Hud?.SetChronoLog(infos);

        if (m_PathVisualizer && path != null)
        {
            m_PathVisualizer.gameObject.SetActive(true);
            m_PathVisualizer.TracePath(path, m_GameGrid.TilesRows);
        }
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

    private void GenerateMap(string _MapName)
    {
        if (!m_GridMaker)
            return;

        m_GameGrid = m_GridMaker.CreateGrid(_MapName);
        m_GridClustered = null;

        if (m_GameGrid == null)
            return;

        Camera.main.transform.position = new Vector3(m_GameGrid.TilesColumns / 2f, 10, m_GameGrid.TilesRows / 2f);
        Camera.main.orthographicSize = Mathf.Max(m_GameGrid.TilesRows, m_GameGrid.TilesColumns) / 2f + 1;
    }

    private void GenerateClusters(int _RowsSize, int _ColumnsSize)
    {
        if (m_GameGrid == null)
            return;

        m_GridClustered = new GridClustered(m_GameGrid, _RowsSize, _ColumnsSize);
        m_SizeClusterRows = _RowsSize;
        m_SizeClusterColumns = _ColumnsSize;
    }
    #endregion

    #region Private Attributes
    [Header("Pathfinding Mode")]
    [SerializeField] private Pathfinder.PathfindingMode m_PathfindingMode = Pathfinder.PathfindingMode.PM_None;
    [SerializeField] private float m_TimeLimit = 1f;
    [SerializeField] private bool m_HasLimitSearchTime = true;

    [Header("HUD")]
    [SerializeField] private Hud m_Hud = null;

    [Header("Path Visualizer")]
    [SerializeField] private Transform m_StartMark = null;
    [SerializeField] private Transform m_EndMark = null;
    [SerializeField] private Transform m_HoverMark = null;
    [SerializeField] private PathVisualizer m_PathVisualizer = null;

    [Header("Inputs")]
    [SerializeField] private PlayerInputs m_PlayerInputs = null;

    [Header("Grid Settings")]
    [SerializeField] private bool m_AutoGenerateMap = false;
    [SerializeField] private GridMaker m_GridMaker = null;

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
