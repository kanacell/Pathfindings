using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Public Methods
    public void SearchPath()
    {
        if (!m_StartTile || !m_EndTile)
            return;

        Path path = null;
        Stopwatch chrono = new Stopwatch();
        switch (m_PathfindingMode)
        {
            case Pathfinder.PathfindingMode.PM_Dijkstra:
                chrono.Start();
                path = Pathfinder.SearchDijkstraPathFromTo(m_StartTile, m_EndTile);
                chrono.Stop();
                break;
            case Pathfinder.PathfindingMode.PM_AStar:
                chrono.Start();
                path = null;
                chrono.Stop();
                break;
            case Pathfinder.PathfindingMode.PM_HPA:
                chrono.Start();
                path = null;
                chrono.Stop();
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
            m_ChronoText.text = $"chrono : {chrono.ElapsedMilliseconds} (ms)";
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

    #region Protected Methods
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MarkAsStart(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            MarkAsEnd(Input.mousePosition);
        }
    }

    private void MarkAsStart(Vector3 _ScreenPos)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            GameTile tile = hitInfos.transform.GetComponent<GameTile>();
            if (!tile)
                return;

            if (tile == m_StartTile)
                return;

            m_StartTile?.Unmark();
            m_StartTile = tile;
            m_StartTile.MarkAsStart();
        }
    }

    private void MarkAsEnd(Vector3 _ScreenPos)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(_ScreenPos);
        RaycastHit hitInfos;
        if (Physics.Raycast(screenRay, out hitInfos, 100))
        {
            GameTile tile = hitInfos.transform.GetComponent<GameTile>();
            if (!tile)
                return;

            if (tile == m_EndTile)
                return;

            m_EndTile?.Unmark();
            m_EndTile = tile;
            m_EndTile.MarkAsEnd();
        }
    }

    private void MoveCamera()
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");
    }

    private void ZoomCamera()
    {

    }
    #endregion

    #region Getters/Setters
    #endregion

    #region Public Attributes
    #endregion

    #region Protected Attributes
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

    private GameTile m_StartTile = null;
    private GameTile m_EndTile = null;
    #endregion
}
