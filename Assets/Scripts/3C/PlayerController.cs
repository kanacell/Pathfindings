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
        if (!m_StartTile)
        {
            m_StartTile = m_GameGrid.GetTileAt(m_RowStart, m_ColumnStart);
        }

        if (!m_EndTile)
        {
            m_EndTile = m_GameGrid.GetTileAt(m_RowEnd, m_ColumnEnd);
        }

        if (!m_StartTile || !m_EndTile)
            return;

        Path path = null;
        ChronoInfos infos = new ChronoInfos();
        switch (m_PathfindingMode)
        {
            case Pathfinder.PathfindingMode.PM_Dijkstra:
                path = Pathfinder.SearchDijkstraPathFromTo(m_StartTile, m_EndTile, out infos);
                break;
            case Pathfinder.PathfindingMode.PM_AStar:
                //path = Pathfinder.AstarWiki(m_StartTile, m_EndTile);
                path = Pathfinder.AStarCustomBasic(m_StartTile, m_EndTile, out infos);
                //path = Pathfinder.AStarAlgo(m_StartTile, m_EndTile);
                break;
            case Pathfinder.PathfindingMode.PM_HPA:
                path = null;
                break;
            default:
                break;
        }

        if (m_PathFoundText)
        {
            m_PathFoundText.text = $"path found : {path != null}";
        }

        if (m_ChronoText)
        {
            m_ChronoText.text = $"elasped time : {infos.ElapsedTime} ms\n";
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
            m_PathVisualizer.TracePath(path);
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
    }

    private void MarkAsStart(Vector3 _ScreenPos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            GameTile tile = hitInfos.transform.GetComponent<GameTile>();
            if (!tile || tile == m_StartTile || !tile.IsAccessible)
                return;

            m_StartTile?.Unmark();
            m_StartTile = tile;
            m_StartTile.MarkAsStart();
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
            GameTile tile = hitInfos.transform.GetComponent<GameTile>();
            if (!tile || tile == m_EndTile || !tile.IsAccessible)
                return;

            m_EndTile?.Unmark();
            m_EndTile = tile;
            m_EndTile.MarkAsEnd();
        }
    }
    #endregion

    #region Private Attributes
    [Header("Pathfinding Mode")]
    [SerializeField] private Pathfinder.PathfindingMode m_PathfindingMode = Pathfinder.PathfindingMode.PM_None;

    [Header("Path UI")]
    [SerializeField] private TextMeshProUGUI m_PathFoundText = null;
    [SerializeField] private TextMeshProUGUI m_ChronoText = null;

    [Header("Path Visualizer")]
    [SerializeField] private PathVisualizer m_PathVisualizer = null;

    [Header("Inputs")]
    [SerializeField] private PlayerInputs m_PlayerInputs = null;

    [Header("Grid reference")]
    [SerializeField] private GameGrid m_GameGrid = null;
    [SerializeField] private int m_RowStart = 0;
    [SerializeField] private int m_ColumnStart = 0;
    [SerializeField] private int m_RowEnd = 0;
    [SerializeField] private int m_ColumnEnd = 0;

    private GameTile m_StartTile = null;
    private GameTile m_EndTile = null;
    #endregion
}
