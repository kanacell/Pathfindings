using System.Collections;
using System.Collections.Generic;
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
            m_StartTile = m_GridCreator.GetTileAt(m_RowStart, m_ColumnStart);
        }

        if (m_EndTile == null)
        {
            m_EndTile = m_GridCreator.GetTileAt(m_RowEnd, m_ColumnEnd);
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
                path = null;
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
        }

        if (path != null)
        {
            if (!m_PathVisualizer)
                return;

            m_PathVisualizer.gameObject.SetActive(true);
            m_PathVisualizer.TracePath(path, m_GridCreator.Rows);
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
        int row = m_GridCreator.Rows -  Mathf.FloorToInt(_Point.z) - 1;
        int column = Mathf.FloorToInt(_Point.x);

        return m_GridCreator.GetTileAt(row, column);
    }

    private void PlaceMarkerAt(Transform _Marker, Tile _TileAbove)
    {
        _Marker.gameObject.SetActive(true);

        float x = _TileAbove.Column + 0.5f;
        float z = m_GridCreator.Rows - _TileAbove.Row - 1 + 0.5f;
        _Marker.position = new Vector3(x, 0, z); ;
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

    [Header("Grid reference")]
    [SerializeField] private GridCreator m_GridCreator = null;
    [SerializeField] private int m_RowStart = 0;
    [SerializeField] private int m_ColumnStart = 0;
    [SerializeField] private int m_RowEnd = 0;
    [SerializeField] private int m_ColumnEnd = 0;

    private Tile m_StartTile = null;
    private Tile m_EndTile = null;
    #endregion
}
